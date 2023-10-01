using System.Reflection;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using NewLife.Log;
using NewLife.Serialization;

namespace NewLife.IoT.Drivers;

/// <summary>协议驱动基类。抽象各种硬件设备的数据采集及远程控制</summary>
/// <typeparam name="TNode">节点类型，可使用默认Node</typeparam>
/// <typeparam name="TParameter"></typeparam>
public class DriverBase<TNode, TParameter> : DriverBase
    where TNode : INode, new()
    where TParameter : IDriverParameter, new()
{
    #region 元数据
    /// <summary>创建驱动参数对象，分析参数配置或创建默认参数</summary>
    /// <returns></returns>
    protected override IDriverParameter OnCreateParameter() => new TParameter();
    #endregion

    #region 核心方法
    /// <summary>
    /// 打开设备驱动，传入参数。一个物理设备可能有多个逻辑设备共用，需要以节点来区分
    /// </summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">参数。不同驱动的参数设置相差较大，对象字典具有较好灵活性，其对应IDriverParameter</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    public override INode Open(IDevice device, IDriverParameter? parameter)
    {
        var node = new TNode
        {
            Driver = this,
            Device = device,
            Parameter = parameter,
        };

        return node;
    }
    #endregion
}

/// <summary>协议驱动基类。抽象各种硬件设备的数据采集及远程控制</summary>
/// <remarks>
/// 在Modbus协议上，一个通信链路（串口/ModbusTcp地址）即是IDriver，可能有多个物理设备共用，各自表示为INode。
/// 即使是一个物理设备，也可能因为管理需要而划分为多个逻辑设备，例如变配电网关等Modbus汇集网关。
/// </remarks>
public abstract class DriverBase : DisposeBase, IDriver, ILogFeature, ITracerFeature
{
    #region 元数据
    /// <summary>创建驱动参数对象，分析参数配置或创建默认参数</summary>
    /// <remarks>
    /// 可序列化成Xml/Json作为该协议的参数模板。由于Xml需要良好的注释特性，优先使用。
    /// 获取后，按新版本覆盖旧版本。
    /// </remarks>
    /// <param name="parameter">Xml/Json参数配置</param>
    /// <returns></returns>
    public virtual IDriverParameter? CreateParameter(String? parameter)
    {
        var p = OnCreateParameter();
        if (p == null) return null;

        if (!parameter.IsNullOrEmpty())
        {
            // 按Xml或Json解析参数成为字典
            var ps = parameter.StartsWith("<") && parameter.EndsWith(">") ?
                XmlParser.Decode(parameter) :
                JsonParser.Decode(parameter);

            // 字段转对象
            new JsonReader().ToObject(ps, null, p);
        }

        return p;
    }

    /// <summary>创建驱动参数对象，分析参数配置或创建默认参数</summary>
    /// <returns></returns>
    protected virtual IDriverParameter? OnCreateParameter() => null;

    /// <summary>获取产品物模型</summary>
    /// <remarks>
    /// 如果设备有固定点位属性、服务和事件，则直接返回，否则返回空。
    /// 物联网平台有两种情况调用该接口：
    /// 1，打开设备后。常见于OPC/BACnet等，此时可获取特定设备场景的物模型。
    /// 2，扫描设备时。此时未连接任何设备，只能返回该类设备的通用物模型，常用于具体硬件产品，例如各种传感器。
    /// 获取后，按新版本覆盖旧版本。
    /// </remarks>
    /// <returns></returns>
    public virtual ThingSpec? GetSpecification()
    {
        var type = GetType();
        var spec = new ThingSpec
        {
            Profile = new Profile
            {
                Version = type.Assembly.GetName().Version + "",
                ProductKey = type.GetCustomAttribute<DriverAttribute>()?.Name ?? type.Name.TrimEnd("Protocol", "Driver")
            }
        };

        return OnGetSpecification(spec) ? spec : null;
    }

    /// <summary>填充产品物模型</summary>
    /// <param name="thingSpec"></param>
    /// <returns>是否填充成功</returns>
    protected virtual Boolean OnGetSpecification(ThingSpec thingSpec) => false;
    #endregion

    #region 核心方法
    /// <summary>
    /// 打开设备驱动，传入参数。一个物理设备可能有多个逻辑设备共用，需要以节点来区分
    /// </summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">参数。不同驱动的参数设置相差较大，对象字典具有较好灵活性，其对应IDriverParameter</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    public virtual INode Open(IDevice device, IDriverParameter? parameter)
    {
        var node = new Node
        {
            Driver = this,
            Device = device,
        };

        return node;
    }

    /// <summary>
    /// 关闭设备节点。多节点共用通信链路时，需等最后一个节点关闭才能断开
    /// </summary>
    /// <param name="node"></param>
    public virtual void Close(INode node) { }

    /// <summary>读取数据</summary>
    /// <remarks>
    /// 驱动实现数据采集的核心方法，各驱动全力以赴实现好该接口。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合</param>
    /// <returns></returns>
    public virtual IDictionary<String, Object?> Read(INode node, IPoint[] points) => throw new NotImplementedException();

    /// <summary>写入数据</summary>
    /// <remarks>
    /// 驱动实现远程控制的核心方法，各驱动全力以赴实现好该接口。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="point">点位</param>
    /// <param name="value">数值</param>
    public virtual Object? Write(INode node, IPoint point, Object? value) => throw new NotImplementedException();

    /// <summary>控制设备，特殊功能使用</summary>
    /// <remarks>
    /// 除了点位读写之外的其它控制功能。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="parameters">参数</param>
    public virtual Object? Control(INode node, IDictionary<String, Object?> parameters) => throw new NotImplementedException();
    #endregion

    #region 日志
    /// <summary>日志</summary>
    public ILog Log { get; set; } = Logger.Null;

    /// <summary>性能追踪器</summary>
    public ITracer? Tracer { get; set; }

    /// <summary>写日志</summary>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);
    #endregion
}