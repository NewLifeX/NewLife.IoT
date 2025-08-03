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

namespace NewLife.IoTSerial.Drivers;

/// <summary>IoT标准通用串口驱动</summary>
/// <remarks>
/// IoT驱动，符合IoT标准的通用串口驱动，连接后向目标发送数据即可收到数据。
/// </remarks>
[Driver("IoTSerial")]
[DisplayName("通用串口驱动")]
public class IoTSerialDriver : DriverBase<Node, SerialParameter>
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
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    public override INode Open(IDevice device, IDriverParameter? parameter)
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

        return node;
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
    public override void Close(INode node)
    {
        if (Interlocked.Decrement(ref _nodes) <= 0)
        {
            _serialPort.TryDispose();
            _serialPort = null;
        }
    }

    /// <summary>读取数据</summary>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合，Address属性地址示例：D100、C100、W100、H100</param>
    /// <returns></returns>
    public override IDictionary<String, Object?> Read(INode node, IPoint[] points)
    {
        var result = new Dictionary<String, Object?>();
        if (points == null || points.Length == 0) return result;

        var snode = node as SerialNode;
        if (snode?.SerialPort == null || node.Parameter is not IoTSerialParameter parameter) return result;

        var request = Encode(parameter.RequestCommand);
        var response = snode.SerialPort.Invoke(request, 1);

        result["Data"] = Decode(response, parameter.ResponseEncoding);

        return result;
    }

    /// <summary>写入数据</summary>
    public override Object? Write(INode node, IPoint point, Object? value)
    {
        var snode = node as SerialNode;
        if (snode?.SerialPort == null || node.Parameter is not IoTSerialParameter parameter) return null;

        var request = Encode(value);
        var response = snode.SerialPort.Invoke(request, 1);

        return Decode(response, parameter.ResponseEncoding);
    }

    /// <summary>设备控制</summary>
    /// <param name="node"></param>
    /// <param name="parameters"></param>
    public override Object? Control(INode node, IDictionary<String, Object?> parameters)
    {
        var service = JsonHelper.Convert<ServiceModel>(parameters);
        if (service == null || service.Name.IsNullOrEmpty()) throw new NotImplementedException();

        var sp = _serialPort ?? throw new InvalidOperationException("设备节点无效");

        // 批量操作
        var result = new Dictionary<String, Object?>();
        foreach (var item in parameters)
        {
            var request = Encode(item.Value);
            var response = sp.Invoke(request, 1);

            // 转换编码
            if (node is SerialNode { Parameter: IoTSerialParameter p } && !p.ResponseEncoding.IsNullOrEmpty())
                result[item.Key] = Decode(response, p.ResponseEncoding);
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