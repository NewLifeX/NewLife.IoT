using System.IO.Ports;
using NewLife.IoT;
using NewLife.IoT.Drivers;

namespace NewLife.IoTSerial.Drivers;

/// <summary>串口参数</summary>
[System.Text.Json.Serialization.JsonSerializable(typeof(SerialParameter))]
public class SerialParameter : IDriverParameter, IDriverParameterKey
{
    /// <summary>串口</summary>
    [LocalizedDescription("Serial port")]
    public String PortName { get; set; } = "COM1";

    /// <summary>波特率</summary>
    [LocalizedDescription("Baud rate")]
    public Int32 Baudrate { get; set; } = 9600;

    /// <summary>数据位。默认8</summary>
    [LocalizedDescription("Data bits")]
    public Int32 DataBits { get; set; } = 8;

    /// <summary>奇偶校验位。默认None无校验</summary>
    [LocalizedDescription("Parity")]
    public Parity Parity { get; set; } = Parity.None;

    /// <summary>停止位。默认One</summary>
    [LocalizedDescription("Stop bits")]
    public StopBits StopBits { get; set; } = StopBits.One;

    /// <summary>超时时间。发起请求后等待响应的超时时间，默认3000ms</summary>
    [LocalizedDescription("Timeout. Time to wait for a response after sending a request. Default 3000 ms.")]
    public Int32 Timeout { get; set; } = 3000;

    /// <summary>字节超时。数据包间隔，默认10ms</summary>
    [LocalizedDescription("Byte timeout. Interval between data packets. Default 10 ms.")]
    public Int32 ByteTimeout { get; set; } = 10;

    /// <summary>获取唯一标识</summary>
    /// <returns></returns>
    public String GetKey() => PortName;
}