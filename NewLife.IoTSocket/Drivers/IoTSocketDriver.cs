using System.Text;
using NewLife;
using NewLife.Data;
using NewLife.IoT;
using NewLife.IoT.Drivers;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using NewLife.Net;
using NewLife.Serialization;

namespace NewLife.IoTSocket.Drivers;

/// <summary>IoT标准通用T网络驱动</summary>
/// <remarks>
/// IoT驱动，符合IoT标准的通用网络驱动，连接后向目标发送数据即可收到数据。
/// </remarks>
public abstract class IoTSocketDriver : DriverBase<SocketNode, SocketParameter>
{
    #region 属性
    private Int32 _nodes;
    private ISocketClient? _client;
    #endregion

    #region 方法
    /// <summary>获取产品物模型</summary>
    protected override Boolean OnGetSpecification(ThingSpec thingSpec)
    {
        thingSpec.Properties =
        [
            PropertySpec.Create("Data", "响应数据", "string")
        ];

        return true;
    }

    /// <summary>
    /// 打开设备驱动，传入参数。一个物理设备可能有多个逻辑设备共用，需要以节点来区分
    /// </summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">参数。不同驱动的参数设置相差较大，对象字典具有较好灵活性，其对应IDriverParameter</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    public override Task<INode> OpenAsync(IDevice device, IDriverParameter? parameter, CancellationToken cancellationToken = default)
    {
        if (parameter is not SocketParameter p) throw new ArgumentException("参数不能为空");
        if (p.Server.IsNullOrEmpty()) throw new ArgumentException("网络地址不能为空");

        var node = new SocketNode
        {
            Driver = this,
            Device = device,
            Parameter = p,
            SocketParameter = p,
        };

        if (_client == null)
        {
            lock (this)
            {
                _client ??= CreateClient(p);
            }
        }

        node.Client = _client;

        Interlocked.Increment(ref _nodes);

        return TaskEx.FromResult(node as INode)!;
    }

    /// <summary>创建网络</summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    protected virtual ISocketClient CreateClient(SocketParameter parameter)
    {
        var uri = new NetUri(NetType.Tcp, parameter.Server, parameter.Port);
        var client = uri.CreateRemote();

        client.Timeout = parameter.Timeout;

        return client;
    }

    /// <summary>关闭设备节点</summary>
    public override Task CloseAsync(INode node, CancellationToken cancellationToken = default)
    {
        if (Interlocked.Decrement(ref _nodes) <= 0)
        {
            _client.TryDispose();
            _client = null;
        }

        return TaskEx.CompletedTask;
    }

    /// <summary>读取数据</summary>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合，Address属性地址示例：D100、C100、W100、H100</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public override Task<ReadResult> ReadAsync(INode node, IPoint[] points, CancellationToken cancellationToken = default)
    {
        if (points == null) return Task.FromResult(ReadResult.Success([], []));

        var client = (node as SocketNode)?.Client;
        if (client == null || node.Parameter is not SocketParameter parameter)
            return Task.FromResult(ReadResult.Success(points, new Object?[points.Length]));

        var request = Encode(parameter.RequestCommand);
        if (request != null) client.Send(request);
        var response = client.Receive();
        var decoded = Decode(response, parameter.ResponseEncoding);

        var values = new Object?[points.Length];
        // 将解码结果分配到名为 Data 的点位，没有则第 0 项
        var dataIdx = 0;
        for (var i = 0; i < points.Length; i++)
        {
            if (points[i].Name.EqualIgnoreCase("Data")) { dataIdx = i; break; }
        }
        values[dataIdx] = decoded;

        return Task.FromResult(ReadResult.Success(points, values));
    }

    /// <summary>写入数据。requests.Length==1 时单点写入并返回回显值；多点时逐项写入并返回成功计数</summary>
    /// <param name="node">节点对象</param>
    /// <param name="requests">写入请求数组</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>写入结果</returns>
    public override Task<WriteResult> WriteAsync(INode node, WriteRequest[] requests, CancellationToken cancellationToken = default)
    {
        var client = (node as SocketNode)?.Client;
        if (client == null || node.Parameter is not SocketParameter parameter)
            return Task.FromResult(WriteResult.Fail(IoTErrorCode.InvalidParameter, "节点或参数无效"));

        if (requests.Length == 1)
        {
            var encoded = Encode(requests[0].Value);
            if (encoded != null) client.Send(encoded);
            var response = client.Receive();
            return Task.FromResult(WriteResult.Success(Decode(response, parameter.ResponseEncoding)));
        }

        var count = 0;
        foreach (var req in requests)
        {
            var encoded = Encode(req.Value);
            if (encoded != null) client.Send(encoded);
            client.Receive();
            count++;
        }
        return Task.FromResult(WriteResult.SuccessBatch(count));
    }

    /// <summary>设备控制</summary>
    /// <param name="node">节点对象</param>
    /// <param name="request">控制请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>控制结果</returns>
    public override Task<ControlResult> ControlAsync(INode node, ControlRequest request, CancellationToken cancellationToken = default)
    {
        if (request.ServiceName.IsNullOrEmpty()) throw new NotImplementedException();

        var client = (node as SocketNode)?.Client;
        if (client == null || node.Parameter is not SocketParameter parameter)
            return Task.FromResult(ControlResult.Fail(IoTErrorCode.InvalidParameter, "节点或参数无效"));

        // 批量操作
        var result = new Dictionary<String, Object?>();
        foreach (var item in request.Parameters)
        {
            var encoded = Encode(item.Value);
            if (encoded != null) client.Send(encoded);
            var response = client.Receive();

            // 转换编码
            if (!parameter.ResponseEncoding.IsNullOrEmpty())
                result[item.Key] = Decode(response, parameter.ResponseEncoding);
            else
                result[item.Key] = response;
        }

        return Task.FromResult(ControlResult.Success(result));
    }

    /// <summary>编码请求数据</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected virtual IPacket? Encode(Object? value)
    {
        if (value == null) return null;

        switch (value)
        {
            case IPacket pk:
                return pk;
            case String str:
                if (str.StartsWithIgnoreCase("0x"))
                    return new ArrayPacket(str[2..].ToHex());
                else
                    return new ArrayPacket(str.GetBytes());
            case Byte[] bytes:
                return new ArrayPacket(bytes);
            default:
                throw new NotSupportedException($"不支持的数据类型 {value?.GetType().FullName}");
        }
    }

    /// <summary>解码响应数据</summary>
    /// <param name="data"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    protected virtual Object? Decode(IPacket? data, String encoding)
    {
        return encoding switch
        {
            "HEX" => data?.ToHex(),
            "ASCII" => data?.ToStr(Encoding.ASCII),
            "UTF8" => data?.ToStr(Encoding.UTF8),
            _ => data,
        };
    }
    #endregion
}