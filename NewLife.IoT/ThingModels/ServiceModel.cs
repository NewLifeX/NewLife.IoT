namespace NewLife.IoT.ThingModels;

/// <summary>服务调用模型。平台向设备发起服务调用请求</summary>
public class ServiceModel
{
    /// <summary>编号。用于服务调用请求与结果响应配对</summary>
    public Int64 Id { get; set; }

    /// <summary>服务名。如SetProperty</summary>
    public String Name { get; set; } = null!;

    /// <summary>入参。传递给该服务的参数，常见Json格式</summary>
    public String? InputData { get; set; }

    /// <summary>开始执行时间。用于提前下发指令后延期执行，暂时不支持取消</summary>
    public DateTime StartTime { get; set; }

    /// <summary>过期时间。超过该时间后不再执行，未指定时表示不限制</summary>
    public DateTime Expire { get; set; }

    /// <summary>设备编码。服务调用请求发送给网关设备时，该参数指定子设备编码</summary>
    public String? DeviceCode { get; set; }

    /// <summary>服务类型</summary>
    public String? Type { get; set; }

    /// <summary>链路追踪</summary>
    public String? TraceId { get; set; }
}