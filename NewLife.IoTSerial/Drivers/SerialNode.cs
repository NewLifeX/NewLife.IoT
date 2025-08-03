using NewLife.IoT.Controllers;
using NewLife.IoT.Drivers;

namespace NewLife.IoTSerial.Drivers;

/// <summary>
/// 串口节点。多设备共用驱动时，以节点区分
/// </summary>
public class SerialNode : Node
{
    /// <summary>串口对象</summary>
    public ISerialPort SerialPort { get; set; } = null!;

    /// <summary>
    /// 参数。设备使用的专用参数
    /// </summary>
    public SerialParameter? SerialParameter { get; set; }
}