using NewLife.Collections;

namespace NewLife.IoT.ThingSpecification;

/// <summary>
/// 属性规范
/// </summary>
public class PropertySpec : SpecBase, IDictionarySource
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

    /// <summary>转字典。根据不同类型，提供不一样的序列化能力</summary>
    /// <returns></returns>
    public IDictionary<String, Object> ToDictionary()
    {
        var dic = new Dictionary<String, Object>();
        if (!Id.IsNullOrEmpty())
            dic.Add("identifier", Id);
        if (!Name.IsNullOrEmpty())
            dic.Add(nameof(Name), Name);
        if (Required)
            dic.Add(nameof(Required), Required);

        if (!AccessMode.IsNullOrEmpty())
            dic.Add(nameof(AccessMode), AccessMode);

        var dt = DataType?.ToDictionary();
        if (dt != null)
            dic.Add(nameof(DataType), dt);

        if (!Address.IsNullOrEmpty())
            dic.Add(nameof(Address), Address);

        return dic;
    }

    /// <summary>
    /// 已重载。友好显示
    /// </summary>
    /// <returns></returns>
    public override String ToString() => $"{Id} {Name} {DataType}";
}