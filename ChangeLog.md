# NewLife.IoT 变更日志

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
