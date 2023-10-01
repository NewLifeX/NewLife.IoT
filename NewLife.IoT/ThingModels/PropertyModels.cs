namespace NewLife.IoT.ThingModels;

/// <summary>
/// 属性集合
/// </summary>
public class PropertyModels
{
    /// <summary>设备编码</summary>
    public String DeviceCode { get; set; } = null!;

    /// <summary>属性集合</summary>
    public PropertyModel[]? Items { get; set; }
}