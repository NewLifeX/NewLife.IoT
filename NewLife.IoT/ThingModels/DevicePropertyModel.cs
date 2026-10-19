namespace NewLife.IoT.ThingModels;

/// <summary>设备属性模型</summary>
[System.Text.Json.Serialization.JsonSerializable(typeof(DevicePropertyModel))]
public class DevicePropertyModel : PropertyModel
{
    /// <summary>设备编码</summary>
    public String DeviceCode { get; set; } = null!;
}
