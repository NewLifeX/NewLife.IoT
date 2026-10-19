using NewLife.IoT;
using NewLife.IoT.Drivers;

namespace NewLife.IoTSocket.Drivers;

/// <summary>通用Http驱动参数</summary>
[System.Text.Json.Serialization.JsonSerializable(typeof(HttpParameter))]
public class HttpParameter : IDriverParameter
{
    /// <summary>服务端地址。如http://star.newlifex.com:6600</summary>
    [LocalizedDescription("Server address, for example http://star.newlifex.com:6600.")]
    public String Address { get; set; } = null!;

    /// <summary>请求方法。Http请求方法GET/POST，默认GET</summary>
    [LocalizedDescription("HTTP method. GET or POST. Default GET.")]
    public String Method { get; set; } = "GET";

    /// <summary>资源路径。请求Url的路径部分</summary>
    [LocalizedDescription("Resource path. Path portion of the request URL.")]
    public String? PathAndQuery { get; set; }

    /// <summary>令牌。在请求头中以Bearer形式传输</summary>
    [LocalizedDescription("Token. Sent in the request header.")]
    public String? Token { get; set; }

    /// <summary>超时时间。发起请求后等待响应的超时时间，默认5000ms</summary>
    [LocalizedDescription("Timeout. Time to wait for a response after sending a request. Default 5000 ms.")]
    public Int32 Timeout { get; set; } = 5_000;

    /// <summary>提交数据。支持十六进制格式(以0x开头)和字符串格式</summary>
    [LocalizedDescription("Post data. Supports hexadecimal (starting with 0x) and string formats.")]
    public String PostData { get; set; } = "";

    /// <summary>捕获所有字段。捕获所有字段作为返回，而不管指定哪些点位，默认false</summary>
    [LocalizedDescription("Capture all fields. Return all fields regardless of the specified points. Default false.")]
    public Boolean CaptureAll { get; set; }
}
