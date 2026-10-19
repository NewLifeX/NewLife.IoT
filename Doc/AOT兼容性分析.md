# NewLife.IoT AOT 兼容性现状与边界

> 基线：NewLife.Core v12｜评估方式：**NativeAOT 真实编译 + 运行实测**（win-x64 / .NET 10）｜冒烟工程：`AotSmoke/`

---

## 1. 现状摘要

- **核心链路实测全通**：11 个场景冒烟 JIT/AOT 双通，覆盖多语言词表与带前缀日志、类型映射、字节序编解码、`DriverCodec`、`ThingSpec` JSON 往返、结果类型与异常翻译、驱动工厂注册与参数创建、驱动物模型、传输层前缀、数据库参数。
- **支持方式**：`IsAotCompatible` 对 net8.0+ 常驻启用；JSON 成员访问由 NewLife.Core 源生成器生成静态表，元数据保留由 `JsonPreserve` 的 `[DynamicDependency]` 声明；驱动工厂提供泛型注册入口，避免运行期反射构造。
- **告警面**：库自身编译 **0** 条 IL 告警，AOT 发布 **0** 条 ILC 告警。
- **已知边界**：插件扫描（`DriverFactory.ScanAll`）属于动态发现能力，AOT 下不支持；`DriverFactory.Register(name, Type)` 依赖运行期反射构造，AOT 下应改用 `Register<T>` 或 `Register(name, type, factory)`。

---

## 2. 冒烟场景

| # | 场景 | 说明 |
|---|------|------|
| 1 | Localization 多语言词表 | `zh-CN` → `zh-Hans` 翻译；未提供语言回退英文 |
| 2 | 带前缀日志与本地化 | `[IoTSerial]` 前缀 + 翻译后的消息 |
| 3 | TypeHelper 类型映射 | `Int16` ⇄ `short`/`int`，点位类型解析 |
| 4 | DataHelper 字节序编解码 | BigEndian `UInt16` 往返 |
| 5 | DriverCodec 编解码 | HEX 编码与解码 |
| 6 | ThingSpec JSON 往返 | `ToJson`/`FromJson` 属性不丢失 |
| 7 | 结果类型与异常翻译 | `ReadResult`/`WriteResult`/`ControlResult`/`IoTException` |
| 8 | 驱动注册与参数创建 | `DriverFactory.Register<T>` + JSON 参数绑定 |
| 9 | 驱动物模型 | `GetSpecification` 与 `DriverAttribute` 反射特性 |
| 10 | 传输层前缀 | 串口/TCP/HTTP 端点前缀 |
| 11 | 数据库参数 | `DatabaseParameter` JSON 绑定 |

> AOT 判真：原生发布件入口 `Assembly.Location` 为空，`RuntimeFeature.IsDynamicCodeSupported=false`。

---

## 3. 关键实现

### 3.1 目标框架与构造参数

- net8.0/net10.0 目标常驻启用 `<IsAotCompatible>true</IsAotCompatible>`，编译期 AOT 分析器常开。
- `PublishAot` 只写在应用工程（`AotSmoke.csproj`），不要用命令行全局属性传入，否则会波及多 TFM 依赖项目的旧目标框架。

### 3.2 JSON 模型

```csharp
[System.Text.Json.Serialization.JsonSerializable(typeof(ThingSpec))]
public class ThingSpec { ... }
```

- `[JsonSerializable]` 让 NewLife.Core 源生成器生成成员访问器表，命中时零反射。
- `JsonPreserve` 以 `[DynamicDependency(PublicProperties | PublicFields | PublicParameterlessConstructor, typeof(T))]` 向裁剪器声明保留成员元数据；模块初始化器保证声明生效。
- 新增需要 JSON 往返的模型时，**同时**打上 `[JsonSerializable]` 并登记到对应项目的 `JsonPreserve`。

### 3.3 驱动工厂

```csharp
// AOT 推荐：泛型约束生成直接构造调用
DriverFactory.Register<IoTSerialDriver>("IoTSerial");

// AOT 推荐：显式工厂
DriverFactory.Register("IoTSerial", typeof(IoTSerialDriver), () => new IoTSerialDriver());

// 仅兼容旧代码：依赖运行期反射构造，AOT 下需要额外保留无参构造
DriverFactory.Register("IoTSerial", typeof(IoTSerialDriver));
```

---

## 4. 已知边界（收口记录）

| 项 | 位置 | 现状与理由 |
|---|---|---|
| 驱动插件扫描 | `DriverFactory.ScanAll` | 程序集与类型发现属 AOT 设计性不支持；AOT 应用应显式注册驱动 |
| 运行时 Type 注册 | `DriverFactory.Register(String?, Type)` | 反射构造依赖裁剪器保留无参构造；AOT 场景改用泛型或工厂重载 |
| 运行时特性读取 | `DriverBase.GetSpecification` / `DriverFactory.ScanAll` | 特性已通过 `GetCustomAttribute<DriverAttribute>()` 静态类型访问，主链可用；任意特性枚举不在支持范围 |
| 反射式 JSON 未标记类型 | `ThingSpec.FromJson` / `DriverBase.CreateParameter` | 未标记 `[JsonSerializable]` 的派生类型会回退反射路径，AOT 下成员可能被裁剪；自定义模型请自行标记并声明保留 |
| 数据库驱动查询链路 | `IoTDatabaseDriver.ReadAsync` → XCode DAL | 实际查询能力取决于 NewLife.XCode 与具体数据库提供程序的 AOT 支持；本仓库冒烟覆盖驱动创建与参数绑定，不覆盖数据库提供程序原生依赖 |

---

## 5. 复现与 CI

```powershell
# JIT 基线（期望 ALL PASS）
dotnet run -c Release --project AotSmoke\AotSmoke.csproj

# NativeAOT 真编译 + 运行（期望 0 条 ILC 告警，ALL PASS）
dotnet publish AotSmoke\AotSmoke.csproj -c Release -r win-x64 -o out\win-x64
.\out\win-x64\AotSmoke.exe
```

CI：`.github/workflows/aot.yml`，包含库编译（AOT 分析器常开）与 AOT 冒烟：win-x64 / linux-x64 / linux-arm64 为阻断门禁，osx-arm64 为观察期（`continue-on-error`），任一阻断项失败即失败。

（完）
