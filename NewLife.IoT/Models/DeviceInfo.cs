namespace NewLife.IoT.Models;

/// <summary>设备信息</summary>
public class DeviceInfo : IDeviceInfo
{
    /// <summary>编码</summary>
    public String Code { get; set; }

    /// <summary>名称</summary>
    public String Name { get; set; }

    /// <summary>产品编码</summary>
    public String ProductCode { get; set; }

    /// <summary>设备参数。Xml/Json格式配置，根据协议驱动来解析</summary>
    public String Parameter { get; set; }
}