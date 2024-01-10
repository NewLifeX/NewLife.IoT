using NewLife.IoT.ThingModels;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace NewLife.IoT.Drivers;

/// <summary>异步协议驱动基类。抽象各种硬件设备的数据采集及远程控制</summary>
/// <typeparam name="TNode">节点类型，可使用默认Node</typeparam>
/// <typeparam name="TParameter"></typeparam>
public class AsyncDriverBase<TNode, TParameter> : AsyncDriverBase
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

/// <summary>异步协议驱动基类。抽象各种硬件设备的数据采集及远程控制</summary>
/// <remarks>
/// 在Modbus协议上，一个通信链路（串口/ModbusTcp地址）即是IDriver，可能有多个物理设备共用，各自表示为INode。
/// 即使是一个物理设备，也可能因为管理需要而划分为多个逻辑设备，例如变配电网关等Modbus汇集网关。
/// 
/// 架构设计需要，本类继承自DriverBase，将来可能移除该继承关系。
/// </remarks>
public abstract class AsyncDriverBase : DriverBase, IAsyncDriver
{
    #region 核心方法
    /// <summary>
    /// 打开设备驱动，传入参数。一个物理设备可能有多个逻辑设备共用，需要以节点来区分
    /// </summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">参数。不同驱动的参数设置相差较大，对象字典具有较好灵活性，其对应IDriverParameter</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    public virtual Task<INode> OpenAsync(IDevice device, IDriverParameter? parameter)
    {
        var node = new Node
        {
            Driver = this,
            Device = device,
        };

        return TaskEx.FromResult(node as INode);
    }

    /// <summary>
    /// 关闭设备节点。多节点共用通信链路时，需等最后一个节点关闭才能断开
    /// </summary>
    /// <param name="node"></param>
#if NET40 || NET45
    public virtual Task CloseAsync(INode node) => TaskEx.FromResult(0);
#else
    public virtual Task CloseAsync(INode node) => TaskEx.CompletedTask;
#endif

    /// <summary>读取数据</summary>
    /// <remarks>
    /// 驱动实现数据采集的核心方法，各驱动全力以赴实现好该接口。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合</param>
    /// <returns></returns>
    public virtual Task<IDictionary<String, Object?>> ReadAsync(INode node, IPoint[] points) => throw new NotImplementedException();

    /// <summary>写入数据</summary>
    /// <remarks>
    /// 驱动实现远程控制的核心方法，各驱动全力以赴实现好该接口。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="point">点位</param>
    /// <param name="value">数值</param>
    public virtual Task<Object?> WriteAsync(INode node, IPoint point, Object? value) => throw new NotImplementedException();

    /// <summary>控制设备，特殊功能使用</summary>
    /// <remarks>
    /// 除了点位读写之外的其它控制功能。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="parameters">参数</param>
    public virtual Task<Object?> ControlAsync(INode node, IDictionary<String, Object?> parameters) => throw new NotImplementedException();
    #endregion

    #region 覆盖同步接口
    /// <summary>
    /// 打开设备驱动，传入参数。一个物理设备可能有多个逻辑设备共用，需要以节点来区分
    /// </summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">参数。不同驱动的参数设置相差较大，对象字典具有较好灵活性，其对应IDriverParameter</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    public override INode Open(IDevice device, IDriverParameter? parameter) => OpenAsync(device, parameter).ConfigureAwait(false).GetAwaiter().GetResult();

    /// <summary>
    /// 关闭设备节点。多节点共用通信链路时，需等最后一个节点关闭才能断开
    /// </summary>
    /// <param name="node"></param>
    public override void Close(INode node) => CloseAsync(node).ConfigureAwait(false).GetAwaiter().GetResult();

    /// <summary>读取数据</summary>
    /// <remarks>
    /// 驱动实现数据采集的核心方法，各驱动全力以赴实现好该接口。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合</param>
    /// <returns></returns>
    public override IDictionary<String, Object?> Read(INode node, IPoint[] points) => ReadAsync(node, points).ConfigureAwait(false).GetAwaiter().GetResult();

    /// <summary>写入数据</summary>
    /// <remarks>
    /// 驱动实现远程控制的核心方法，各驱动全力以赴实现好该接口。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="point">点位</param>
    /// <param name="value">数值</param>
    public override Object? Write(INode node, IPoint point, Object? value) => WriteAsync(node, point, value).ConfigureAwait(false).GetAwaiter().GetResult();

    /// <summary>控制设备，特殊功能使用</summary>
    /// <remarks>
    /// 除了点位读写之外的其它控制功能。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="parameters">参数</param>
    public override Object? Control(INode node, IDictionary<String, Object?> parameters) => ControlAsync(node, parameters).ConfigureAwait(false).GetAwaiter().GetResult();
    #endregion
}