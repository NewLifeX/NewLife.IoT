using System.Runtime.CompilerServices;
using NewLife.IoT.Controllers;

namespace NewLife.IoTSerial;

/// <summary>串口工厂注册。模块加载时向 NewLife.IoT 注册默认串口实现，AOT 下无需运行期类型查找</summary>
internal static class SerialPortFactoryRegistration
{
#pragma warning disable CA2255 // 类库使用模块初始化器注册默认实现，属高级场景
    [ModuleInitializer]
    internal static void Initialize() => SerialPortFactory.Factory = (portName, baudrate) => new DefaultSerialPort
    {
        PortName = portName,
        Baudrate = baudrate,
    };
#pragma warning restore CA2255
}
