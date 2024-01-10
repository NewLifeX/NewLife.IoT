using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;

namespace NewLife.IoT.Drivers;

/// <summary>协议驱动异步接口。抽象各种硬件设备的数据采集及远程控制</summary>
/// <remarks>
/// 在Modbus协议上，一个通信链路（串口/ModbusTcp地址）即是IDriver，可能有多个物理设备共用，各自表示为INode。
/// 即使是一个物理设备，也可能因为管理需要而划分为多个逻辑设备，例如变配电网关等Modbus汇集网关。
/// 
/// 除了具体设备实例化驱动对象，在物联网平台扫描驱动时，也有可能实例化驱动对象，以获取默认参数与产品物模型。
/// </remarks>
public interface IAsyncDriver
{
    #region 元数据
    /// <summary>创建驱动参数对象，分析参数配置或创建默认参数</summary>
    /// <remarks>
    /// 可序列化成Xml/Json作为该协议的参数模板。由于Xml需要良好的注释特性，优先使用。
    /// 获取后，按新版本覆盖旧版本。
    /// </remarks>
    /// <param name="parameter">Xml/Json参数配置，为空时仅创建默认参数</param>
    /// <returns></returns>
    IDriverParameter? CreateParameter(String? parameter = null);

    /// <summary>获取产品物模型</summary>
    /// <remarks>
    /// 如果设备有固定点位属性、服务和事件，则直接返回，否则返回空。
    /// 物联网平台有两种情况调用该接口：
    /// 1，打开设备后。常见于OPC/BACnet等，此时可获取特定设备场景的物模型。
    /// 2，扫描设备时。此时未连接任何设备，只能返回该类设备的通用物模型，常用于具体硬件产品，例如各种传感器。
    /// 获取后，按新版本覆盖旧版本。
    /// </remarks>
    /// <returns></returns>
    ThingSpec? GetSpecification();
    #endregion

    #region 核心方法
    /// <summary>
    /// 打开设备驱动，传入参数。一个物理设备可能有多个逻辑设备共用，需要以节点来区分
    /// </summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">参数。不同驱动的参数设置相差较大，对象字典具有较好灵活性，其对应IDriverParameter</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    Task<INode> OpenAsync(IDevice device, IDriverParameter? parameter);

    /// <summary>
    /// 关闭设备节点。多节点共用通信链路时，需等最后一个节点关闭才能断开
    /// </summary>
    /// <param name="node"></param>
    Task CloseAsync(INode node);

    /// <summary>读取数据</summary>
    /// <remarks>
    /// 驱动实现数据采集的核心方法，各驱动全力以赴实现好该接口。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合</param>
    /// <returns></returns>
    Task<IDictionary<String, Object?>> ReadAsync(INode node, IPoint[] points);

    /// <summary>写入数据</summary>
    /// <remarks>
    /// 驱动实现远程控制的核心方法，各驱动全力以赴实现好该接口。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="point">点位</param>
    /// <param name="value">数值</param>
    Task<Object?> WriteAsync(INode node, IPoint point, Object? value);

    /// <summary>控制设备，特殊功能使用</summary>
    /// <remarks>
    /// 除了点位读写之外的其它控制功能。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="parameters">参数</param>
    Task<Object?> ControlAsync(INode node, IDictionary<String, Object?> parameters);
    #endregion
}