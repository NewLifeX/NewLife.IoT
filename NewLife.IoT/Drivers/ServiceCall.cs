using NewLife.Data;

namespace NewLife.IoT.Drivers;

/// <summary>服务调用请求。用于 ControlAsync 发起结构化服务调用</summary>
/// <remarks>
/// 替代裸字典参数，提供服务名、参数和扩展元数据三层结构，让服务调用语义更清晰。
/// <para>与 <see cref="NewLife.IoT.Models.ServiceRequest"/> 的区别：Models 版用于平台层下发（含 DeviceId、Timeout 等调度字段），
/// 本类用于驱动层执行时的参数传递。</para>
/// <example>
/// <code>
/// var req = ServiceCall.Create("Calibrate", new Dictionary&lt;String, Object?&gt;
/// {
///     ["Zero"] = 0.0,
///     ["Span"] = 100.0
/// });
/// var result = await driver.ControlAsync(node, req);
/// </code>
/// </example>
/// </remarks>
public class ServiceCall : IExtend
{
    #region 属性
    /// <summary>服务名称</summary>
    public String ServiceName { get; set; } = String.Empty;

    /// <summary>服务输入参数字典</summary>
    public IDictionary<String, Object?> Parameters { get; set; } = new Dictionary<String, Object?>();
    #endregion

    #region IExtend
    private Dictionary<String, Object?>? _items;

    /// <summary>扩展数据项字典。可存储调用超时、优先级等控制元数据</summary>
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
    /// <summary>快速创建服务调用请求</summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="parameters">输入参数字典，可为空</param>
    /// <returns>服务调用请求</returns>
    public static ServiceCall Create(String serviceName, IDictionary<String, Object?>? parameters = null)
        => new() { ServiceName = serviceName, Parameters = parameters ?? new Dictionary<String, Object?>() };
    #endregion

    /// <summary>以文本方式输出请求摘要</summary>
    public override String ToString() => $"ServiceCall({ServiceName}, {Parameters.Count} params)";
}
