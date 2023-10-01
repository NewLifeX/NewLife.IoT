namespace NewLife.IoT.ThingModels;

/// <summary>数据集合</summary>
public class DataModels
{
    /// <summary>设备编码</summary>
    public String DeviceCode { get; set; } = null!;

    /// <summary>数据集合</summary>
    public DataModel[]? Items { get; set; }
}