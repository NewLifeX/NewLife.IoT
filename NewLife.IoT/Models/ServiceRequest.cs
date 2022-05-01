using System;
using System.Collections.Generic;
using System.Text;

namespace NewLife.IoT.Models;

/// <summary>服务请求</summary>
public class ServiceRequest
{
    /// <summary>设备编号</summary>
    public Int32 DeviceId { get; set; }

    /// <summary>设备编码</summary>
    public String DeviceCode { get; set; }

    /// <summary>服务名</summary>
    public String ServiceName { get; set; }

    /// <summary>入参</summary>
    public String InputData { get; set; }

    /// <summary>过期时间。未指定时表示不限制</summary>
    public DateTime Expire { get; set; }

    /// <summary>超时时间。如果指定，则等待服务调用返回，单位毫秒</summary>
    public Int32 Timeout { get; set; }
}