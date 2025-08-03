using System.ComponentModel;
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

/// <summary>IoT标准通用网络驱动</summary>
/// <remarks>
/// IoT驱动，符合IoT标准的通用网络驱动，连接后向目标发送数据即可收到数据。
/// </remarks>
[Driver("IoTSocket")]
[DisplayName("通用网络驱动")]
public class IoTSocketDriver : DriverBase<SocketNode, SocketParameter>
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
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    public override INode Open(IDevice device, IDriverParameter? parameter)
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

        return node;
    }

    /// <summary>创建网络</summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    protected virtual ISocketClient CreateClient(SocketParameter parameter)
    {
        var uri = new NetUri(parameter.Server);
        var client = uri.CreateRemote();

        client.Timeout = parameter.Timeout;

        return client;
    }

    /// <summary>关闭设备节点</summary>
    public override void Close(INode node)
    {
        if (Interlocked.Decrement(ref _nodes) <= 0)
        {
            _client.TryDispose();
            _client = null;
        }
    }

    /// <summary>读取数据</summary>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合，Address属性地址示例：D100、C100、W100、H100</param>
    /// <returns></returns>
    public override IDictionary<String, Object?> Read(INode node, IPoint[] points)
    {
        var result = new Dictionary<String, Object?>();
        if (points == null) return result;

        var client = (node as SocketNode)?.Client;
        if (client == null || node.Parameter is not SocketParameter parameter) return result;

        var request = Encode(parameter.RequestCommand);
        if (request != null) client.Send(request);
        var response = client.Receive();

        result["Data"] = Decode(response, parameter.ResponseEncoding);

        return result;
    }

    /// <summary>写入数据</summary>
    public override Object? Write(INode node, IPoint point, Object? value)
    {
        var client = (node as SocketNode)?.Client;
        if (client == null || node.Parameter is not SocketParameter parameter) return null;

        var request = Encode(value);
        if (request != null) client.Send(request);
        var response = client.Receive();

        return Decode(response, parameter.ResponseEncoding);
    }

    /// <summary>设备控制</summary>
    /// <param name="node"></param>
    /// <param name="parameters"></param>
    public override Object? Control(INode node, IDictionary<String, Object?> parameters)
    {
        var service = JsonHelper.Convert<ServiceModel>(parameters);
        if (service == null || service.Name.IsNullOrEmpty()) throw new NotImplementedException();

        var client = (node as SocketNode)?.Client;
        if (client == null || node.Parameter is not SocketParameter parameter) return null;

        // 批量操作
        var result = new Dictionary<String, Object?>();
        foreach (var item in parameters)
        {
            var request = Encode(item.Value);
            if (request != null) client.Send(request);
            var response = client.Receive();

            // 转换编码
            if (!parameter.ResponseEncoding.IsNullOrEmpty())
                result[item.Key] = Decode(response, parameter.ResponseEncoding);
            else
                result[item.Key] = response;
        }

        return result;
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