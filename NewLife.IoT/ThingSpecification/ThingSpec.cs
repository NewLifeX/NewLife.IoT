using NewLife.Serialization;
using System.Linq;

namespace NewLife.IoT.ThingSpecification;

/// <summary>物模型规范</summary>
/// <remarks>
/// 物联网平台通过定义一种物的描述语言来描述物模型模块和功能，称为TSL（Thing Specification Language）
/// </remarks>
public class ThingSpec
{
    #region 属性
    /// <summary>模式</summary>
    public String Schema { get; set; } = "http://iot.feifan.link/schema.json";

    /// <summary>简介</summary>
    public Profile? Profile { get; set; }

    /// <summary>属性</summary>
    public PropertySpec[]? Properties { get; set; }

    /// <summary>事件</summary>
    public EventSpec[]? Events { get; set; }

    /// <summary>服务</summary>
    public ServiceSpec[]? Services { get; set; }

    ///// <summary>属性扩展</summary>
    //public PropertyExtend[]? ExtendedProperties { get; set; }
    #endregion

    #region 方法
    /// <summary>快速创建属性</summary>
    /// <param name="id">标识</param>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    /// <param name="length">长度</param>
    /// <param name="address">点位地址</param>
    /// <returns></returns>
    public PropertySpec AddProperty(String id, String name, String type, Int32 length = 0, String? address = null)
    {
        {
            var ps = PropertySpec.Create(id, name, type, length, address);

            var list = new List<PropertySpec>();
            if (Properties != null) list.AddRange(Properties);
            list.Add(ps);
            Properties = list.ToArray();

            return ps;
        }

        //if (address != null)
        //{
        //    var pt = new PropertyExtend { Id = id, Address = address };

        //    var list = new List<PropertyExtend>();
        //    if (ExtendedProperties != null) list.AddRange(ExtendedProperties);
        //    list.Add(pt);
        //    ExtendedProperties = list.ToArray();
        //}
    }

    /// <summary>获取属性</summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public PropertySpec? GetProperty(String? id)
    {
        if (id.IsNullOrEmpty()) return null;

        return Properties?.FirstOrDefault(e => e.Id.EqualIgnoreCase(id));
    }

    /// <summary>
    /// 解析Json串
    /// </summary>
    /// <param name="json"></param>
    public void FromJson(String json)
    {
        var dic = JsonParser.Decode(json);

        var jr = new JsonReader();
        jr.ToObject(dic, null, this);
    }

    /// <summary>
    /// 转为Json串
    /// </summary>
    /// <returns></returns>
    public String ToJson()
    {
        var dic = this.ToDictionary();

        var jw = new JsonWriter
        {
            //Indented = true,
            IndentedLength = 2,
            //CamelCase = true,
            EnumString = true,
        };
        jw.Options.WriteIndented = true;
        jw.Options.CamelCase = true;
        jw.Write(dic);

        return jw.GetString();
    }
    #endregion
}