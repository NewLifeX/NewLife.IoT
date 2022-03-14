using NewLife.IoT.Thing;
using NewLife.IoT.ThingSpecification;

namespace NewLife.IoT.Drivers;

/// <summary>设备通道。用于数据采集与远程控制的通道</summary>
public interface IChannel
{
    /// <summary>物模型主机。业务</summary>
    IThing Thing { get; }

    /// <summary>产品物模型。自定义客户端或驱动插件内部，可借助物模型去规范所需要采集的数据</summary>
    ThingSpec Specification { get; }
}