using System.IO.Ports;
using NewLife.Data;
using NewLife.IoT.Controllers;
using NewLife.IoT.Drivers;
using NewLife.Log;

namespace NewLife.IoTSerial.Transport;

/// <summary>串口传输层。将 <see cref="ISerialPort"/> 封装为 <see cref="ITransport"/> 字节流接口</summary>
/// <remarks>
/// 每个 SerialTransport 实例对应一个物理串口，提供 Open/Close/Send/Receive/SendReceive 操作。
/// SendReceive 使用 <see cref="ISerialPort.Invoke"/> 实现原子化发送-等待响应；
/// 单独的 Send / Receive 用于需要拆分收发的场景。
/// </remarks>
public class SerialTransport : DisposeBase, ITransport
{
    #region 属性

    /// <summary>串口名称。默认 COM1</summary>
    public String PortName { get; set; } = "COM1";

    /// <summary>波特率。默认 9600</summary>
    public Int32 Baudrate { get; set; } = 9600;

    /// <summary>数据位。默认 8</summary>
    public Int32 DataBits { get; set; } = 8;

    /// <summary>奇偶校验位。默认 None 无校验</summary>
    public Parity Parity { get; set; } = Parity.None;

    /// <summary>停止位。默认 One</summary>
    public StopBits StopBits { get; set; } = StopBits.One;

    /// <summary>超时时间，ms。发起请求后等待响应的超时时间，默认 3000</summary>
    public Int32 Timeout { get; set; } = 3000;

    /// <summary>字节超时，ms。数据包字节间隔，默认 10</summary>
    public Int32 ByteTimeout { get; set; } = 10;

    /// <summary>是否已打开连接</summary>
    public Boolean IsConnected { get; private set; }

    /// <summary>追踪器</summary>
    public ITracer? Tracer { get; set; }

    /// <summary>日志</summary>
    public ILog Log { get; set; } = Logger.Null;

    #endregion

    #region 私有字段

    private ISerialPort? _serialPort;
    private readonly Object _lock = new();

    #endregion

    #region 方法

    /// <summary>销毁</summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);
        _serialPort.TryDispose();
        _serialPort = null;
        IsConnected = false;
    }

    /// <summary>打开串口</summary>
    public void Open()
    {
        if (IsConnected) return;
        lock (_lock)
        {
            if (IsConnected) return;
            _serialPort ??= CreateSerial();
            _serialPort.Open();
            IsConnected = true;
        }
    }

    /// <summary>创建串口对象。可重写以自定义串口实现</summary>
    /// <returns>串口实例</returns>
    protected virtual ISerialPort CreateSerial()
    {
        var sp = new DefaultSerialPort
        {
            PortName = PortName,
            Baudrate = Baudrate,
            DataBits = DataBits,
            Parity = Parity,
            StopBits = StopBits,
            Timeout = Timeout,
            ByteTimeout = ByteTimeout,
        };
        return sp;
    }

    /// <summary>关闭串口</summary>
    public void Close()
    {
        _serialPort?.Close();
        IsConnected = false;
    }

    /// <summary>发送数据</summary>
    /// <param name="data">待发送的字节数据</param>
    public void Send(ReadOnlySpan<Byte> data)
    {
        if (!IsConnected) Open();
        var buf = data.ToArray();
        _serialPort!.Write(buf, 0, buf.Length);
    }

    /// <summary>接收数据</summary>
    /// <param name="timeout">超时时间 ms，-1 使用串口默认超时</param>
    /// <returns>接收到的数据，无数据时返回 null</returns>
    public Byte[]? Receive(Int32 timeout = -1)
    {
        if (_serialPort == null || !IsConnected) return null;
        var buf = new Byte[4096];
        var count = _serialPort.Read(buf, 0, buf.Length);
        if (count <= 0) return null;
        var result = new Byte[count];
        Array.Copy(buf, result, count);
        return result;
    }

    /// <summary>发送数据并等待响应（原子化操作）</summary>
    /// <param name="request">请求数据</param>
    /// <param name="timeout">超时时间 ms，-1 使用串口默认超时</param>
    /// <returns>响应数据，超时或无响应返回 null</returns>
    public Byte[]? SendReceive(ReadOnlySpan<Byte> request, Int32 timeout = -1)
    {
        if (!IsConnected) Open();
        var pk = new ArrayPacket(request.ToArray());
        var response = _serialPort!.Invoke(pk, 1);
        return response?.ToArray();
    }

    #endregion

    #region 日志

    /// <summary>输出日志</summary>
    /// <param name="format">格式</param>
    /// <param name="args">参数</param>
    public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);

    #endregion
}
