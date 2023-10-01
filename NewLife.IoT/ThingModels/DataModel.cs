namespace NewLife.IoT.ThingModels;

/// <summary>数据模型</summary>
public class DataModel
{
    /// <summary>时间。数据采集时间，UTC毫秒</summary>
    public Int64 Time { get; set; }

    /// <summary>名称</summary>
    public String Name { get; set; } = null!;

    /// <summary>数据</summary>
    public String? Value { get; set; }
}