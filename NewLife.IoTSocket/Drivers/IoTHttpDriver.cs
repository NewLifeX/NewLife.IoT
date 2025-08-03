using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;
using NewLife;
using NewLife.Data;
using NewLife.IoT;
using NewLife.IoT.Drivers;
using NewLife.IoT.ThingModels;
using NewLife.Serialization;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace NewLife.IoTSocket.Drivers;

/// <summary>IoT标准通用Http驱动</summary>
/// <remarks>
/// IoT驱动，符合IoT标准的通用Http驱动，连接后向目标发送数据即可收到数据。
/// </remarks>
[Driver("IoTHttp")]
[DisplayName("通用网络驱动")]
public class IoTHttpDriver : AsyncDriverBase
{
    #region 属性
    /// <summary>客户端</summary>
    public HttpClient? Client { get; set; }
    #endregion

    #region 方法
    /// <summary>
    /// 打开设备驱动，传入参数。一个物理设备可能有多个逻辑设备共用，需要以节点来区分
    /// </summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">参数。不同驱动的参数设置相差较大，对象字典具有较好灵活性，其对应IDriverParameter</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    public override Task<INode> OpenAsync(IDevice device, IDriverParameter? parameter, CancellationToken cancellationToken = default)
    {
        if (parameter is not HttpParameter p) throw new ArgumentException("参数不能为空");
        if (p.Address.IsNullOrEmpty()) throw new ArgumentException("网络地址不能为空");

        var node = base.Open(device, parameter);

        var client = new HttpClient
        {
            BaseAddress = new Uri(p.Address),
            Timeout = TimeSpan.FromMilliseconds(p.Timeout),
        };

        if (!p.Token.IsNullOrEmpty())
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", p.Token);

        Client = client;

        return Task.FromResult(node);
    }

    /// <summary>
    /// 关闭设备节点。多节点共用通信链路时，需等最后一个节点关闭才能断开
    /// </summary>
    /// <param name="node"></param>
    /// <param name="cancellationToken">取消令牌</param>
    public override Task CloseAsync(INode node, CancellationToken cancellationToken = default)
    {
        Client.TryDispose();
        Client = null;

#if NET40 || NET45
        return TaskEx.FromResult(0);
#else
        return TaskEx.CompletedTask;
#endif
    }

    /// <summary>读取数据</summary>
    /// <remarks>
    /// 驱动实现数据采集的核心方法，各驱动全力以赴实现好该接口。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public override async Task<IDictionary<String, Object?>> ReadAsync(INode node, IPoint[] points, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<String, Object?>();
        if (points == null || points.Length == 0) return result;

        var client = Client;
        if (client == null) return result;
        if (node.Parameter is not HttpParameter parameter) return result;

        var path = parameter.PathAndQuery;
        if (path.IsNullOrEmpty()) path = "/";

        if (parameter.Method.EqualIgnoreCase("GET"))
        {
            var rs = await client.GetStringAsync(path);
            result["Data"] = rs;
        }
        else
        {
            var str = parameter.RequestCommand;
            if (str.IsNullOrEmpty())
            {
                var rs = await client.PostAsync(path, new StringContent(""), cancellationToken);
                result["Data"] = await rs.Content.ReadAsStringAsync();
            }
            else
            {
                var content = str.StartsWithIgnoreCase("0x")
                    ? new ByteArrayContent(str[2..].ToHex())
                    : (HttpContent)new StringContent(str, Encoding.UTF8, "application/json");

                var rs = await client.PostAsync(path, content, cancellationToken);
                result["Data"] = await rs.Content.ReadAsStringAsync();
            }
        }

        return result;
    }

    /// <summary>批量写入数据</summary>
    /// <remarks>
    /// 驱动实现远程控制的核心方法，各驱动全力以赴实现好该接口。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="values">点位数值</param>
    /// <param name="cancellationToken">取消令牌</param>
    public override TaskEx WriteAsync(INode node, IDictionary<IPoint, Object> values, CancellationToken cancellationToken = default) => base.WriteAsync(node, values, cancellationToken);
    #endregion
}