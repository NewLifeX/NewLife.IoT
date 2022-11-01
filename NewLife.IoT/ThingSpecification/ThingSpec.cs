using NewLife.Serialization;

namespace NewLife.IoT.ThingSpecification;

/// <summary>
/// 物模型规范
/// </summary>
public class ThingSpec
{
    #region 属性
    /// <summary>
    /// 模式
    /// </summary>
    public String Schema { get; set; } = "http://iot.feifan.link/schema.json";

    /// <summary>
    /// 简介
    /// </summary>
    public Profile Profile { get; set; }

    /// <summary>
    /// 属性
    /// </summary>
    public PropertySpec[] Properties { get; set; }

    /// <summary>
    /// 事件
    /// </summary>
    public EventSpec[] Events { get; set; }

    /// <summary>
    /// 服务
    /// </summary>
    public ServiceSpec[] Services { get; set; }
    #endregion

    #region 方法
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
            Indented = true,
            IndentedLength = 2,
            CamelCase = true,
        };
        jw.Write(dic);

        return jw.GetString();
    }
    #endregion
}