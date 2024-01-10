using NewLife.Collections;

namespace NewLife.IoT.ThingSpecification;

/// <summary>
/// 类型规范
/// </summary>
public class TypeSpec : IDictionarySource
{
    /// <summary>
    /// 类型。int/float/text
    /// </summary>
    public String? Type { get; set; }

    /// <summary>
    /// 数据规范
    /// </summary>
    public DataSpecs? Specs { get; set; }

    /// <summary>转字典。根据不同类型，提供不一样的序列化能力</summary>
    /// <returns></returns>
    public IDictionary<String, Object?> ToDictionary()
    {
        if (Type.IsNullOrEmpty()) return new Dictionary<String, Object?>();

        var ds = Specs?.GetDictionary(Type);

        var dic = new Dictionary<String, Object?>
        {
            { nameof(Type), Type }
        };

        if (ds != null)
            dic.Add(nameof(Specs), ds);

        return dic;
    }

    /// <summary>
    /// 已重载。友好显示
    /// </summary>
    /// <returns></returns>
    public override String? ToString() => Type;
}