# NewLife.IoT 变更日志

## v3.2.2026.1024 (2026-10-24)

### NativeAOT 支持
- **AOT 分析器常驻**：net8.0 及以上目标启用 IsAotCompatible，IL2026/IL2070/IL2072/IL2075/IL3050 作为错误阻断
- **驱动工厂免反射注册**：DriverFactory.Register 标注裁剪保留并生成构造委托，新增 SerialPortFactory/ModbusFactory 工厂注入，板卡驱动在 AOT 下不再依赖运行期反射
- **AOT 冒烟矩阵**：AotSmoke 扩展至 20 个场景，覆盖 win-x64/linux-x64/linux-arm64/osx-arm64 的 JIT 与 NativeAOT 双跑
- **裁剪安全 JSON**：ReadResult/ControlResult/DriverDataEventArgs 等模型补充 JsonSerializable 与裁剪保留声明

### 多语言本地化
- **31 种语言词表**：与 NewLife.Core 语言集完全对齐，新增 21 种语言资源，覆盖驱动元数据、参数描述、异常消息与日志模板
- **带前缀日志**：驱动与传输层日志统一实例前缀，并支持通过 LogCulture 切换输出语言
- **区域性无关处理**：JSON 与数值转换固定 InvariantCulture，不同区域性下输出一致

### 驱动与传输层
- **板卡工厂注入**：NewLife.IoTSerial 模块初始化时自动注册串口工厂，Board 优先使用工厂委托并保留反射回退
- **大端字节序修复**：DataHelper 的单精度/双精度转换按主机字节序换算，PointHelper 保持小端契约，修复大端主机上的浮点与点位转换错误

### 测试与质量
- **424 项单元测试**：新增多语言、驱动工厂、板卡工厂与字节序测试，全部通过
- **词表门禁**：新增本地化、英文源文、日志模板与运行时消息校验脚本，CI 拦截键集合、占位符与漏译问题
- **包消费验证**：以本地源消费者验证包内多语言、驱动工厂、参数绑定与板卡串口，并支持 NativeAOT 发布运行

### 文档与协作
- **README 13 语种**：默认语言切换为英文，生态版图与团队介绍对齐 NewLife.Core，新增繁體中文、Deutsch 与 한국어
- **依赖升级**：NewLife.Core 12.0.2026.1024、NewLife.XCode 12.2.2026.1021-beta0423、NewLife.Modbus 2.1.2026.1022-beta0202

### 发布流程
- **双通道发包**：发布工作流同时推送 GitHub Packages 与 NuGet.org，tag 发布时自动创建 GitHub Release，说明取自 ChangeLog 对应版本段落
- **统一 Ubuntu 运行**：打包、消费验证与发布均在 Ubuntu 完成，降低跨平台差异

---

## v3.1.2026.0802 (2026-08-02)

### 接口异步化增强
- **接口全面异步化**：IModbus/IRelayController/IDevice 接口全面异步化，所有方法支持 CancellationToken 取消令牌与异步上报
- **DriverCodec 编解码助手**：新增共享编解码助手，统一支持 HEX/ASCII/UTF8/Json 编解码，串口/网络通用驱动复用

### 性能优化
- **采集热点路径零分配化**：DataHelper/TypeHelper 热点路径优化，降低 GC 压力，提升采集吞吐

### 健壮性优化
- **驱动与传输层缺陷修复**：修复串口/网络驱动并发安全问题，请求-响应原子化，优化超时处理与资源清理
- **共享编解码与并发安全**：抽取共享编解码逻辑，通用驱动并发采集串行化执行

### 测试与质量
- **测试全覆盖**：新增 130+ 项单元测试，覆盖物模型/接口/驱动/传输层等全部未测试功能，372 项测试全部通过

### 文档与协作
- **文档全面重构**：统一中英双语与 v3.1 架构，新增多语言 README 与功能清单
- **依赖包升级**：多项目及 IoT 相关包版本常规升级

---

## v3.0.2026.0601 (2026-06-01)

### 驱动接口重构（重大变更）
- **IDriver 全异步化**：消除 IAsyncDriver/IDriver 双路判断，所有 I/O 操作统一为 Task 返回，简化驱动开发模型
- **结构化操作结果**：新增 ReadResult/WriteResult/ControlResult 结构，内置质量码、错误码、诊断帧，替代裸字典返回
- **结构化错误处理**：引入 IoTException/IoTErrorCode，支持高频失败场景的结构化错误传递
- **DataQuality 枚举**：采集结果支持多级质量标记（Good/Uncertain/Bad 等）
- **DataReceived 事件**：IDriver 新增事件，支持推送型驱动主动上报数据

### 传输层抽象
- **ITransport 接口**：新增统一传输层接口，整合串口/TCP/UDP/HTTP 传输，IDataPort 标记废弃
- **SerialTransport**：新增串口传输层实现（NewLife.IoTSerial）
- **HttpTransport / SocketTransport**：新增 HTTP/Socket/TCP/UDP 传输层实现（NewLife.IoTSocket）

### 控制接口规范化
- **ControlRequest/ControlResult**：将 ServiceCall/ServiceResult 重命名为 ControlRequest/ControlResult，统一"控制"语义；删除废弃 ServiceRequest 类型
- **WriteRequest**：新增结构化写入参数类型，提升扩展性

### 兼容性
- **DriverBase 异步桥接**：通过桥接机制兼容现有同步驱动
- 兼容 net45 ~ net10.0

---

## v2.7.2026.0501 (2026-05-01)

### 依赖升级
- **依赖包升级**：多次升级 NuGet 依赖包至最新版本

### 文档与协作
- **驱动描述统一**：统一驱动描述规范，新增开发/性能/网络指令文档

---

## v2.7.2026.0201 (2026-02-01)
- 升级依赖包版本

## v2.7.2026.0102 (2026-01-02)
- 修正物模型解码点位数据为小数时，数据精度丢失的问题

## v2.7.2025.1112 (2025-11-12)
- 支持.NET 10

## v2.6.2025.1001 (2025-10-01)
- 优化日志输出
- IoTUdp增加DiscoverAsync
- IoTSocket拆分为IoTTcp和IoTUdp两个驱动
- 新增IoTDatabase通用数据库驱动
- 通用驱动新增参数CaptureAll，捕获所有字段
- 完善通用Http驱动，支持Get和Post
- 新增通用串口驱动、通用网络驱动、通用数据库驱动

## v2.6.2025.0801 (2025-08-01)
- 新增IDiscoverableDriver，为支持设备自动发现的驱动定义契约
- Modbus驱动可以获取外部注入的IBoard服务对象
- 优化驱动扫描的日志显示
- DriverInfo增加参数类型名ParameterClassName

## v2.6.2025.0701 (2025-07-01)
- 升级物模型架构
- 点位地址和缩放因子字段移动到属性类
- 废弃PropertyExtend，地址和缩放因子等字段放置到DataSpecs中
- IDriver增加批量Write接口
- IAsyncDriver所有异步接口增加取消令牌参数
- 如果未指定字节序且使用了缩放因子，则先转换成大端整数

## v2.5.2025.0601 (2025-06-01)
- 新增IRelayController继电器控制器
