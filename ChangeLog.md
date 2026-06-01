# NewLife.IoT 变更日志

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
