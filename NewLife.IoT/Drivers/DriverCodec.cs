using System.Text;
using NewLife.Data;
using NewLife.Serialization;

namespace NewLife.IoT.Drivers;

/// <summary>驱动数据编解码助手。用于通用驱动的原始请求字节编码与响应解码</summary>
/// <remarks>
/// IoTSerialDriver、IoTSocketDriver 等通用驱动共用此助手，避免重复实现编解码逻辑。
/// 编码支持十六进制(0x开头)/字符串/字节数组/数据包；解码支持 HEX/ASCII/UTF8/Json。
/// <example>
/// <code>
/// var pk = DriverCodec.Encode("0x010300000002C40B");
/// var data = DriverCodec.Decode(pk, "HEX");
/// </code>
/// </example>
/// </remarks>
public static class DriverCodec
{
    /// <summary>编码请求数据为数据包</summary>
    /// <param name="value">原始数据。支持数据包/字符串/字节数组，字符串以0x开头时按十六进制解析</param>
    /// <returns>数据包，空值返回 null</returns>
    /// <exception cref="NotSupportedException">不支持的数据类型</exception>
    public static IPacket? Encode(Object? value)
    {
        if (value == null) return null;

        switch (value)
        {
            case IPacket pk:
                return pk;
            case String str:
                if (str.StartsWithIgnoreCase("0x"))
                    return new ArrayPacket(str[2..].ToHex());
                else
                    return new ArrayPacket(str.GetBytes());
            case Byte[] bytes:
                return new ArrayPacket(bytes);
            default:
                throw new NotSupportedException(L.T("Unsupported data type {0}!", value?.GetType().FullName));
        }
    }

    /// <summary>解码响应数据</summary>
    /// <param name="data">响应数据包</param>
    /// <param name="encoding">编码格式。HEX/ASCII/UTF8/Json，其他格式返回原数据包</param>
    /// <returns>解码结果，无法解码时返回原数据包</returns>
    public static Object? Decode(IPacket? data, String encoding)
    {
        return encoding switch
        {
            "HEX" => data?.ToHex(),
            "ASCII" => data?.ToStr(Encoding.ASCII),
            "UTF8" => data?.ToStr(Encoding.UTF8),
            "Json" => data == null ? null : JsonParser.Decode(data.ToStr()),
            _ => data,
        };
    }
}
