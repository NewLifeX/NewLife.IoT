namespace NewLife.IoT.Drivers;

/// <summary>写入操作结果。同时承载单点写入和批量写入的结果信息</summary>
/// <remarks>
/// <see cref="AffectedCount"/> 在单点写入时为 1（成功）或 0（失败），批量写入时为实际写入成功的点数。
/// <see cref="EchoValue"/> 携带部分协议在写入确认时返回的回显值，不支持回显的驱动保持 null。
/// </remarks>
public readonly struct WriteResult : IDriverResult
{
    #region 属性
    /// <summary>操作是否成功</summary>
    public Boolean IsSuccess { get; init; }

    /// <summary>错误码。仅 IsSuccess=false 时有意义</summary>
    public IoTErrorCode Code { get; init; }

    /// <summary>错误消息。仅 IsSuccess=false 时有意义</summary>
    public String? Message { get; init; }

    /// <summary>成功写入的点数。单点写入时为 1（成功）或 0（失败）；批量写入时为实际成功数</summary>
    public Int32 AffectedCount { get; init; }

    /// <summary>设备回显值。部分协议在写入后会返回确认值，不支持时为 null</summary>
    public Object? EchoValue { get; init; }

    /// <summary>原始请求帧字节。仅 IDriver.Diagnostics=true 时填充</summary>
    public Byte[]? RequestBytes { get; init; }

    /// <summary>原始响应帧字节。仅 IDriver.Diagnostics=true 时填充</summary>
    public Byte[]? ResponseBytes { get; init; }
    #endregion

    #region 工厂方法
    /// <summary>创建单点写入成功结果</summary>
    /// <param name="echoValue">设备回显值</param>
    /// <param name="requestBytes">原始请求帧（诊断用）</param>
    /// <param name="responseBytes">原始响应帧（诊断用）</param>
    /// <returns>单点写入成功结果</returns>
    public static WriteResult Success(Object? echoValue = null, Byte[]? requestBytes = null, Byte[]? responseBytes = null)
        => new() { IsSuccess = true, AffectedCount = 1, EchoValue = echoValue, RequestBytes = requestBytes, ResponseBytes = responseBytes };

    /// <summary>创建批量写入成功结果</summary>
    /// <param name="count">成功写入的点数</param>
    /// <returns>批量写入成功结果</returns>
    public static WriteResult SuccessBatch(Int32 count)
        => new() { IsSuccess = true, AffectedCount = count };

    /// <summary>创建失败结果</summary>
    /// <param name="code">错误码</param>
    /// <param name="message">错误消息</param>
    /// <returns>失败结果</returns>
    public static WriteResult Fail(IoTErrorCode code, String message)
        => new() { Code = code, Message = message };
    #endregion

    /// <summary>以文本方式输出结果摘要</summary>
    public override String ToString()
        => IsSuccess
            ? $"WriteResult.Success(count={AffectedCount}, echo={EchoValue})"
            : $"WriteResult.Fail({Code}: {Message})";
}
