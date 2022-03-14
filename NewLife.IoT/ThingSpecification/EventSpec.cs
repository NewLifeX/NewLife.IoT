using System.Runtime.Serialization;

namespace NewLife.IoT.ThingSpecification;

/// <summary>
/// 事件规范
/// </summary>
public class EventSpec : SpecBase
{
    #region 属性
    /// <summary>
    /// 类型。info/warning/error
    /// </summary>
    public String Type { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [DataMember(Name="desc")]
    public String Description { get; set; }

    /// <summary>
    /// 方法
    /// </summary>
    public String Method { get; set; }

    /// <summary>
    /// 输出
    /// </summary>
    public PropertySpec[] OutputData { get; set; }
    #endregion
}