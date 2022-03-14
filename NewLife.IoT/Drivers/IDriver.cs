using NewLife.IoT.ThingModels;
using NewLife.Log;

namespace NewLife.IoT.Drivers;

/// <summary>
/// 节点接口
/// </summary>
public interface INode { }

/// <summary>协议驱动接口</summary>
public interface IDriver
{
    #region 方法
    /// <summary>
    /// 打开设备驱动，传入参数。一个设备可能有多个通道共用，需要以节点来区分
    /// </summary>
    /// <param name="channel">通道</param>
    /// <param name="parameters">参数</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    INode Open(IChannel channel, IDictionary<String, Object> parameters);

    /// <summary>
    /// 关闭设备驱动
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

    #region 日志
    /// <summary>日志</summary>
    ILog Log { get; set; }

    /// <summary>性能追踪器</summary>
    ITracer Tracer { get; set; }

    /// <summary>写日志</summary>
    /// <param name="format"></param>
    /// <param name="args"></param>
    void WriteLog(String format, params Object[] args) => Log?.Info(format, args);
    #endregion
}