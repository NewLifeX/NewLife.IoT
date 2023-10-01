namespace NewLife.IoT.ThingModels;

/// <summary>
/// 影子模型
/// </summary>
public class ShadowModel
{
    /// <summary>设备编码</summary>
    public String DeviceCode { get; set; } = null!;

    /// <summary>影子</summary>
    public Object? Shadow { get; set; }
}