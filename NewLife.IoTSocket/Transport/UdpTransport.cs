using NewLife.Net;

namespace NewLife.IoTSocket.Transport;

/// <summary>UDP 传输层</summary>
/// <remarks>
/// 基于 UDP 协议的 <see cref="SocketTransport"/> 实现，适用于无连接的广播/单播场景。
/// </remarks>
public class UdpTransport : SocketTransport
{
    /// <summary>创建 UDP 网络客户端</summary>
    /// <returns>UDP 客户端实例</returns>
    protected override ISocketClient CreateClient()
    {
        var uri = new NetUri(NetType.Udp, Server, Port);
        var client = uri.CreateRemote();
        client.Timeout = Timeout;
        return client;
    }
}
