using System.ComponentModel;
using System.Text;
using NewLife.Data;
using NewLife.IoT;
using NewLife.IoT.Controllers;
using NewLife.IoT.Drivers;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using NewLife.Model;
using NewLife.Reflection;
using NewLife.Serialization;
#if NET45
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace NewLife.IoTSerial.Drivers;

/// <summary>IoT标准通用串口驱动</summary>
/// <remarks>
/// IoT驱动，符合IoT标准的通用串口驱动，连接后向目标发送数据即可收到数据。
/// </remarks>
[Driver("IoTSerial")]
[DisplayName("通用串口驱动")]
public class IoTSerialDriver : DriverBase<SerialNode, IoTSerialParameter>
{
    #region 属性
    private Int32 _nodes;
    private ISerialPort? _serialPort;
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
        if (parameter is not IoTSerialParameter p) throw new ArgumentException("参数不能为空");
        if (p.PortName.IsNullOrEmpty()) throw new ArgumentException("串口名称不能为空");

        if (p.Baudrate <= 0) p.Baudrate = 9600;

        var node = new SerialNode
        {
            Driver = this,
            Device = device,
            Parameter = p,
            SerialParameter = p,
        };

        if (_serialPort == null)
        {
            lock (this)
            {
                _serialPort ??= CreateSerial(p);
            }
        }

        node.SerialPort = _serialPort;

        Interlocked.Increment(ref _nodes);

        return Task.FromResult(node as INode)!;
    }

    /// <summary>创建串口</summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    protected virtual ISerialPort CreateSerial(SerialParameter parameter)
    {
        var board = ServiceProvider?.GetService<IBoard>();
        var serialPort = board?.CreateSerial(parameter.PortName, parameter.Baudrate);

        // 创建串口对象
        if (serialPort == null)
        {
            // 借助IBoard服务获取串口映射名，在A2工业计算机中，可使用COM1替代/dev/ttyAMA0
            var portName = parameter.PortName;
            if (board != null)
            {
                var portName2 = board.Map(portName);
                if (!portName2.IsNullOrEmpty()) portName = portName2;
            }

            serialPort = ServiceProvider?.GetService<ISerialPort>() ?? new DefaultSerialPort();
            serialPort.PortName = portName;
            serialPort.Baudrate = parameter.Baudrate;
        }
        serialPort.Timeout = parameter.Timeout;

        if (serialPort is DefaultSerialPort sp)
        {
            sp.DataBits = parameter.DataBits;
            sp.Parity = parameter.Parity;
            sp.StopBits = parameter.StopBits;
            sp.ByteTimeout = parameter.ByteTimeout;
        }
        else
        {
            serialPort.SetValue("DataBits", parameter.DataBits);
            serialPort.SetValue("Parity", parameter.Parity);
            serialPort.SetValue("StopBits", parameter.StopBits);
            serialPort.SetValue("ByteTimeout", parameter.ByteTimeout);
        }

        return serialPort;
    }

    /// <summary>关闭设备节点</summary>
    public override Task CloseAsync(INode node, CancellationToken cancellationToken = default)
    {
        if (Interlocked.Decrement(ref _nodes) <= 0)
        {
            _serialPort.TryDispose();
            _serialPort = null;
        }

#if NET45
        return TaskEx.FromResult(0);
#else
        return Task.CompletedTask;
#endif
    }

    /// <summary>读取数据</summary>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合，Address属性地址示例：D100、C100、W100、H100</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public override Task<ReadResult> ReadAsync(INode node, IPoint[] points, CancellationToken cancellationToken = default)
    {
        var client = (node as SerialNode)?.SerialPort;
        if (client == null || node.Parameter is not IoTSerialParameter parameter)
            return Task.FromResult(ReadResult.Success(points ?? [], new Object?[points?.Length ?? 0]));

        var request = Encode(parameter.RequestCommand);
        var response = client.Invoke(request, 1);
        var decoded = Decode(response, parameter.ResponseEncoding);

        if (points != null && points.Length > 0)
        {
            var values = new Object?[points.Length];
            if (decoded is IDictionary<String, Object?> dic)
            {
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
            }
            else
            {
                // 单值响应：分配到名为 Data 的点位，没有则第 0 项
                var dataIdx = 0;
                for (var i = 0; i < points.Length; i++)
                {
                    if (points[i].Name.EqualIgnoreCase("Data")) { dataIdx = i; break; }
                }
                values[dataIdx] = decoded;
            }
            return Task.FromResult(ReadResult.Success(points, values));
        }

        // 无输入点位：动态捕获
        var dynPoints = new List<IPoint>();
        var dynValues = new List<Object?>();
        if (decoded is IDictionary<String, Object?> allDic)
        {
            foreach (var item in allDic)
            {
                dynPoints.Add(new PointModel { Name = item.Key });
                dynValues.Add(item.Value);
            }
        }
        else
        {
            dynPoints.Add(new PointModel { Name = "Data" });
            dynValues.Add(decoded);
        }
        return Task.FromResult(ReadResult.Success([.. dynPoints], [.. dynValues]));
    }

    /// <summary>写入数据。requests.Length==1 时单点写入并返回回显值；多点时逐项写入并返回成功计数</summary>
    /// <param name="node">节点对象</param>
    /// <param name="requests">写入请求数组</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>写入结果</returns>
    public override Task<WriteResult> WriteAsync(INode node, WriteRequest[] requests, CancellationToken cancellationToken = default)
    {
        var client = (node as SerialNode)?.SerialPort;
        if (client == null || node.Parameter is not IoTSerialParameter parameter)
            return Task.FromResult(WriteResult.Fail(IoTErrorCode.InvalidParameter, "节点或参数无效"));

        if (requests.Length == 1)
        {
            var encoded = Encode(requests[0].Value);
            var response = client.Invoke(encoded, 1);
            return Task.FromResult(WriteResult.Success(Decode(response, parameter.ResponseEncoding)));
        }

        var count = 0;
        foreach (var req in requests)
        {
            var encoded = Encode(req.Value);
            client.Invoke(encoded, 1);
            count++;
        }
        return Task.FromResult(WriteResult.SuccessBatch(count));
    }

    /// <summary>设备控制</summary>
    /// <param name="node">节点对象</param>
    /// <param name="request">服务调用请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>服务调用结果</returns>
    public override Task<ServiceResult> ControlAsync(INode node, ServiceCall request, CancellationToken cancellationToken = default)
    {
        if (request.ServiceName.IsNullOrEmpty()) throw new NotImplementedException();

        var client = (node as SerialNode)?.SerialPort;
        if (client == null || node.Parameter is not IoTSerialParameter parameter)
            return Task.FromResult(ServiceResult.Fail(IoTErrorCode.InvalidParameter, "节点或参数无效"));

        var result = new Dictionary<String, Object?>();
        foreach (var item in request.Parameters)
        {
            var encoded = Encode(item.Value);
            var response = client.Invoke(encoded, 1);

            // 转换编码
            if (!parameter.ResponseEncoding.IsNullOrEmpty())
                result[item.Key] = Decode(response, parameter.ResponseEncoding);
            else
                result[item.Key] = response;
        }

        return Task.FromResult(ServiceResult.Success(result));
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
            "Json" => data == null ? null : JsonParser.Decode(data.ToStr()),
            _ => data,
        };
    }
    #endregion
}