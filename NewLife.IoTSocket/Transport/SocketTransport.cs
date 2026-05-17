using NewLife.Data;
using NewLife.Log;
using NewLife.Net;
using IoTTransport = NewLife.IoT.Drivers.ITransport;

namespace NewLife.IoTSocket.Transport;

/// <summary>基于 <see cref="ISocketClient"/> 的网络传输层抽象基类</summary>
/// <remarks>
/// 封装 NewLife.Net 的 ISocketClient 为 <see cref="ITransport"/> 接口，
/// 子类通过重写 <see cref="CreateClient"/> 决定具体协议（TCP / UDP）。
/// </remarks>
public abstract class SocketTransport : DisposeBase, IoTTransport
{
    #region 属性

    /// <summary>服务端地址。默认 127.0.0.1</summary>
    public String Server { get; set; } = "127.0.0.1";

    /// <summary>端口号。默认 5500</summary>
    public Int32 Port { get; set; } = 5500;

    /// <summary>超时时间，ms。默认 3000</summary>
    public Int32 Timeout { get; set; } = 3000;

    /// <summary>是否已创建客户端（连接就绪）</summary>
    public Boolean IsConnected => _client != null;

    /// <summary>追踪器</summary>
    public ITracer? Tracer { get; set; }

    /// <summary>日志</summary>
    public ILog Log { get; set; } = Logger.Null;

    #endregion

    #region 私有字段

    private ISocketClient? _client;
    private readonly Object _lock = new();

    #endregion

    #region 方法

    /// <summary>销毁</summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);
        _client.TryDispose();
        _client = null;
    }

    /// <summary>打开网络连接</summary>
    public void Open()
    {
        if (_client != null) return;
        lock (_lock)
        {
            if (_client != null) return;
            _client = CreateClient();
        }
    }

    /// <summary>创建网络客户端。子类重写以指定协议类型</summary>
    /// <returns>网络客户端实例</returns>
    protected abstract ISocketClient CreateClient();

    /// <summary>关闭网络连接</summary>
    public void Close()
    {
        _client.TryDispose();
        _client = null;
    }

    /// <summary>发送数据</summary>
    /// <param name="data">待发送的字节数据</param>
    public void Send(ReadOnlySpan<Byte> data)
    {
        if (_client == null) Open();
        var pk = new ArrayPacket(data.ToArray());
        _client!.Send(pk);
    }

    /// <summary>接收数据</summary>
    /// <param name="timeout">超时时间 ms，-1 使用客户端默认超时</param>
    /// <returns>接收到的数据，无数据时返回 null</returns>
    public Byte[]? Receive(Int32 timeout = -1)
    {
        if (_client == null) return null;
        var response = _client.Receive();
        return response?.ToArray();
    }

    /// <summary>发送数据并等待响应</summary>
    /// <param name="request">请求数据</param>
    /// <param name="timeout">超时时间 ms，-1 使用客户端默认超时</param>
    /// <returns>响应数据，超时或无响应返回 null</returns>
    public Byte[]? SendReceive(ReadOnlySpan<Byte> request, Int32 timeout = -1)
    {
        if (_client == null) Open();
        var pk = new ArrayPacket(request.ToArray());
        _client!.Send(pk);
        var response = _client.Receive();
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
