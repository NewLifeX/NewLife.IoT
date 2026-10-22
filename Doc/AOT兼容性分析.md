# NewLife.IoT AOT 兼容性现状与边界

> 基线：NewLife.Core v12｜评估方式：**NativeAOT 真实编译 + 运行实测**（win-x64 / .NET 10）｜冒烟工程：`AotSmoke/`

---

## 1. 现状摘要

- **核心链路实测全通**：20 个场景冒烟 JIT/AOT 双通，覆盖多语言词表（全部 31 种语言）与带前缀日志、类型映射、驱动元数据与参数描述本地化、字节序编解码、`DriverCodec`、`ThingSpec`/结果类型/上报模型 JSON 往返、XML/JSON 参数绑定、结果类型与异常翻译、驱动工厂注册与参数创建、串口/网络/数据库参数绑定、驱动物模型、传输层前缀、板卡串口/Modbus 工厂注入、AOT 插件扫描守卫与串口原生库加载。
- **支持方式**：`IsAotCompatible` 对 net8.0+ 常驻启用；JSON 成员访问由 NewLife.Core 源生成器生成静态表，元数据保留由 `JsonPreserve` 的 `[DynamicDependency]` 声明；驱动工厂提供泛型注册与带 `DynamicallyAccessedMembers` 注解的 Type 注册入口，板卡串口/Modbus 提供工厂委托，AOT 下均可正常构造。
- **告警面**：库自身编译 **0** 条 IL 告警，AOT 发布 **0** 条 ILC 告警。
- **已知边界**：插件扫描（`DriverFactory.ScanAll`）属于动态发现能力，AOT 下不支持；直接向 `DriverFactory.Map` 塞入类型会绕过 Type 注册的裁剪注解，应使用 `Register<T>`/`Register(name, type)`/`Register(name, type, factory)`。

---

## 2. 冒烟场景

| # | 场景 | 说明 |
|---|------|------|
| 1 | Localization 多语言词表 | `zh-CN` → `zh-Hans` 翻译；未提供语言回退英文 |
| 2 | 全语言词表（31 种） | 逐语言校验关键运行时词条；单元测试按词表文件校验全部 72 个词条（含与英文同形的术语） |
| 3 | 带前缀日志与本地化 | `[IoTSerial]` 前缀 + 翻译后的消息，含 `LogCulture` 覆盖 |
| 4 | TypeHelper 类型映射 | `Int16` ⇄ `short`/`int`，点位类型解析 |
| 5 | DataHelper 字节序编解码 | BigEndian `UInt16` 往返 |
| 6 | DriverCodec 编解码 | HEX 编码与解码 |
| 7 | ThingSpec JSON 往返 | `ToJson`/`FromJson` 属性、数据类型规范、事件与服务不丢失 |
| 8 | 结果类型与异常翻译 | `ReadResult`/`WriteResult`/`ControlResult`/`IoTException` |
| 9 | 驱动注册与参数创建 | `DriverFactory.Register<T>` + JSON 参数绑定 |
| 10 | 驱动参数 Xml 解析 | `DriverBase.CreateParameter` 的 XML 分支 |
| 11 | 驱动物模型 | `GetSpecification` 与 `DriverAttribute` 反射特性 |
| 12 | 传输层前缀 | 串口/TCP/HTTP 端点前缀 |
| 13 | 网络驱动参数 | `Register(name, Type)` + `SocketParameter`/`HttpParameter` JSON 绑定 |
| 14 | 结果类型 JSON | `WriteResult` 序列化与反序列化 |
| 15 | 上报模型 JSON | `PropertyModels`/`EventModels`/`DataModels`/`ShadowModel` 往返 |
| 16 | AOT 插件扫描守卫 | AOT 下 `ScanAll` 直接返回，不执行反射扫描 |
| 17 | 串口实现加载 | `DefaultSerialPort` 在 AOT 下加载 `System.IO.Ports` 原生库并处理不存在的端口 |
| 18 | 数据库参数 | `DatabaseParameter` JSON 绑定 |
| 19 | 板卡工厂注入 | `SerialPortFactory`/`ModbusFactory` 覆盖 `Board.CreateSerial`/`CreateModbus`，IoTSerial 模块初始化自动注册串口工厂 |
| 20 | 驱动元数据本地化 | `LocalizedDisplayName`/`LocalizedDescription` 在 AOT 下可反射读取；驱动显示名、枚举描述与参数 Xml 注释随语言切换 |

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

// Type 注册：参数已标注 DynamicallyAccessedMembers，裁剪器保留公开无参构造，
// 注册时即生成工厂委托，Create 阶段不再依赖运行期反射
DriverFactory.Register("IoTSerial", typeof(IoTSerialDriver));
```

### 3.4 板卡工厂

```csharp
// NewLife.IoTSerial 模块初始化时自动注册；也可在应用启动时覆盖为自定义串口
SerialPortFactory.Factory = (portName, baudrate) => new DefaultSerialPort
{
    PortName = portName,
    Baudrate = baudrate,
};

// Modbus 实现位于独立包，AOT 应用需显式注册
ModbusFactory.Factory = (portName, baudrate) => new ModbusRtu
{
    PortName = portName,
    Baudrate = baudrate,
};

var sp = new Board().CreateSerial("COM1", 9600);
```

---

## 4. 已知边界（收口记录）

| 项 | 位置 | 现状与理由 |
|---|---|---|
| 驱动插件扫描 | `DriverFactory.ScanAll` | 程序集与类型发现属 AOT 设计性不支持；已标注 `RequiresUnreferencedCode`/`RequiresDynamicCode` 并在 AOT 下直接返回；AOT 应用应显式注册驱动 |
| 运行时 Type 注册 | `DriverFactory.Register(String?, Type)` | 参数已标注 `DynamicallyAccessedMembers(PublicParameterlessConstructor)`，注册时生成工厂委托；如果直接向 `DriverFactory.Map` 塞入类型则绕过该保证 |
| 字典参数转换 | `DriverExtensions.Open(IDictionary)` | 依赖运行时类型成员访问，已标注 `RequiresUnreferencedCode`/`RequiresDynamicCode`；AOT 场景使用强类型参数重载 |
| 板卡动态创建 | `Board.CreateSerial` / `Board.CreateModbus` | 优先走 `SerialPortFactory`/`ModbusFactory` 委托；`NewLife.IoTSerial` 模块初始化时自动注册串口工厂，Modbus 由应用注册。未注册时保留旧版运行期类型查找作为兼容回退，AOT 应用应显式注入 |
| 运行时特性读取 | `DriverBase.GetSpecification` / `DriverFactory.ScanAll` | 特性已通过 `GetCustomAttribute<DriverAttribute>()` 静态类型访问，主链可用；任意特性枚举不在支持范围 |
| 反射式 JSON 未标记类型 | `ThingSpec.FromJson` / `DriverBase.CreateParameter` | 未标记 `[JsonSerializable]` 的派生类型会回退反射路径，AOT 下成员可能被裁剪；自定义模型请自行标记并声明保留 |
| 数据库驱动查询链路 | `IoTDatabaseDriver.ReadAsync` → XCode DAL | 实际查询能力取决于 NewLife.XCode 与具体数据库提供程序的 AOT 支持；本仓库冒烟覆盖驱动创建与参数绑定，不覆盖数据库提供程序原生依赖 |
| 大端平台实测 | `DataHelper` / `PointHelper` | 已按 `BitConverter.IsLittleEndian` 适配主机端序，显式字节序用例保证小端 CI 下的确定性；s390x 等大端 RID 未接入 CI，需要硬件或仿真环境验证 |

---

## 5. 复现与 CI

```powershell
# JIT 基线（期望 ALL PASS；工程内含 PublishAot=true，需显式关闭以走 JIT）
dotnet run -c Release -p:PublishAot=false --project AotSmoke\AotSmoke.csproj

# NativeAOT 真编译 + 运行（期望 0 条 ILC 告警，ALL PASS）
dotnet publish AotSmoke\AotSmoke.csproj -c Release -r win-x64 -o out\win-x64
.\out\win-x64\AotSmoke.exe
```

CI：`.github/workflows/aot.yml`，包含库编译（AOT 分析器常开，IL2026/IL2070/IL2072/IL2075/IL3050 视为错误）与 AOT 冒烟：win-x64 / linux-x64 / linux-arm64 为阻断门禁，osx-arm64 为观察期（`continue-on-error`），任一阻断项失败即失败。标量指令集回归由 test.yml 的 `DOTNET_EnableHWIntrinsic=0` 任务覆盖；包消费验证（`tools/check-package-consumer.ps1 -Aot`）在 `package smoke` 任务中从本地 NuGet 源还原并 NativeAOT 发布运行。

（完）
