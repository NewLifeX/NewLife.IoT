using System.Runtime.Serialization;
#if NETCOREAPP
using System.Text.Json.Serialization;
#endif

namespace NewLife.IoT.ThingSpecification;

/// <summary>
/// 物模型基础属性
/// </summary>
public abstract class SpecBase
{
    #region 属性
    /// <summary>
    /// 唯一标识
    /// </summary>
#if NETCOREAPP
    [JsonPropertyName("identifier")]
#endif
    [DataMember(Name = "identifier")]
    public String Id { get; set; } = null!;

    /// <summary>
    /// 名称
    /// </summary>
    public String? Name { get; set; }

    /// <summary>
    /// 是否必须
    /// </summary>
    public Boolean Required { get; set; }
    #endregion

    /// <summary>
    /// 已重载。友好显示
    /// </summary>
    /// <returns></returns>
    public override String ToString() => $"{Id} {Name}";
}