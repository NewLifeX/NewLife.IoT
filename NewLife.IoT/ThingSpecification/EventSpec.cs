using System.Runtime.Serialization;
#if NETCOREAPP
using System.Text.Json.Serialization;
#endif

namespace NewLife.IoT.ThingSpecification;

/// <summary>事件规范</summary>
/// <remarks>
/// 设备运行时，主动上报给云端的信息，一般包含需要被外部感知和处理的信息、告警和故障。事件中可包含多个输出参数。
/// 例如，某项任务完成后的通知信息；设备发生故障时的温度、时间信息；设备告警时的运行状态等。
/// 事件可以被订阅和推送。
/// </remarks>
public class EventSpec : SpecBase
{
    #region 属性
    /// <summary>
    /// 类型。info/warning/error
    /// </summary>
    public String? Type { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
#if NETCOREAPP
    [JsonPropertyName("desc")]
#endif
    [DataMember(Name = "desc")]
    public String? Description { get; set; }

    /// <summary>
    /// 方法
    /// </summary>
    public String? Method { get; set; }

    /// <summary>
    /// 输出
    /// </summary>
    public PropertySpec[]? OutputData { get; set; }
    #endregion
}