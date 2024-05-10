namespace NewLife.IoT.ThingModels;

/// <summary>数据集合</summary>
public class DataModels
{
    /// <summary>消息编号。适用于多人同时操作，同一任务需要服务器端多指令串行下发等场景</summary>
    public Int64 Id { get; set; }

    /// <summary>设备编码</summary>
    public String DeviceCode { get; set; } = null!;

    /// <summary>数据集合</summary>
    public DataModel[]? Items { get; set; }
}