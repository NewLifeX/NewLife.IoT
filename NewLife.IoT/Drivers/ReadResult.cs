using NewLife.Data;
using NewLife.IoT.ThingModels;

namespace NewLife.IoT.Drivers;

/// <summary>读取操作结果。携带点位数值、数据质量、时间戳及诊断信息</summary>
/// <remarks>
/// ReadResult 统一表达读取成功和失败两种状态，替代裸字典返回，内置质量码和诊断支持。
/// 成功时 <see cref="IsSuccess"/>=true，<see cref="Points"/> 与 <see cref="Values"/> 平行数组携带点位数据；
/// 失败时 <see cref="IsSuccess"/>=false，<see cref="Code"/>/<see cref="Message"/> 说明原因。
/// <see cref="GetValue"/> 提供按名称查找的便利方法。
/// <example>
/// <code>
/// var result = await driver.ReadAsync(node, points);
/// if (result.IsSuccess)
/// {
///     for (var i = 0; i &lt; result.Points.Length; i++)
///         Console.WriteLine($"{result.Points[i].Name} = {result.Values[i]}");
///     // 或按名称查找
///     var temp = result.GetValue("Temperature");
/// }
/// else
/// {
///     Log.Warn("读取失败：[{0}] {1}", result.Code, result.Message);
/// }
/// </code>
/// </example>
/// </remarks>
public class ReadResult : IExtend, IDriverResult
{
    #region 属性
    /// <summary>操作是否成功</summary>
    public Boolean IsSuccess { get; init; }

    /// <summary>错误码。仅 IsSuccess=false 时有意义</summary>
    public IoTErrorCode Code { get; init; }

    /// <summary>错误消息。仅 IsSuccess=false 时有意义</summary>
    public String? Message { get; init; }

    /// <summary>数据采集时刻（UTC）</summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>数据质量码</summary>
    public DataQuality Quality { get; init; } = DataQuality.Good;

    /// <summary>点位集合，与 Values 下标一一对应。IsSuccess=true 时有效</summary>
    public IPoint[] Points { get; init; } = [];

    /// <summary>点位数值数组，与 Points 下标一一对应。IsSuccess=true 时有效</summary>
    public Object?[] Values { get; init; } = [];

    /// <summary>原始请求帧字节。仅 IDriver.Diagnostics=true 时填充</summary>
    public Byte[]? RequestBytes { get; init; }

    /// <summary>原始响应帧字节。仅 IDriver.Diagnostics=true 时填充</summary>
    public Byte[]? ResponseBytes { get; init; }
    #endregion

    #region IExtend
    private Dictionary<String, Object?>? _items;

    /// <summary>扩展数据项字典。可存储驱动自定义元数据，如信号强度、帧序号等</summary>
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

    #region 方法
    /// <summary>按点位名称或地址查找对应值</summary>
    /// <param name="pointName">点位名称或地址</param>
    /// <returns>点位值，未找到时返回 null</returns>
    public Object? GetValue(String pointName)
    {
        for (var i = 0; i < Points.Length; i++)
        {
            if (pointName.EqualIgnoreCase(Points[i].Name, Points[i].Address))
                return Values[i];
        }
        return null;
    }
    #endregion

    #region 工厂方法
    /// <summary>创建成功结果</summary>
    /// <param name="points">点位集合</param>
    /// <param name="values">点位数值数组，与 points 下标对应</param>
    /// <param name="quality">数据质量码。默认 Good</param>
    /// <param name="requestBytes">原始请求帧（诊断用）</param>
    /// <param name="responseBytes">原始响应帧（诊断用）</param>
    /// <returns>成功的读取结果</returns>
    public static ReadResult Success(IPoint[] points, Object?[] values,
        DataQuality quality = DataQuality.Good,
        Byte[]? requestBytes = null, Byte[]? responseBytes = null)
        => new()
        {
            IsSuccess = true,
            Quality = quality,
            Points = points,
            Values = values,
            RequestBytes = requestBytes,
            ResponseBytes = responseBytes,
        };

    /// <summary>创建失败结果</summary>
    /// <param name="code">错误码</param>
    /// <param name="message">错误消息</param>
    /// <param name="quality">数据质量码。默认 Bad</param>
    /// <returns>失败的读取结果</returns>
    public static ReadResult Fail(IoTErrorCode code, String message, DataQuality quality = DataQuality.Bad)
        => new()
        {
            Code = code,
            Message = message,
            Quality = quality,
        };
    #endregion

    /// <summary>以文本方式输出结果摘要</summary>
    public override String ToString()
        => IsSuccess
            ? $"ReadResult.Success({Points.Length} points, {Quality})"
            : $"ReadResult.Fail({Code}: {Message})";
}
