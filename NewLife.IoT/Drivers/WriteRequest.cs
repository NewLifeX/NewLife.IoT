using NewLife.Data;
using NewLife.IoT.ThingModels;

namespace NewLife.IoT.Drivers;

/// <summary>写入请求。包装目标点位、要写入的值及扩展元数据</summary>
/// <remarks>
/// 通过构造函数或工厂方法快速包装：
/// <example>
/// <code>
/// // 含点位的写入（推荐）
/// await driver.WriteAsync(node, new WriteRequest(point, 42));
///
/// // 携带扩展元数据的写入
/// var req = new WriteRequest(point, 42);
/// req["QoS"] = 1;
/// await driver.WriteAsync(node, req);
/// </code>
/// </example>
/// </remarks>
public class WriteRequest : IExtend
{
    #region 属性
    /// <summary>目标点位</summary>
    public IPoint? Point { get; set; }

    /// <summary>要写入的值</summary>
    public Object? Value { get; set; }
    #endregion

    #region 构造
    /// <summary>创建空写入请求</summary>
    public WriteRequest() { }

    /// <summary>创建携带值的写入请求（不含点位）</summary>
    /// <param name="value">要写入的值</param>
    public WriteRequest(Object? value) { Value = value; }

    /// <summary>创建携带点位和值的写入请求</summary>
    /// <param name="point">目标点位</param>
    /// <param name="value">要写入的值</param>
    public WriteRequest(IPoint point, Object? value) { Point = point; Value = value; }
    #endregion

    #region IExtend
    private Dictionary<String, Object?>? _items;

    /// <summary>扩展数据项字典。可存储 QoS、超时覆盖、校验模式等控制元数据</summary>
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
    /// <summary>从裸值快速创建写入请求（不含点位）</summary>
    /// <param name="value">要写入的值</param>
    /// <returns>写入请求</returns>
    public static WriteRequest From(Object? value) => new(value);

    /// <summary>从点位和值快速创建写入请求</summary>
    /// <param name="point">目标点位</param>
    /// <param name="value">要写入的值</param>
    /// <returns>写入请求</returns>
    public static WriteRequest From(IPoint point, Object? value) => new(point, value);
    #endregion

    /// <summary>以文本方式输出请求摘要</summary>
    public override String ToString() => $"WriteRequest({Point?.Name ?? "?"}, {Value})";
}
