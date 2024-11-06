#if NETFRAMEWORK
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Ports;
using NewLife.Data;
using NewLife.Net;

namespace NewLife.IoT.Controllers;

/// <summary>默认串口实现</summary>
public class DefaultSerialPort : DisposeBase, ISerialPort
{
    /// <summary>串口名</summary>
    public String PortName { get; set; } = null!;

    /// <summary>波特率</summary>
    public Int32 Baudrate { get; set; }

    /// <summary>网络超时。发起请求后等待响应的超时时间，默认3000ms</summary>
    public Int32 Timeout { get; set; } = 3000;

    /// <summary>字节超时。数据包间隔，默认10ms</summary>
    public Int32 ByteTimeout { get; set; } = 10;

    /// <summary>缓冲区大小。默认256</summary>
    public Int32 BufferSize { get; set; } = 256;

    /// <summary>收到数据事件</summary>
    public event EventHandler<ReceivedEventArgs>? Received;

    private SerialPort? _port;
    /// <summary>串口对象</summary>
    public Object Port => _port ??= new(PortName, Baudrate) { ReadTimeout = Timeout, WriteTimeout = Timeout };

    /// <summary>销毁</summary>
    /// <param name="disposing"></param>
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);

        if (_port != null)
        {
            if (Received != null) _port.DataReceived -= OnReceiveSerial;

            _port.TryDispose();
        }
    }

    /// <summary>打开</summary>
    [MemberNotNull(nameof(_port))]
    public virtual void Open()
    {
        if (_port != null) return;

        if (PortName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(PortName));
        if (Baudrate == 0) Baudrate = 9600;

        _port = new SerialPort(PortName, Baudrate)
        {
            ReadTimeout = Timeout,
            WriteTimeout = Timeout
        };

        if (Received != null) _port.DataReceived += OnReceiveSerial;

        _port.Open();
    }

    void OnReceiveSerial(Object sender, SerialDataReceivedEventArgs e)
    {
        var rs = Invoke(null, 1);
        if (rs != null)
        {
            Received?.Invoke(this, new ReceivedEventArgs { Packet = rs });

            // 回收内存池
            rs.TryDispose();
        }
    }

    /// <summary>发送数据</summary>
    /// <param name="buffer">待发送数据</param>
    /// <param name="offset">偏移</param>
    /// <param name="count">个数</param>
    public virtual void Write(Byte[] buffer, Int32 offset, Int32 count)
    {
        Open();

        _port.Write(buffer, offset, count);
    }

    /// <summary>接收数据</summary>
    /// <param name="buffer">接收缓冲区</param>
    /// <param name="offset">偏移</param>
    /// <param name="count">个数</param>
    /// <returns>已接收数据的字节数</returns>
    public virtual Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
    {
        Open();

        return _port.Read(buffer, offset, count);
    }

    /// <summary>调用，发送数据后等待响应</summary>
    /// <param name="request">待发送数据</param>
    /// <param name="minLength">等待响应数据的最小长度，默认1</param>
    /// <returns></returns>
    public virtual IPacket Invoke(IPacket? request, Int32 minLength)
    {
        Open();

        if (request != null)
        {
            // 清空缓冲区
            _port.DiscardInBuffer();

            if (request.Next == null && request is ArrayPacket ap)
                _port.Write(ap.Buffer, ap.Offset, ap.Length);
            else
                _port.Write(request.ReadBytes(), 0, request.Total);

            if (ByteTimeout > 10) Thread.Sleep(ByteTimeout);
        }

        // 串口速度较慢，等待收完数据
        WaitMore(_port, minLength);

        var p = new OwnerPacket(BufferSize);
        var rs = _port.Read(p.Buffer, p.Offset, p.Length);
        p.Resize(rs);

        return p;
    }

    private void WaitMore(SerialPort sp, Int32 minLength)
    {
        var count = sp.BytesToRead;
        if (count >= minLength) return;

        var ms = ByteTimeout > 0 ? ByteTimeout : 10;
        var sw = Stopwatch.StartNew();
        while (sp.IsOpen && sw.ElapsedMilliseconds < Timeout)
        {
            //Thread.SpinWait(1);
            Thread.Sleep(ms);
            if (count != sp.BytesToRead)
            {
                count = sp.BytesToRead;
                if (count >= minLength) break;

                //sw.Restart();
            }
        }
    }
}
#endif
