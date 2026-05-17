using System.Net.Http;
using System.Net.Http.Headers;
using NewLife.IoT.Drivers;
using NewLife.Log;

namespace NewLife.IoTSocket.Transport;

/// <summary>HTTP 传输层。将 <see cref="HttpClient"/> 封装为 <see cref="ITransport"/> 接口</summary>
/// <remarks>
/// HTTP 是无状态请求-响应协议，<see cref="SendReceive"/> 为主要使用方式。
/// <see cref="Send"/> 以即发即弃方式投递 POST 请求；<see cref="Receive"/> 对 HTTP 无意义，始终返回 null。
/// </remarks>
public class HttpTransport : DisposeBase, ITransport
{
    #region 属性

    /// <summary>服务端地址。如 http://192.168.1.1:8080</summary>
    public String Address { get; set; } = null!;

    /// <summary>资源路径。附加在 Address 之后的路径和查询字符串</summary>
    public String? PathAndQuery { get; set; }

    /// <summary>请求方法。GET 或 POST，默认 POST</summary>
    public String Method { get; set; } = "POST";

    /// <summary>令牌。在请求头中以 Bearer 形式传输</summary>
    public String? Token { get; set; }

    /// <summary>超时时间，ms。默认 5000</summary>
    public Int32 Timeout { get; set; } = 5000;

    /// <summary>是否已打开（Address 不为空且客户端已创建）</summary>
    public Boolean IsConnected => _client != null && !Address.IsNullOrEmpty();

    /// <summary>追踪器</summary>
    public ITracer? Tracer { get; set; }

    /// <summary>日志</summary>
    public ILog Log { get; set; } = Logger.Null;

    #endregion

    #region 私有字段

    private HttpClient? _client;

    #endregion

    #region 方法

    /// <summary>销毁</summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);
        _client?.Dispose();
        _client = null;
    }

    /// <summary>初始化 HTTP 客户端</summary>
    public void Open()
    {
        _client ??= CreateClient();
    }

    /// <summary>创建 HTTP 客户端。可重写以自定义 HttpClientHandler 等配置</summary>
    /// <returns>HttpClient 实例</returns>
    protected virtual HttpClient CreateClient()
    {
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromMilliseconds(Timeout),
        };
        if (!Token.IsNullOrEmpty())
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        return client;
    }

    /// <summary>关闭并释放 HTTP 客户端</summary>
    public void Close()
    {
        _client?.Dispose();
        _client = null;
    }

    /// <summary>即发即弃地发送 POST 请求，不等待响应</summary>
    /// <param name="data">请求体数据</param>
    public void Send(ReadOnlySpan<Byte> data)
    {
        if (_client == null) Open();
        var content = new ByteArrayContent(data.ToArray());
        // 即发即弃，不 await，不阻塞调用线程
        _ = _client!.PostAsync(BuildUrl(), content);
    }

    /// <summary>HTTP 协议无法单独接收，始终返回 null</summary>
    /// <param name="timeout">忽略</param>
    /// <returns>null</returns>
    public Byte[]? Receive(Int32 timeout = -1) => null;

    /// <summary>发送 HTTP 请求并返回响应体字节</summary>
    /// <param name="request">请求体数据（GET 时忽略）</param>
    /// <param name="timeout">超时时间 ms，-1 使用 HttpClient 默认超时</param>
    /// <returns>响应体字节，请求失败时返回 null</returns>
    public Byte[]? SendReceive(ReadOnlySpan<Byte> request, Int32 timeout = -1)
    {
        if (_client == null) Open();

        HttpResponseMessage response;
        if (Method.EqualIgnoreCase("GET"))
        {
            response = _client!.GetAsync(BuildUrl()).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        else
        {
            var content = new ByteArrayContent(request.ToArray());
            response = _client!.PostAsync(BuildUrl(), content).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsByteArrayAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    /// <summary>构建完整请求 URL</summary>
    /// <returns>URL 字符串</returns>
    private String BuildUrl()
    {
        if (PathAndQuery.IsNullOrEmpty()) return Address;
        return Address.TrimEnd('/') + "/" + PathAndQuery!.TrimStart('/');
    }

    #endregion

    #region 日志

    /// <summary>输出日志</summary>
    /// <param name="format">格式</param>
    /// <param name="args">参数</param>
    public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);

    #endregion
}
