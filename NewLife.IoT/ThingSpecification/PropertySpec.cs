namespace NewLife.IoT.ThingSpecification;

/// <summary>
/// 属性规范
/// </summary>
public class PropertySpec : SpecBase
{
    #region 属性
    /// <summary>
    /// 访问模式
    /// </summary>
    public String AccessMode { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    public TypeSpec DataType { get; set; }

    /// <summary>
    /// 采集点位置信息
    /// </summary>
    public String Address { get; set; }
    #endregion

    /// <summary>
    /// 已重载。友好显示
    /// </summary>
    /// <returns></returns>
    public override String ToString() => $"{Id} {Name} {DataType}";
}