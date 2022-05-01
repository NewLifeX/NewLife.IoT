using System;
using System.Collections.Generic;
using System.Text;

namespace NewLife.IoT.Models;

/// <summary>设备信息</summary>
public class DeviceInfo : IDeviceInfo
{
    /// <summary>编码</summary>
    public String Code { get; set; }

    /// <summary>名称</summary>
    public String Name { get; set; }
}