using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;
using NewLife.IoT;
using NewLife.IoT.Drivers;
using NewLife.IoT.ThingModels;
using NewLife.Reflection;
using NewLife.Serialization;

namespace NewLife.IoTSocket.Drivers;

/// <summary>IoT标准通用Http驱动</summary>
/// <remarks>
/// IoT驱动，符合IoT标准的通用Http驱动，连接后向目标发送数据即可收到数据。
/// </remarks>
[Driver("IoTHttp")]
[DisplayName("通用Http驱动")]
public class IoTHttpDriver : DriverBase<Node, HttpParameter>
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
    public override async Task<INode> OpenAsync(IDevice device, IDriverParameter? parameter, CancellationToken cancellationToken = default)
    {
        if (parameter is not HttpParameter p) throw new ArgumentException("参数不能为空");
        if (p.Address.IsNullOrEmpty()) throw new ArgumentException("网络地址不能为空");

        var node = await base.OpenAsync(device, parameter);

        var client = new HttpClient
        {
            BaseAddress = new Uri(p.Address),
            Timeout = TimeSpan.FromMilliseconds(p.Timeout),
        };

        if (!p.Token.IsNullOrEmpty())
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", p.Token);

        Client = client;

        return node;
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

        return TaskEx.CompletedTask;
    }

    /// <summary>读取数据</summary>
    /// <remarks>
    /// 驱动实现数据采集的核心方法。
    /// 其中点位表名称和地址，仅该驱动能够识别。类型和长度等信息，则由物联网平台统一规范。
    /// </remarks>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public override async Task<ReadResult> ReadAsync(INode node, IPoint[] points, CancellationToken cancellationToken = default)
    {
        var client = Client;
        if (client == null) return ReadResult.Success(points ?? [], new Object?[points?.Length ?? 0]);
        if (node.Parameter is not HttpParameter parameter) return ReadResult.Success(points ?? [], new Object?[points?.Length ?? 0]);

        var path = parameter.PathAndQuery;
        if (path.IsNullOrEmpty()) path = "/";

        // 根据不同场景请求数据
        String? response = null;
        if (parameter.Method.EqualIgnoreCase("GET"))
        {
            response = await client.GetStringAsync(path);
        }
        else
        {
            var str = parameter.PostData;
            if (str.IsNullOrEmpty())
            {
                var rs = await client.PostAsync(path, new StringContent(""), cancellationToken);
                response = await rs.Content.ReadAsStringAsync();
            }
            else
            {
                HttpContent? content;
                if (str.StartsWithIgnoreCase("0x"))
                    content = new ByteArrayContent(str[2..].ToHex());
                else if (str[0] == '{' && str[^1] == '}')
                    content = new StringContent(str, Encoding.UTF8, "application/json");
                else if (str.Contains('='))
                    content = new StringContent(str, Encoding.UTF8, "application/x-www-form-urlencoded");
                else
                    content = new StringContent(str, Encoding.UTF8, "text/plain");

                var rs = await client.PostAsync(path, content, cancellationToken);
                response = await rs.Content.ReadAsStringAsync();
            }
        }

        if (response.IsNullOrEmpty())
            return ReadResult.Success(points ?? [], new Object?[points?.Length ?? 0]);

        // 尝试按照 Json 解析数据，转为字典，然后逐个给点位赋值
        var dic = Decode(response);

        if (dic != null)
        {
            if (points != null && points.Length > 0)
            {
                // 严格按下标填充
                var values = new Object?[points.Length];
                for (var i = 0; i < points.Length; i++)
                {
                    var pt = points[i];
                    var kv = dic.FirstOrDefault(x => x.Key.EqualIgnoreCase(pt.Name, pt.Address));
                    if (kv.Key != null)
                    {
                        var val = kv.Value;
                        var type = pt.GetNetType();
                        values[i] = type != null ? val.ChangeType(type) : val;
                    }
                }
                return ReadResult.Success(points, values);
            }
            else if (parameter.CaptureAll)
            {
                // 无点位且允许捕获所有：动态建立点位
                var dynPoints = new IPoint[dic.Count];
                var dynValues = new Object?[dic.Count];
                var idx = 0;
                foreach (var item in dic)
                {
                    dynPoints[idx] = new PointModel { Name = item.Key };
                    dynValues[idx] = item.Value;
                    idx++;
                }
                return ReadResult.Success(dynPoints, dynValues);
            }
        }
        else
        {
            // 未解析出字典：将原始响应放入 data 点位
            if (points != null && points.Length > 0)
            {
                var values = new Object?[points.Length];
                var dataIdx = Array.FindIndex(points, p => p.Name.EqualIgnoreCase("data"));
                if (dataIdx >= 0) values[dataIdx] = response;
                return ReadResult.Success(points, values);
            }
            return ReadResult.Success(
                [new PointModel { Name = "data" }],
                [response]);
        }

        return ReadResult.Success([], []);
    }

    /// <summary>解码字符串</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected virtual IDictionary<String, Object?>? Decode(String value)
    {
        if (value.IsNullOrEmpty()) return null;

        var dic = JsonParser.Decode(value) ?? XmlParser.Decode(value);
        if (dic == null) return null;

        // 内嵌data
        if (dic.TryGetValue("data", out var data) && data is IDictionary<String, Object?> dic2) return dic2;

        return dic;
    }

    #endregion
}