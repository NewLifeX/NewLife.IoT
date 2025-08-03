using NewLife.IoT.Drivers;
using NewLife.Net;

namespace NewLife.IoTSocket.Drivers;

/// <summary>
/// 网络节点。多设备共用驱动时，以节点区分
/// </summary>
public class SocketNode : Node
{
    /// <summary>客户端</summary>
    public ISocketClient Client { get; set; } = null!;

    /// <summary>
    /// 参数。设备使用的专用参数
    /// </summary>
    public SocketParameter? SocketParameter { get; set; }
}