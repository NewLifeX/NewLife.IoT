namespace NewLife.IoT.Controllers;

/// <summary>串口工厂。AOT/裁剪环境下通过委托提供串口实现，避免运行期类型查找</summary>
/// <remarks>
/// NewLife.IoTSerial 模块初始化时自动注册默认实现；
/// 应用也可以覆盖 <see cref="Factory"/> 注入自定义串口，
/// 或注入自定义 <see cref="IBoard"/> 并重写 <see cref="Board.CreateSerial"/>。
/// </remarks>
public static class SerialPortFactory
{
    /// <summary>创建串口实例的工厂。参数为串口名与波特率</summary>
    public static Func<String, Int32, ISerialPort>? Factory { get; set; }
}
