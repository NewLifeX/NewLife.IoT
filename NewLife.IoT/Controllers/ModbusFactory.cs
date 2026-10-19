namespace NewLife.IoT.Controllers;

/// <summary>Modbus 工厂。AOT/裁剪环境下通过委托提供 Modbus 实现，避免运行期类型查找</summary>
/// <remarks>
/// Modbus 实现通常位于 NewLife.Modbus 等独立包中，AOT 应用应在启动时注册
/// <see cref="Factory"/>，例如 <c>ModbusFactory.Factory = (port, baud) => new ModbusRtu { PortName = port, Baudrate = baud };</c>
/// </remarks>
public static class ModbusFactory
{
    /// <summary>创建 Modbus 实例的工厂。参数为串口名与波特率</summary>
    public static Func<String, Int32, IModbus>? Factory { get; set; }
}
