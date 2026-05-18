using System.Reflection;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using NewLife.Log;
using NewLife.Serialization;

namespace NewLife.IoT.Drivers;

/// <summary>协议驱动基类（泛型版）。提供泛型节点和参数类型支持</summary>
/// <typeparam name="TNode">节点类型，可使用默认Node</typeparam>
/// <typeparam name="TParameter">驱动参数类型</typeparam>
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
    /// <summary>打开设备驱动，传入参数。一个物理设备可能有多个逻辑设备共用，需要以节点来区分</summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">参数。不同驱动的参数设置相差较大，对象字典具有较好灵活性，其对应IDriverParameter</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    public override Task<INode> OpenAsync(IDevice device, IDriverParameter? parameter, CancellationToken cancellationToken = default)
    {
        var node = new TNode
        {
            Driver = this,
            Device = device,
            Parameter = parameter,
        };

        return TaskEx.FromResult(node as INode);
    }
    #endregion
}

/// <summary>协议驱动基类。实现IDriver异步接口，派生类直接重写OpenAsync/ReadAsync/WriteAsync等虚方法</summary>
/// <remarks>
/// 在Modbus协议上，一个通信链路（串口/ModbusTcp地址）即是IDriver，可能有多个物理设备共用，各自表示为INode。
/// 即使是一个物理设备，也可能因为管理需要而划分为多个逻辑设备，例如变配电网关等Modbus汇集网关。
/// </remarks>
public abstract class DriverBase : DisposeBase, IDriver, ILogFeature, ITracerFeature
{
    #region 属性
    /// <summary>服务提供者。驱动可在此取得外部注入到容器中的服务对象</summary>
    /// <remarks>
    /// 例如：Modbus驱动可以获取外部注入的IBoard服务对象，在A2工业计算机中，借助其中的Map方法把串口COM1映射到/dev/ttyAMA0。
    /// </remarks>
    public IServiceProvider? ServiceProvider { get; set; }

    /// <summary>诊断模式开关。开启后 ReadResult/WriteResult 将填充 RequestBytes/ResponseBytes 原始帧数据</summary>
    /// <remarks>默认关闭；仅在调试、问题排查阶段开启，生产环境关闭以避免额外内存分配</remarks>
    public Boolean Diagnostics { get; set; }
    #endregion

    #region 事件
    /// <summary>数据到达事件。推送型驱动（MQTT/WebSocket/OPC-UA等）在收到设备数据时触发</summary>
    public event EventHandler<DriverDataEventArgs>? DataReceived;

    /// <summary>触发数据到达事件</summary>
    /// <param name="node">节点对象</param>
    /// <param name="points">数据对应的点位集合</param>
    /// <param name="result">读取结果</param>
    protected void RaiseDataReceived(INode node, IPoint[] points, ReadResult result)
        => DataReceived?.Invoke(this, new DriverDataEventArgs { Node = node, Points = points, Result = result });
    #endregion

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

    /// <summary>创建驱动参数对象（虚钩子，派生类重写以返回具体参数类型）</summary>
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
                ProductKey = type.GetCustomAttribute<DriverAttribute>()?.Name ?? type.Name.TrimSuffix("Protocol", "Driver")
            }
        };

        return OnGetSpecification(spec) ? spec : null;
    }

    /// <summary>填充产品物模型（虚钩子，派生类重写以提供具体物模型内容）</summary>
    /// <param name="thingSpec">待填充的物模型对象</param>
    /// <returns>是否填充成功</returns>
    protected virtual Boolean OnGetSpecification(ThingSpec thingSpec) => false;
    #endregion

    #region 核心方法
    /// <summary>打开设备驱动，传入参数。一个物理设备可能有多个逻辑设备共用，需要以节点来区分</summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">驱动参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    public virtual Task<INode> OpenAsync(IDevice device, IDriverParameter? parameter, CancellationToken cancellationToken = default)
    {
        var node = new Node
        {
            Driver = this,
            Device = device,
        };
        return TaskEx.FromResult(node as INode);
    }

    /// <summary>关闭设备节点</summary>
    /// <param name="node">节点对象</param>
    /// <param name="cancellationToken">取消令牌</param>
    public virtual Task CloseAsync(INode node, CancellationToken cancellationToken = default) => TaskEx.CompletedTask;

    /// <summary>读取数据</summary>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>读取结果，包含点位数据、质量码和可选诊断帧</returns>
    public virtual Task<ReadResult> ReadAsync(INode node, IPoint[] points, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    /// <summary>批量写入数据（IDriver 核心方法）。派生类重写时自行判断 requests.Length 以决定走单点或批量路径</summary>
    /// <param name="node">节点对象</param>
    /// <param name="requests">写入请求数组，长度为 1 时表示单点写入</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量写入结果，AffectedCount 为实际成功点数</returns>
    public virtual Task<WriteResult> WriteAsync(INode node, WriteRequest[] requests, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    /// <summary>控制设备</summary>
    /// <param name="node">节点对象</param>
    /// <param name="request">控制请求，携带服务名和输入参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>控制结果，携带输出参数</returns>
    public virtual Task<ControlResult> ControlAsync(INode node, ControlRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
    #endregion

    #region 日志
    /// <summary>日志</summary>
    public ILog Log { get; set; } = Logger.Null;

    /// <summary>性能追踪器</summary>
    public ITracer? Tracer { get; set; }

    /// <summary>写日志</summary>
    /// <param name="format">格式字符串</param>
    /// <param name="args">参数列表</param>
    public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);
    #endregion
}