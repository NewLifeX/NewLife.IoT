using NewLife.IoT;
using NewLife.IoT.Drivers;

namespace NewLife.IoTSocket.Drivers;

/// <summary>通用网络驱动参数</summary>
[System.Text.Json.Serialization.JsonSerializable(typeof(SocketParameter))]
public class SocketParameter : IDriverParameter, IDriverParameterKey
{
    /// <summary>服务地址。IP地址或域名，例如：127.0.0.1</summary>
    [LocalizedDescription("Server address. IP address or domain name, for example 127.0.0.1.")]
    public String Server { get; set; } = "127.0.0.1";

    /// <summary>端口。例如：5500</summary>
    [LocalizedDescription("Port. For example 5500.")]
    public Int32 Port { get; set; } = 5500;

    /// <summary>超时时间。发起请求后等待响应的超时时间，默认3000ms</summary>
    [LocalizedDescription("Timeout. Time to wait for a response after sending a request. Default 3000 ms.")]
    public Int32 Timeout { get; set; } = 3000;

    /// <summary>请求命令模板。支持十六进制格式(以0x开头)和字符串格式</summary>
    [LocalizedDescription("Request command template. Supports hexadecimal (starting with 0x) and string formats.")]
    public String RequestCommand { get; set; } = "";

    /// <summary>响应数据编码格式。支持HEX、ASCII、UTF8等</summary>
    [LocalizedDescription("Response encoding. Supports HEX, ASCII, UTF8, Json, etc.")]
    public String ResponseEncoding { get; set; } = "HEX";

    /// <summary>获取唯一标识</summary>
    /// <returns></returns>
    public String GetKey() => $"{Server}:{Port}";
}
