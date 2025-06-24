using NewLife.Reflection;

namespace NewLife.IoT.Controllers;

/// <summary>板卡接口。约定板卡所具备的一些基础功能</summary>
/// <remarks>
/// 一般工业计算机和各种板卡设备，常用端口是输入输出口和串口，而网络口比较通用，这里不做统一定义。
/// </remarks>
public interface IBoard
{
    /// <summary>创建输出口</summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IOutputPort CreateOutput(String name);

    /// <summary>创建输入口</summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IInputPort CreateInput(String name);

    /// <summary>创建串口</summary>
    /// <param name="portName">串口名，在Windows上一般是COM1/COM3等，在Linux上是串口设备路径，工控Linux也可以把COM1/COM3映射到内部串口</param>
    /// <param name="baudrate">波特率，默认9600</param>
    /// <returns></returns>
    ISerialPort CreateSerial(String portName, Int32 baudrate = 9600);

    /// <summary>创建Modbus</summary>
    /// <param name="portName">串口名，在Windows上一般是COM1/COM3等，在Linux上是串口设备路径，工控Linux也可以把COM1/COM3映射到内部串口</param>
    /// <param name="baudrate">波特率，默认9600</param>
    /// <returns></returns>
    IModbus CreateModbus(String portName, Int32 baudrate = 9600);
}

/// <summary>板卡基类，包含一些端口的默认实现</summary>
public class Board : IBoard
{
    /// <summary>创建输出口</summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual IOutputPort CreateOutput(String name) => new FileOutputPort(name);

    /// <summary>创建输入口</summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual IInputPort CreateInput(String name) => new FileInputPort(name);

    /// <summary>创建串口</summary>
    /// <param name="portName">串口名，在Windows上一般是COM1/COM3等，在Linux上是串口设备路径，工控Linux也可以把COM1/COM3映射到内部串口</param>
    /// <param name="baudrate">波特率，默认9600</param>
    /// <returns></returns>
    public virtual ISerialPort CreateSerial(String portName, Int32 baudrate = 9600)
    {
#if NETFRAMEWORK
        var sp = new DefaultSerialPort
        {
            PortName = portName,
            Baudrate = baudrate,
        };
        return sp;
#else
        var type = "DefaultSerialPort".GetTypeEx();
        if (type != null)
        {
            if (type.CreateInstance() is ISerialPort sp)
            {
                sp.PortName = portName;
                sp.Baudrate = baudrate;

                return sp;
            }
        }
#endif

        throw new NotImplementedException();
    }

    /// <summary>创建Modbus</summary>
    /// <param name="portName">串口名，在Windows上一般是COM1/COM3等，在Linux上是串口设备路径，工控Linux也可以把COM1/COM3映射到内部串口</param>
    /// <param name="baudrate">波特率，默认9600</param>
    /// <returns></returns>
    public virtual IModbus CreateModbus(String portName, Int32 baudrate = 9600) => throw new NotImplementedException();
}