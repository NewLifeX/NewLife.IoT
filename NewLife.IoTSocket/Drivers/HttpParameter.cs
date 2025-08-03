using System.ComponentModel;
using NewLife.IoT.Drivers;

namespace NewLife.IoTSocket.Drivers;

/// <summary>通用Http驱动参数</summary>
public class HttpParameter : IDriverParameter
{
    /// <summary>服务端地址。如http://star.newlifex.com:6600</summary>
    [Description("服务端地址。如http://star.newlifex.com:6600")]
    public String Address { get; set; } = null!;

    /// <summary>请求方法。Http请求方法GET/POST，默认GET</summary>
    [Description("请求方法。Http请求方法GET/POST，默认GET")]
    public String Method { get; set; } = "GET";

    /// <summary>资源路径。请求Url的路径部分</summary>
    [Description("资源路径。请求Url的路径部分")]
    public String? PathAndQuery { get; set; }

    /// <summary>令牌。在请求头中以Bearer形式传输</summary>
    [Description("令牌。在请求头中以Bearer形式传输")]
    public String? Token { get; set; }

    /// <summary>超时时间。发起请求后等待响应的超时时间，默认5000ms</summary>
    [Description("超时时间。发起请求后等待响应的超时时间，默认5000ms")]
    public Int32 Timeout { get; set; } = 5_000;

    /// <summary>请求命令模板。支持十六进制格式(以0x开头)和字符串格式</summary>
    [Description("请求命令模板。支持十六进制格式(以0x开头)和字符串格式")]
    public String RequestCommand { get; set; } = "";
}
