using System.Runtime.Serialization;

namespace NewLife.IoT.ThingSpecification;

/// <summary>
/// 服务规范
/// </summary>
public class ServiceSpec : SpecBase
{
    #region 属性
    /// <summary>
    /// 调用类型。sync/async
    /// </summary>
    [DataMember(Name = "callType")]
    public String Type { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [DataMember(Name = "desc")]
    public String Description { get; set; }

    /// <summary>
    /// 方法
    /// </summary>
    public String Method { get; set; }

    /// <summary>
    /// 输入
    /// </summary>
    public PropertySpec[] InputData { get; set; }

    /// <summary>
    /// 输出
    /// </summary>
    public PropertySpec[] OutputData { get; set; }
    #endregion
}