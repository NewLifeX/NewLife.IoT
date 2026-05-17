namespace NewLife.IoT.Drivers;

/// <summary>IoT操作错误码。用于区分不同类型的运行时失败，便于分类处理</summary>
/// <remarks>
/// 错误码按段分类：
///   1000段：配置/编程错误，属于不应发生的错误，使用IoTException抛出，不被TryReadAsync捕获
///   2000段：运行时通信错误，工业现场属于频繁预期失败，推荐使用DriverResult而非异常
///   3000段：设备状态，属于正常业务状态，推荐使用DriverResult
/// </remarks>
public enum IoTErrorCode
{
    /// <summary>无错误</summary>
    None = 0,

    #region 1000段：配置/编程错误（应抛出，不被TryRead捕获）
    /// <summary>参数无效。驱动参数格式错误、必填项为空等</summary>
    InvalidParameter = 1001,

    /// <summary>驱动未找到。DriverFactory中未注册对应名称的驱动</summary>
    DriverNotFound = 1002,

    /// <summary>操作不支持。驱动未实现该功能</summary>
    NotSupported = 1003,
    #endregion

    #region 2000段：运行时通信错误（频繁发生，推荐DriverResult）
    /// <summary>连接失败。重试后仍无法建立通信连接</summary>
    ConnectionFailed = 2001,

    /// <summary>操作超时。在规定时间内未收到设备响应</summary>
    Timeout = 2002,

    /// <summary>设备忙。设备当前正在处理其他请求，拒绝本次请求</summary>
    DeviceBusy = 2003,

    /// <summary>校验和错误。响应数据CRC/LRC/XOR校验失败，数据可能受到干扰</summary>
    ChecksumError = 2004,

    /// <summary>协议解析错误。响应数据格式不符合协议规范</summary>
    ProtocolError = 2005,

    /// <summary>读取错误。设备返回错误响应码</summary>
    ReadError = 2006,

    /// <summary>写入错误。设备返回写入失败响应码</summary>
    WriteError = 2007,
    #endregion

    #region 3000段：设备状态（业务状态，推荐DriverResult）
    /// <summary>设备离线。设备当前不可达或已断开</summary>
    DeviceOffline = 3001,

    /// <summary>地址无效。指定的点位地址在设备上不存在</summary>
    InvalidAddress = 3002,
    #endregion
}

/// <summary>IoT专用异常。携带结构化错误码，便于调用方按类型处理</summary>
/// <remarks>
/// 错误码为1000段的异常（配置/编程错误）应直接抛出，调用方不应捕获。
/// 错误码为2000段和3000段的运行时失败，在高频采集场景推荐使用DriverExtensions.TryReadAsync
/// 以DriverResult形式返回，避免异常对象分配带来的GC压力。
/// </remarks>
public class IoTException : Exception
{
    /// <summary>错误码</summary>
    public IoTErrorCode Code { get; }

    /// <summary>使用错误码和消息构造IoTException</summary>
    /// <param name="code">错误码</param>
    /// <param name="message">错误消息</param>
    public IoTException(IoTErrorCode code, String message) : base(message)
        => Code = code;

    /// <summary>使用错误码、消息和内部异常构造IoTException</summary>
    /// <param name="code">错误码</param>
    /// <param name="message">错误消息</param>
    /// <param name="innerException">内部异常</param>
    public IoTException(IoTErrorCode code, String message, Exception innerException) : base(message, innerException)
        => Code = code;
}
