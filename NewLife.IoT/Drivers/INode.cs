namespace NewLife.IoT.Drivers;

/// <summary>
/// 节点接口
/// </summary>
public interface INode
{
    /// <summary>
    /// 设备驱动
    /// </summary>
    IDriver Driver { get; set; }

    /// <summary>
    /// 通道
    /// </summary>
    IChannel Channel { get; set; }

    /// <summary>
    /// 参数
    /// </summary>
    IDriverParameter Parameter { get; set; }
}