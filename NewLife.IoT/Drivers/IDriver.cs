using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using NewLife.Serialization;

namespace NewLife.IoT.Drivers;

/// <summary>协议驱动接口。抽象各种硬件设备的数据采集及远程控制</summary>
/// <remarks>
/// 在Modbus协议上，一个通信链路（串口/ModbusTcp地址）即是IDriver，可能有多个物理设备共用，各自表示为INode。
/// 即使是一个物理设备，也可能因为管理需要而划分为多个逻辑设备，例如变配电网关等Modbus汇集网关。
///
/// 除了具体设备实例化驱动对象，在物联网平台扫描驱动时，也有可能实例化驱动对象，以获取默认参数与产品物模型。
///
/// v3.0起，IDriver采用异步优先设计，所有I/O操作均为异步方法（Task返回）。
/// 同步便利方法由<see cref="DriverExtensions"/>扩展提供，不建议在并发采集场景中使用。
/// 驱动实现者继承<see cref="DriverBase"/>，重写受保护的同步虚钩子即可，无需感知异步。
/// </remarks>
public interface IDriver
{
    #region 事件
    /// <summary>数据到达事件。推送型驱动（MQTT/WebSocket/OPC-UA等）在收到设备数据时触发</summary>
    /// <remarks>轮询型驱动无需实现此事件；平台订阅后，驱动在数据到达时主动调用 RaiseDataReceived</remarks>
    event EventHandler<DriverDataEventArgs> DataReceived;
    #endregion

    #region 元数据
    /// <summary>创建驱动参数对象，分析参数配置或创建默认参数</summary>
    /// <remarks>
    /// 可序列化成Xml/Json作为该协议的参数模板。由于Xml需要良好的注释特性，优先使用。
    /// 获取后，按新版本覆盖旧版本。
    /// </remarks>
    /// <param name="parameter">Xml/Json参数配置，为空时仅创建默认参数</param>
    /// <returns>驱动参数对象</returns>
    IDriverParameter? CreateParameter(String? parameter = null);

    /// <summary>获取产品物模型</summary>
    /// <remarks>
    /// 如果设备有固定点位属性、服务和事件，则直接返回，否则返回空。
    /// 物联网平台有两种情况调用该接口：
    /// 1，打开设备后。常见于OPC/BACnet等，此时可获取特定设备场景的物模型。
    /// 2，扫描设备时。此时未连接任何设备，只能返回该类设备的通用物模型，常用于具体硬件产品，例如各种传感器。
    /// 获取后，按新版本覆盖旧版本。
    /// </remarks>
    /// <returns>产品物模型，无固定物模型时返回 null</returns>
    ThingSpec? GetSpecification();
    #endregion

    #region 核心异步方法
    /// <summary>打开设备驱动，传入参数。一个物理设备可能有多个逻辑设备共用，需要以节点来区分</summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">参数。不同驱动的参数设置相差较大，对象字典具有较好灵活性，其对应IDriverParameter</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    Task<INode> OpenAsync(IDevice device, IDriverParameter? parameter, CancellationToken cancellationToken = default);

    /// <summary>关闭设备节点。多节点共用通信链路时，需等最后一个节点关闭才能断开</summary>
    /// <param name="node">节点对象</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task CloseAsync(INode node, CancellationToken cancellationToken = default);

    /// <summary>读取数据</summary>
    /// <remarks>
    /// 驱动实现数据采集的核心方法，各驱动全力以赴实现好该接口。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// 返回 ReadResult 携带点位字典、数据质量码及诊断帧，比裸字典提供更丰富的语义。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>读取结果，包含点位数据、质量码和可选诊断帧</returns>
    Task<ReadResult> ReadAsync(INode node, IPoint[] points, CancellationToken cancellationToken = default);

    /// <summary>批量写入数据</summary>
    /// <remarks>
    /// 驱动实现远程控制的核心方法。
    /// 每个 WriteRequest 携带目标点位（Point）和写入值（Value）。
    /// DriverBase 默认实现逐项调用单点虚钩子 WriteAsync；支持批量帧的驱动可重写此方法以提高效率。
    /// 单点写入请使用 DriverExtensions.WriteAsync 扩展方法，内部将请求包装为单元素数组后调用本方法。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="requests">写入请求数组，每项含目标点位和值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量写入结果，AffectedCount 为实际成功点数</returns>
    Task<WriteResult> WriteAsync(INode node, WriteRequest[] requests, CancellationToken cancellationToken = default);

    /// <summary>控制设备，特殊功能使用</summary>
    /// <remarks>
    /// 除了点位读写之外的其它控制功能。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="request">服务调用请求，携带服务名和输入参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>服务调用结果，携带输出参数</returns>
    Task<ServiceResult> ControlAsync(INode node, ServiceCall request, CancellationToken cancellationToken = default);
    #endregion
}

/// <summary>驱动数据到达事件参数</summary>
public class DriverDataEventArgs : EventArgs
{
    /// <summary>节点对象</summary>
    public INode Node { get; init; } = null!;

    /// <summary>数据对应的点位集合</summary>
    public IPoint[] Points { get; init; } = [];

    /// <summary>读取结果，包含点位数据和质量码</summary>
    public ReadResult Result { get; init; } = null!;

    /// <summary>数据到达时刻（UTC）。默认取 Result.Timestamp</summary>
    public DateTime Timestamp => Result?.Timestamp ?? DateTime.UtcNow;
}

/// <summary>驱动扩展方法</summary>
public static class DriverExtensions
{
    /// <summary>打开设备驱动（同步便利方法，内部调用OpenAsync）</summary>
    /// <param name="driver">驱动对象</param>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameters">参数字典，将自动转换为驱动参数对象</param>
    /// <returns>节点对象</returns>
    public static INode Open(this IDriver driver, IDevice device, IDictionary<String, Object?> parameters)
    {
        var type = (driver.CreateParameter()?.GetType()) ?? throw new InvalidOperationException();

        var ps = JsonHelper.Default.Convert(parameters, type) as IDriverParameter;
        var node = driver.OpenAsync(device, ps).ConfigureAwait(false).GetAwaiter().GetResult();

        node.Driver ??= driver;
        node.Device ??= device;
        node.Parameter ??= ps;

        return node;
    }

    /// <summary>打开设备驱动（同步便利方法）</summary>
    /// <param name="driver">驱动对象</param>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">驱动参数</param>
    /// <returns>节点对象</returns>
    public static INode Open(this IDriver driver, IDevice device, IDriverParameter? parameter)
        => driver.OpenAsync(device, parameter).ConfigureAwait(false).GetAwaiter().GetResult();

    /// <summary>关闭设备节点（同步便利方法）</summary>
    /// <param name="driver">驱动对象</param>
    /// <param name="node">节点对象</param>
    public static void Close(this IDriver driver, INode node)
        => driver.CloseAsync(node).ConfigureAwait(false).GetAwaiter().GetResult();

    /// <summary>读取数据（同步便利方法）</summary>
    /// <param name="driver">驱动对象</param>
    /// <param name="node">节点对象</param>
    /// <param name="points">点位集合</param>
    /// <returns>读取结果</returns>
    public static ReadResult Read(this IDriver driver, INode node, IPoint[] points)
        => driver.ReadAsync(node, points).ConfigureAwait(false).GetAwaiter().GetResult();

    /// <summary>写入数据（单点异步便利方法）。将请求包装为单元素数组后调用批量 WriteAsync</summary>
    /// <param name="driver">驱动对象</param>
    /// <param name="node">节点对象</param>
    /// <param name="request">写入请求，携带目标点位、值及扩展元数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>写入结果，包含回显值和可选诊断帧</returns>
    public static Task<WriteResult> WriteAsync(this IDriver driver, INode node, WriteRequest request, CancellationToken cancellationToken = default)
        => driver.WriteAsync(node, [request], cancellationToken);

    /// <summary>写入数据（同步便利方法）</summary>
    /// <param name="driver">驱动对象</param>
    /// <param name="node">节点对象</param>
    /// <param name="point">点位</param>
    /// <param name="value">数值，自动包装为 WriteRequest</param>
    /// <returns>写入结果</returns>
    public static WriteResult Write(this IDriver driver, INode node, IPoint point, Object? value)
        => driver.WriteAsync(node, new WriteRequest(point, value)).ConfigureAwait(false).GetAwaiter().GetResult();

    /// <summary>控制设备（同步便利方法）</summary>
    /// <param name="driver">驱动对象</param>
    /// <param name="node">节点对象</param>
    /// <param name="request">服务调用请求</param>
    /// <returns>服务调用结果</returns>
    public static ServiceResult Control(this IDriver driver, INode node, ServiceCall request)
        => driver.ControlAsync(node, request).ConfigureAwait(false).GetAwaiter().GetResult();

    /// <summary>控制设备（同步便利方法，按服务名+参数字典）</summary>
    /// <param name="driver">驱动对象</param>
    /// <param name="node">节点对象</param>
    /// <param name="serviceName">服务名称</param>
    /// <param name="parameters">输入参数字典，可为空</param>
    /// <returns>服务调用结果</returns>
    public static ServiceResult Control(this IDriver driver, INode node, String serviceName, IDictionary<String, Object?>? parameters = null)
        => driver.ControlAsync(node, ServiceCall.Create(serviceName, parameters)).ConfigureAwait(false).GetAwaiter().GetResult();
}