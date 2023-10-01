namespace NewLife.IoT.Models;

/// <summary>设备信息</summary>
/// <remarks>
/// 客户端与服务端之间交换设备信息。
/// 
/// 客户端启动时，从服务端获取设备信息，用于后续操作；
/// 客户端子设备上线时，上报设备信息，服务端新增子设备或更新状态；
/// </remarks>
public interface IDeviceInfo
{
    /// <summary>编码</summary>
    String Code { get; set; }

    /// <summary>名称</summary>
    String? Name { get; set; }

    /// <summary>产品编码</summary>
    String? ProductCode { get; set; }
}