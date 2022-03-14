namespace NewLife.IoT.ThingModels;

/// <summary>
/// 服务模型
/// </summary>
public class ServiceModel
{
    /// <summary>编号</summary>
    public Int64 Id { get; set; }

    /// <summary>服务名</summary>
    public String Name { get; set; }

    /// <summary>入参</summary>
    public String InputData { get; set; }

    /// <summary>过期时间。未指定时表示不限制</summary>
    public DateTime Expire { get; set; }

    /// <summary>设备编码</summary>
    public String DeviceCode { get; set; }

    /// <summary>链路追踪</summary>
    public String TraceId { get; set; }
}