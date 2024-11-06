using System.Runtime.Serialization;
using System.Xml.Serialization;
using NewLife.Collections;
using NewLife.IoT.ThingModels;
#if NETCOREAPP
using System.Text.Json.Serialization;
#endif

namespace NewLife.IoT.ThingSpecification;

/// <summary>属性扩展</summary>
/// <remarks>
/// 大部分字段仅适用于数字型属性，用于数据解析
/// </remarks>
public class PropertyExtend : IDictionarySource
{
    #region 属性
    /// <summary>唯一标识</summary>
#if NETCOREAPP
    [JsonPropertyName("identifier")]
#endif
    [DataMember(Name = "identifier")]
    public String Id { get; set; } = null!;

    /// <summary>采集点位置信息。常规地址6，Modbus地址 4x0023，位域地址D12.05，虚拟点位地址#</summary>
    public String? Address { get; set; }

    ///// <summary>交换16。字节交换，12转21，默认false大端</summary>
    //public Boolean Swap16 { get; set; }

    ///// <summary>交换32。字节交换，1234转3412，默认false大端</summary>
    //public Boolean Swap32 { get; set; }

    /// <summary>字节序</summary>
    public ByteOrder Order { get; set; }

    /// <summary>缩放因子。不能是0，默认1，n*scaling+constant</summary>
    public Single Scaling { get; set; }

    /// <summary>常量因子。默认0，n*scaling+constant</summary>
    public Single Constant { get; set; }

    /// <summary>读取规则。数据解析规则，表达式或脚本</summary>
    public String? ReadRule { get; set; }

    /// <summary>写入规则。数据反解析规则，表达式或脚本</summary>
    public String? WriteRule { get; set; }

    /// <summary>事件模式。在客户端或服务端生成属性变更事件</summary>
    public EventModes EventMode { get; set; }

    /// <summary>属性分组</summary>
    public String? Subset { get; set; }
    #endregion

    #region 扩展属性
    /// <summary>交换16。字节交换，12转21</summary>
    [XmlIgnore, IgnoreDataMember]
    public Boolean Swap16 => Endian == EndianType.LittleEndian || Endian == EndianType.BigSwap;

    /// <summary>交换32。字节交换，1234转3412</summary>
    [XmlIgnore, IgnoreDataMember]
    public Boolean Swap32 => Endian == EndianType.LittleEndian || Endian == EndianType.LittleSwap;

    /// <summary>字节序。另一种表达形式</summary>
    [XmlIgnore, IgnoreDataMember]
    public EndianType Endian { get => (EndianType)Order; set => Order = (ByteOrder)value; }
    #endregion

    #region 方法
    /// <summary>转字典。根据不同类型，提供不一样的序列化能力</summary>
    /// <returns></returns>
    public IDictionary<String, Object?> ToDictionary()
    {
        var dic = CollectionHelper.ToDictionary(this);

        //return dic.Where(e => e.Value != null).ToDictionary(e => e.Key, e => e.Value);

        var rs = new Dictionary<String, Object?>();
        foreach (var item in dic)
        {
            if (item.Value == null) continue;
            if (item.Value is Single f && f == 0) continue;
            if (item.Value is Boolean b && b == false) continue;
            if (item.Value is Int32 n && n == 0) continue;
            if (item.Value is Enum && item.Value.ToInt() == 0) continue;

            rs.Add(item.Key, item.Value);
        }

        return rs;
    }

    /// <summary>
    /// 已重载。友好显示
    /// </summary>
    /// <returns></returns>
    public override String ToString() => $"{Id} {Address}";
    #endregion
}