using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;

namespace NewLife.IoT.Drivers;

/// <summary>协议驱动接口。抽象各种硬件设备的数据采集及远程控制</summary>
/// <remarks>
/// 在Modbus协议上，一个通信链路（串口/ModbusTcp地址）即是IDriver，可能有多个物理设备共用，各自表示为INode。
/// 即使是一个物理设备，也可能因为管理需要而划分为多个逻辑设备，例如变配电网关等Modbus汇集网关。
/// </remarks>
public interface IDriver
{
    #region 元数据
    /// <summary>
    /// 获取默认驱动参数对象，可序列化成Xml/Json作为该协议的参数模板
    /// </summary>
    /// <returns></returns>
    IDriverParameter GetDefaultParameter();

    /// <summary>获取产品物模型。如果设备有固定点位属性、服务和事件，则直接返回，否则返回空</summary>
    /// <returns></returns>
    ThingSpec GetSpecification();
    #endregion

    #region 核心方法
    /// <summary>
    /// 打开设备驱动，传入参数。一个物理设备可能有多个逻辑设备共用，需要以节点来区分
    /// </summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameters">参数。不同驱动的参数设置相差较大，对象字典具有较好灵活性，其对应IDriverParameter</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    INode Open(IDevice device, IDictionary<String, Object> parameters);

    /// <summary>
    /// 关闭设备节点。多节点共用通信链路时，需等最后一个节点关闭才能断开
    /// </summary>
    /// <param name="node"></param>
    void Close(INode node);

    /// <summary>
    /// 读取数据
    /// </summary>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合</param>
    /// <returns></returns>
    IDictionary<String, Object> Read(INode node, IPoint[] points);

    /// <summary>
    /// 写入数据
    /// </summary>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="point">点位</param>
    /// <param name="value">数值</param>
    Object Write(INode node, IPoint point, Object value);

    /// <summary>
    /// 控制设备，特殊功能使用
    /// </summary>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="parameters">参数</param>
    void Control(INode node, IDictionary<String, Object> parameters);
    #endregion
}

/// <summary>扩展</summary>
public static class DriverExtensions
{
    /// <summary>
    /// 打开设备驱动
    /// </summary>
    /// <param name="driver">驱动对象</param>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">参数对象</param>
    /// <returns></returns>
    public static INode Open(this IDriver driver, IDevice device, IDriverParameter parameter)
    {
        var ps = parameter?.Serialize();
        var node = driver.Open(device, ps);

        node.Driver ??= driver;
        node.Device ??= device;
        node.Parameter ??= parameter;

        return node;
    }
}