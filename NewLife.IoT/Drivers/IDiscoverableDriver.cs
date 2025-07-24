using NewLife.IoT.Models;

namespace NewLife.IoT.Drivers;

/// <summary>为支持设备自动发现的驱动定义契约</summary>
public interface IDiscoverableDriver
{
    /// <summary>异步扫描和发现网络或串行总线上的兼容设备</summary>
    /// <param name="parameters">
    /// 发现操作所需的参数字典。
    /// Key-Value 示例:
    /// - For ModbusTCP: {"Subnet": "192.168.1.0/24"}
    /// - For ModbusRTU: {"ComPorts": ["COM1", "COM3"], "BaudRates": [9600, 19200]}
    /// </param>
    /// <param name="cancellationToken">用于取消长时间运行的发现操作。</param>
    /// <returns>一个包含所有被发现设备信息的枚举集合。</returns>
    Task<IEnumerable<IDeviceInfo>> DiscoverAsync(
        Dictionary<String, Object> parameters,
        CancellationToken cancellationToken = default);
}
