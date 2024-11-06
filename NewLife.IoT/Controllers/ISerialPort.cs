using NewLife.Data;
using NewLife.Net;

namespace NewLife.IoT.Controllers;

/// <summary>串口接口</summary>
/// <remarks>
/// 基础串口类SerialPort的接口，方便模拟串口，以及使用其它串口类。
/// </remarks>
public interface ISerialPort
{
    /// <summary>串口名</summary>
    String PortName { get; set; }

    /// <summary>波特率</summary>
    Int32 Baudrate { get; set; }

    /// <summary>收到数据事件</summary>
    event EventHandler<ReceivedEventArgs> Received;

    /// <summary>串口对象</summary>
    Object Port { get; }

    /// <summary>打开</summary>
    void Open();

    /// <summary>发送数据</summary>
    /// <param name="buffer">待发送数据</param>
    /// <param name="offset">偏移</param>
    /// <param name="count">个数</param>
    void Write(Byte[] buffer, Int32 offset, Int32 count);

    /// <summary>接收数据</summary>
    /// <param name="buffer">接收缓冲区</param>
    /// <param name="offset">偏移</param>
    /// <param name="count">个数</param>
    /// <returns>已接收数据的字节数</returns>
    Int32 Read(Byte[] buffer, Int32 offset, Int32 count);

    /// <summary>调用，发送数据后等待响应</summary>
    /// <param name="request">待发送数据</param>
    /// <param name="minLength">等待响应数据的最小长度，默认1</param>
    /// <returns></returns>
    IPacket Invoke(IPacket? request, Int32 minLength = 1);
}
