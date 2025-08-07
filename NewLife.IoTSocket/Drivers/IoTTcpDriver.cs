using System.ComponentModel;
using NewLife.IoT.Drivers;
using NewLife.Net;

namespace NewLife.IoTSocket.Drivers;

/// <summary>IoT标准通用TCP网络驱动</summary>
/// <remarks>
/// IoT驱动，符合IoT标准的通用网络驱动，连接后向目标发送数据即可收到数据。
/// </remarks>
[Driver("IoTTcp")]
[DisplayName("通用TCP网络驱动")]
public class IoTTcpDriver : IoTSocketDriver
{
    #region 方法
    /// <summary>创建网络</summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    protected override ISocketClient CreateClient(SocketParameter parameter)
    {
        var uri = new NetUri(NetType.Tcp, parameter.Server, parameter.Port);
        var client = uri.CreateRemote();

        client.Timeout = parameter.Timeout;

        return client;
    }
    #endregion
}