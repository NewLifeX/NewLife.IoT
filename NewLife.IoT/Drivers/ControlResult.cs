using NewLife.Data;

namespace NewLife.IoT.Drivers;

/// <summary>控制结果。携带控制执行状态和输出参数</summary>
/// <remarks>
/// 成功时 <see cref="IsSuccess"/>=true，<see cref="OutputParameters"/> 携带服务输出；
/// 失败时 <see cref="IsSuccess"/>=false，<see cref="Code"/>/<see cref="Message"/> 说明原因。
/// </remarks>
public class ControlResult : IExtend, IDriverResult
{
    #region 属性
    /// <summary>操作是否成功</summary>
    public Boolean IsSuccess { get; init; }

    /// <summary>错误码。仅 IsSuccess=false 时有意义</summary>
    public IoTErrorCode Code { get; init; }

    /// <summary>错误消息。仅 IsSuccess=false 时有意义</summary>
    public String? Message { get; init; }

    /// <summary>服务输出参数字典。IsSuccess=true 时有效</summary>
    public IDictionary<String, Object?> OutputParameters { get; init; } = new Dictionary<String, Object?>();
    #endregion

    #region IExtend
    private Dictionary<String, Object?>? _items;

    /// <summary>扩展数据项字典。可存储执行耗时、重试次数等元信息</summary>
    public IDictionary<String, Object?> Items => _items ??= [];

    /// <summary>获取或设置扩展数据项</summary>
    /// <param name="key">键</param>
    /// <returns>扩展数据项值</returns>
    public Object? this[String key]
    {
        get => _items != null && _items.TryGetValue(key, out var obj) ? obj : null;
        set => Items[key] = value;
    }
    #endregion

    #region 工厂方法
    /// <summary>创建成功结果</summary>
    /// <param name="outputParameters">输出参数字典，可为空</param>
    /// <returns>成功的控制结果</returns>
    public static ControlResult Success(IDictionary<String, Object?>? outputParameters = null)
        => new() { IsSuccess = true, OutputParameters = outputParameters ?? new Dictionary<String, Object?>() };

    /// <summary>创建失败结果</summary>
    /// <param name="code">错误码</param>
    /// <param name="message">错误消息</param>
    /// <returns>失败的控制结果</returns>
    public static ControlResult Fail(IoTErrorCode code, String message)
        => new() { Code = code, Message = message };
    #endregion

    /// <summary>以文本方式输出结果摘要</summary>
    public override String ToString()
        => IsSuccess
            ? $"ControlResult.Success({OutputParameters.Count} outputs)"
            : $"ControlResult.Fail({Code}: {Message})";
}
