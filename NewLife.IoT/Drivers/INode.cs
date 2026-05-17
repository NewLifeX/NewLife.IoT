namespace NewLife.IoT.Drivers;

/// <summary>
/// 节点接口。多设备共用驱动时，以节点区分
/// </summary>
public interface INode
{
    /// <summary>
    /// 驱动。设备节点使用的驱动对象，可能多设备共用
    /// </summary>
    IDriver Driver { get; set; }

    /// <summary>
    /// 设备。业务逻辑设备
    /// </summary>
    IDevice? Device { get; set; }

    /// <summary>
    /// 参数。设备使用的专用参数
    /// </summary>
    IDriverParameter? Parameter { get; set; }

    /// <summary>是否已连接。驱动维护的连接状态，无需触发一次采集才能感知连接健康</summary>
    /// <remarks>轮询驱动默认返回 true；长连接驱动（TCP/串口等）在断线时应置为 false</remarks>
    Boolean IsConnected { get; }
}

/// <summary>
/// 节点。多设备共用驱动时，以节点区分
/// </summary>
public class Node : INode
{
    /// <summary>
    /// 驱动。设备节点使用的驱动对象，可能多设备共用
    /// </summary>
    public IDriver Driver { get; set; } = null!;

    /// <summary>
    /// 设备。业务逻辑设备
    /// </summary>
    public IDevice? Device { get; set; }

    /// <summary>
    /// 参数。设备使用的专用参数
    /// </summary>
    public IDriverParameter? Parameter { get; set; }

    /// <summary>是否已连接。默认 true，长连接驱动可在 OpenAsync/CloseAsync 中维护此状态</summary>
    public Boolean IsConnected { get; set; } = true;
}