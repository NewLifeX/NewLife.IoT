using NewLife.IoT.Models;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using NewLife.Log;

namespace NewLife.IoT;

/// <summary>逻辑设备接口。具备物模型全部能力，对接物理设备进行数据采集及远程控制</summary>
/// <remarks>
/// IDevice 接口乃是IoT客户端核心，它的不同程度实现，可以建立轻量级或重量级物联网平台。
/// </remarks>
public interface IDevice
{
    #region 属性
    /// <summary>设备编码。在平台中唯一标识设备</summary>
    public String Code { get; set; }

    /// <summary>属性集合</summary>
    IDictionary<String, Object> Properties { get; }

    /// <summary>产品物模型。自定义客户端或驱动插件内部，可借助物模型去规范所需要采集的数据</summary>
    ThingSpec Specification { get; set; }

    /// <summary>点位集合</summary>
    IPoint[] Points { get; set; }

    /// <summary>服务集合</summary>
    IDictionary<String, Delegate> Services { get; }
    #endregion

    #region 方法
    /// <summary>开始</summary>
    Task Start();

    /// <summary>停止</summary>
    void Stop();
    #endregion

    #region 设备方法
    /// <summary>添加子设备。有些设备驱动具备扫描发现子设备能力，通过该方法上报设备</summary>
    /// <param name="devices"></param>
    void AddDevice(IDeviceInfo[] devices);
    #endregion

    #region 物模型方法
    /// <summary>马上上报属性</summary>
    void PostProperty();

    /// <summary>设置属性</summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void SetProperty(String name, Object value);

    /// <summary>添加自定义数据，批量上传</summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    Boolean AddData(String name, String value);

    /// <summary>写事件</summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="remark"></param>
    Boolean WriteEvent(String type, String name, String remark);

    ///// <summary>
    ///// 获取影子
    ///// </summary>
    ///// <remarks>每次获取上一次值，并异步更新</remarks>
    //String GetShadow();

    ///// <summary>
    ///// 设置影子，马上上报
    ///// </summary>
    ///// <param name="shadow"></param>
    //void SetShadow(Object shadow);

    ///// <summary>
    ///// 获取配置
    ///// </summary>
    ///// <remarks>首次获取时启动定时器，确保后续读到新数据</remarks>
    ///// <param name="name"></param>
    ///// <returns></returns>
    //Object GetConfig(String name);
    #endregion

    #region 服务控制
    /// <summary>
    /// 注册服务。收到平台下发的服务调用时，执行注册的方法
    /// </summary>
    /// <param name="service"></param>
    /// <param name="method"></param>
    void RegisterService(String service, Delegate method);
    #endregion

    #region 日志
    /// <summary>链路追踪</summary>
    ITracer Tracer { get; set; }

    /// <summary>日志</summary>
    ILog Log { get; set; }
    #endregion
}

/// <summary>物模型扩展</summary>
public static class ThingExtensions
{
    /// <summary>写信息事件</summary>
    /// <param name="device"></param>
    /// <param name="name"></param>
    /// <param name="remark"></param>
    public static void WriteInfoEvent(this IDevice device, String name, String remark) => device.WriteEvent("info", name, remark);

    /// <summary>写警告事件</summary>
    /// <param name="device"></param>
    /// <param name="name"></param>
    /// <param name="remark"></param>
    public static void WriteAlertEvent(this IDevice device, String name, String remark) => device.WriteEvent("alert", name, remark);

    /// <summary>写错误事件</summary>
    /// <param name="device"></param>
    /// <param name="name"></param>
    /// <param name="remark"></param>
    public static void WriteErrorEvent(this IDevice device, String name, String remark) => device.WriteEvent("error", name, remark);

    ///// <summary>写日志</summary>
    ///// <param name="device"></param>
    ///// <param name="format"></param>
    ///// <param name="args"></param>
    //public static void WriteLog(this IThing device, String format, params Object[] args) => device.Log?.Info(format, args);
}