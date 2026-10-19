using System;
using System.ComponentModel;
using NewLife.IoT.Controllers;
using Xunit;

namespace XUnitTest;

/// <summary>板卡工厂测试。验证 AOT 友好的串口/Modbus 工厂注入路径</summary>
public class BoardFactoryTests
{
    [Fact]
    [DisplayName("板卡串口创建优先使用注册工厂")]
    public void SerialPortFactoryIsUsed()
    {
        var saved = SerialPortFactory.Factory;
        try
        {
            SerialPortFactory.Factory = (name, baudrate) => new DefaultSerialPort
            {
                PortName = name,
                Baudrate = baudrate,
            };

            var sp = new Board().CreateSerial("COM42", 115200);

            var dsp = Assert.IsType<DefaultSerialPort>(sp);
            Assert.Equal("COM42", dsp.PortName);
            Assert.Equal(115200, dsp.Baudrate);
        }
        finally
        {
            SerialPortFactory.Factory = saved;
        }
    }

    [Fact]
    [DisplayName("板卡 Modbus 创建优先使用注册工厂")]
    public void ModbusFactoryIsUsed()
    {
        var saved = ModbusFactory.Factory;
        try
        {
            ModbusFactory.Factory = (name, baudrate) => throw new InvalidOperationException($"{name}:{baudrate}");

            var ex = Assert.Throws<InvalidOperationException>(() => new Board().CreateModbus("COM8", 19200));

            Assert.Equal("COM8:19200", ex.Message);
        }
        finally
        {
            ModbusFactory.Factory = saved;
        }
    }

    [Fact]
    [DisplayName("IoTSerial 模块初始化注册串口工厂")]
    public void IoTSerialRegistersSerialPortFactory()
    {
        // 触发 IoTSerial 类型加载，确保模块初始化器执行
        _ = typeof(NewLife.IoTSerial.Drivers.IoTSerialDriver);

        Assert.NotNull(SerialPortFactory.Factory);
    }
}
