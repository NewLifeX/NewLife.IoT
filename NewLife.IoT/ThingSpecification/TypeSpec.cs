namespace NewLife.IoT.ThingSpecification;

/// <summary>
/// 类型规范
/// </summary>
public class TypeSpec
{
    /// <summary>
    /// 类型。float/text
    /// </summary>
    public String Type { get; set; }

    /// <summary>
    /// 数据规范
    /// </summary>
    public DataSpecs Specs { get; set; }

    /// <summary>
    /// 已重载。友好显示
    /// </summary>
    /// <returns></returns>
    public override String ToString() => Type;
}