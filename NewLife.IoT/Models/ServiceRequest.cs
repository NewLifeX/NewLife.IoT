namespace NewLife.IoT.Models;

/// <summary>服务请求。应用系统之前发送服务调用请求</summary>
public class ServiceRequest
{
    /// <summary>设备编号。目标设备</summary>
    public Int32 DeviceId { get; set; }

    /// <summary>设备编码。目标设备或其子设备</summary>
    public String? DeviceCode { get; set; }

    /// <summary>服务名</summary>
    public String ServiceName { get; set; } = null!;

    /// <summary>入参。传递给该服务的参数，常见Json格式</summary>
    public String? InputData { get; set; }

    /// <summary>开始执行时间。用于提前下发指令后延期执行</summary>
    public DateTime StartTime { get;set; }

    /// <summary>过期时间。超过该时间后不再执行，未指定时表示不限制</summary>
    public DateTime Expire { get; set; }

    /// <summary>超时时间。如果指定，则等待服务调用返回，单位毫秒</summary>
    /// <remarks>服务调用接口内部一般使用异步阻塞的方式来实现超时控制，例如Redis队列</remarks>
    public Int32 Timeout { get; set; }
}