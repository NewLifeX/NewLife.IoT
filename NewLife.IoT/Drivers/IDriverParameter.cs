using NewLife.Serialization;
using NewLife.Xml;

namespace NewLife.IoT.Drivers;

/// <summary>
/// 驱动参数接口。控制设备驱动的参数，可序列化为Xml便于传输与存储
/// </summary>
public interface IDriverParameter
{
}

/// <summary>具有唯一标识的驱动参数</summary>
/// <remarks>
/// 相同驱动下，相同的唯一标识共用驱动对象。
/// 例如多个设备共用一个串口，或者多个设备共用一个ModbusTcp地址。
/// </remarks>
public interface IDriverParameterKey
{
    /// <summary>获取驱动参数的唯一标识</summary>
    /// <remarks>
    /// 相同驱动下，相同的唯一标识共用驱动对象。
    /// 例如多个设备共用一个串口，或者多个设备共用一个ModbusTcp地址。
    /// </remarks>
    String GetKey();
}

/// <summary>默认驱动参数实现</summary>
public class DriverParameter : IDriverParameter { }

/// <summary>
/// 驱动参数扩展
/// </summary>
public static class DriverParameterExtensions
{
    /// <summary>获取驱动参数的唯一标识</summary>
    /// <remarks>
    /// 相同驱动下，相同的唯一标识共用驱动对象。
    /// 例如多个设备共用一个串口，或者多个设备共用一个ModbusTcp地址。
    /// </remarks>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static String GetKey(this IDriverParameter parameter)
    {
        if (parameter is IDriverParameterKey dk) return dk.GetKey();

        var dic = parameter.ToDictionary();
        if (dic.TryGetValue("Address", out var str)) return str + "";
        if (dic.TryGetValue("Server", out str)) return str + "";
        if (dic.TryGetValue("PortName", out str)) return str + "";
        if (dic.Count > 0) return dic.FirstOrDefault().Value + "";

        return parameter + "";
    }

    /// <summary>序列化参数对象为Xml</summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static String EncodeParameter(this IDriverParameter parameter) => parameter.ToXml(null, true).Trim((Char)0xFEFF);

    /// <summary>序列化参数字典为Xml</summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static String EncodeParameter(this IDictionary<String, Object> parameter) => parameter.ToXml(null, true).Trim((Char)0xFEFF);

    /// <summary>从Xml/Json反序列化为字典</summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static IDictionary<String, Object?>? DecodeParameter(this String parameter)
    {
        parameter = parameter.Trim((Char)0xFEFF);
        if (parameter.IsNullOrEmpty()) throw new ArgumentNullException(nameof(parameter));

        // 按Xml或Json解析参数成为字典
        var ps = parameter.StartsWith("<") && parameter.EndsWith(">") ?
            XmlParser.Decode(parameter) :
            JsonParser.Decode(parameter);

        return ps;
    }
}