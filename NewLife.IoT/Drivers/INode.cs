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
}