using System.ComponentModel;
using NewLife.IoT.Drivers;

namespace NewLife.IoTSocket.Drivers;

/// <summary>通用网络驱动参数</summary>
public class SocketParameter : IDriverParameter, IDriverParameterKey
{
    /// <summary>地址。tcp地址如tcp://127.0.0.1:502</summary>
    [Description("地址。tcp地址如tcp://127.0.0.1:502")]
    public String Server { get; set; } = null!;

    /// <summary>超时时间。发起请求后等待响应的超时时间，默认3000ms</summary>
    [Description("超时时间。发起请求后等待响应的超时时间，默认3000ms")]
    public Int32 Timeout { get; set; } = 3000;

    /// <summary>请求命令模板。支持十六进制格式(以0x开头)和字符串格式</summary>
    [Description("请求命令模板。支持十六进制格式(以0x开头)和字符串格式")]
    public String RequestCommand { get; set; } = "";

    /// <summary>响应数据编码格式。支持HEX、ASCII、UTF8等</summary>
    [Description("响应数据编码格式。支持HEX、ASCII、UTF8等")]
    public String ResponseEncoding { get; set; } = "HEX";

    /// <summary>获取唯一标识</summary>
    /// <returns></returns>
    public String GetKey() => Server;
}
