namespace NewLife.IoT.Models;

/// <summary>设备信息</summary>
public interface IDeviceInfo
{
    /// <summary>编码</summary>
    String Code { get; set; }

    /// <summary>名称</summary>
    String Name { get; set; }

    /// <summary>产品编码</summary>
    String ProductCode { get; set; }
}