using NewLife.Net;

namespace NewLife.IoTSocket.Transport;

/// <summary>TCP 传输层</summary>
/// <remarks>
/// 基于 TCP 协议的 <see cref="SocketTransport"/> 实现，适用于面向连接的可靠传输场景。
/// </remarks>
public class TcpTransport : SocketTransport
{
    /// <summary>创建 TCP 网络客户端</summary>
    /// <returns>TCP 客户端实例</returns>
    protected override ISocketClient CreateClient()
    {
        var uri = new NetUri(NetType.Tcp, Server, Port);
        var client = uri.CreateRemote();
        client.Timeout = Timeout;
        return client;
    }
}
