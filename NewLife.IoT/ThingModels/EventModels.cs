namespace NewLife.IoT.ThingModels;

/// <summary>事件集合</summary>
public class EventModels
{
    /// <summary>设备编码</summary>
    public String DeviceCode { get; set; } = null!;

    /// <summary>事件集合</summary>
    public EventModel[]? Items { get; set; }
}