using NewLife.Log;

namespace NewLife.IoT.Drivers;

/// <summary>传输层接口。统一封装串口、TCP、UDP、HTTP等底层通信通道的连接管理与数据收发</summary>
/// <remarks>
/// ITransport 是驱动层与物理通信介质之间的抽象边界。
/// 驱动实现（IDriver）依赖此接口进行数据收发，而无需关心底层是串口还是网络。
/// 各通信类型的具体实现：
///   - 串口：NewLife.IoTSerial.Transport.SerialTransport
///   - TCP：NewLife.IoTSocket.Transport.TcpTransport
///   - UDP：NewLife.IoTSocket.Transport.UdpTransport
///   - HTTP：NewLife.IoTSocket.Transport.HttpTransport
/// </remarks>
public interface ITransport : IDisposable
{
    #region 属性

    /// <summary>当前是否已连接</summary>
    Boolean IsConnected { get; }

    /// <summary>收发超时。单位毫秒，默认3000</summary>
    Int32 Timeout { get; set; }

    /// <summary>性能追踪器</summary>
    ITracer? Tracer { get; set; }

    /// <summary>日志</summary>
    ILog Log { get; set; }

    #endregion

    #region 核心方法

    /// <summary>打开连接。已连接时不重复打开</summary>
    void Open();

    /// <summary>关闭连接</summary>
    void Close();

    /// <summary>发送数据</summary>
    /// <param name="data">待发送的字节数据</param>
    void Send(ReadOnlySpan<Byte> data);

    /// <summary>接收数据</summary>
    /// <param name="timeout">超时毫秒数，-1表示使用默认Timeout</param>
    /// <returns>接收到的字节数组，超时或连接断开时返回null</returns>
    Byte[]? Receive(Int32 timeout = -1);

    /// <summary>发送请求并等待响应。串行请求-响应模式的核心方法</summary>
    /// <remarks>
    /// 对于Modbus、自定义串口协议等请求-响应型协议，推荐使用此方法。
    /// 内部保证发送和接收的原子性，多线程场景下自动串行化。
    /// </remarks>
    /// <param name="request">请求字节数据</param>
    /// <param name="timeout">超时毫秒数，-1表示使用默认Timeout</param>
    /// <returns>响应字节数组，超时或出错时返回null</returns>
    Byte[]? SendReceive(ReadOnlySpan<Byte> request, Int32 timeout = -1);

    #endregion
}
