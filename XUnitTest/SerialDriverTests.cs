using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NewLife.IoT;
using NewLife.IoT.Controllers;
using NewLife.IoT.Drivers;
using NewLife.IoT.Models;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using NewLife.IoTSerial.Drivers;
using NewLife.IoTSerial.Transport;
using Xunit;

namespace XUnitTest;

/// <summary>串口驱动测试。覆盖 SER-1/SER-2/SER-3a/SER-3b/SER-3c（补充测试）</summary>
public class SerialDriverTests
{
    #region 辅助

    private class MiniDevice : IDevice
    {
        public String Code { get; set; } = "serial-device";
        public IDictionary<String, Object?> Properties { get; } = new Dictionary<String, Object?>();
        public ThingSpec? Specification { get; set; }
        public IPoint[]? Points { get; set; }
        public IDictionary<String, Delegate> Services { get; } = new Dictionary<String, Delegate>();

        public Task StartAsync(CancellationToken ct) => Task.CompletedTask;
        public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
        public Task<IDeviceInfo[]> SetOnlineAsync(IDeviceInfo[] d, CancellationToken ct) => Task.FromResult(d);
        public Task<IDeviceInfo[]> SetOfflineAsync(String[] d, CancellationToken ct) => Task.FromResult(Array.Empty<IDeviceInfo>());
        public Task<Int32> PostPropertyAsync(String code, Object items, CancellationToken ct) => Task.FromResult(0);
        public Task<Int32> PostDataAsync(String code, DataModel[] items, CancellationToken ct) => Task.FromResult(0);
        public Task<Int32> PostEventAsync(String code, EventModel[] items, CancellationToken ct) => Task.FromResult(0);
        public void PostProperty() { }
        public void SetProperty(String name, Object? value) { }
        public Boolean AddData(String name, String value) => true;
        public Boolean WriteEvent(String type, String name, String remark) => true;
        public void RegisterService(String service, Delegate method) { }
    }

    /// <summary>派生自 IoTSerialDriver，用 Mock ISerialPort 替代真实串口</summary>
    private class TestSerialDriver : IoTSerialDriver
    {
        public ISerialPort MockSerialPort { get; set; } = null!;

        protected override ISerialPort CreateSerial(SerialParameter parameter)
        {
            MockSerialPort.PortName = parameter.PortName;
            MockSerialPort.Baudrate = parameter.Baudrate;
            MockSerialPort.Timeout = parameter.Timeout;
            return MockSerialPort;
        }
    }

    /// <summary>派生自 SerialTransport，用 Mock ISerialPort 替代真实串口</summary>
    private class TestSerialTransport : SerialTransport
    {
        public ISerialPort MockSerialPort { get; set; } = null!;

        protected override ISerialPort CreateSerial()
        {
            return MockSerialPort;
        }
    }

    #endregion

    // ===== SER-1 IoTSerialDriver =====

    [Fact]
    public void IoTSerialDriver_Attribute()
    {
        var attr = typeof(IoTSerialDriver).GetCustomAttributes(typeof(DriverAttribute), false);
        Assert.Single(attr);
        Assert.Equal("IoTSerial", (attr[0] as DriverAttribute)!.Name);
    }

    [Fact]
    public void IoTSerialDriver_GetSpecification()
    {
        var driver = new IoTSerialDriver();
        var spec = driver.GetSpecification();
        Assert.NotNull(spec);
        Assert.NotNull(spec.Profile);
        Assert.Equal("IoTSerial", spec.Profile.ProductKey);
        Assert.NotNull(spec.Properties);
        Assert.Single(spec.Properties!);
    }

    [Fact]
    public async Task IoTSerialDriver_OpenAsync_WithMockSerial()
    {
        var mockSerial = new Mock<ISerialPort>();
        mockSerial.SetupProperty(m => m.PortName, "COM1");
        mockSerial.SetupProperty(m => m.Baudrate, 9600);
        mockSerial.SetupProperty(m => m.Timeout, 3000);

        var driver = new TestSerialDriver { MockSerialPort = mockSerial.Object };
        var param = new IoTSerialParameter
        {
            PortName = "COM3",
            Baudrate = 115200,
        };
        var device = new MiniDevice();
        var node = await driver.OpenAsync(device, param);

        Assert.NotNull(node);
        Assert.Same(driver, node.Driver);
        Assert.Same(device, node.Device);
        Assert.Same(param, node.Parameter);

        // 验证 Mock 串口属性已设置
        Assert.Equal("COM3", mockSerial.Object.PortName);
        Assert.Equal(115200, mockSerial.Object.Baudrate);

        await driver.CloseAsync(node);
    }

    [Fact]
    public async Task IoTSerialDriver_OpenAsync_NullParameter_Throws()
    {
        var driver = new IoTSerialDriver();
        var device = new MiniDevice();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            driver.OpenAsync(device, null!));
    }

    [Fact]
    public async Task IoTSerialDriver_ReadAsync_WithMockSerial()
    {
        var mockSerial = new Mock<ISerialPort>();
        // Invoke 返回模拟的响应数据
        var responseData = new NewLife.Data.ArrayPacket("World"u8.ToArray());
        mockSerial.Setup(m => m.Invoke(It.IsAny<NewLife.Data.IPacket>(), It.IsAny<Int32>()))
                  .Returns(responseData);

        var driver = new TestSerialDriver { MockSerialPort = mockSerial.Object };
        var param = new IoTSerialParameter
        {
            PortName = "COM1",
            RequestCommand = "Hello",
            ResponseEncoding = "ASCII",
        };
        var device = new MiniDevice();
        var node = await driver.OpenAsync(device, param);
        Assert.NotNull(node);

        var points = new IPoint[] { new PointModel { Name = "Data", Type = "String" } };
        var result = await driver.ReadAsync(node, points);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("World", result.Values?[0]);

        await driver.CloseAsync(node);
    }

    // ===== SER-2 SerialTransport =====

    [Fact]
    public void SerialTransport_DefaultProperties()
    {
        using var transport = new SerialTransport();
        Assert.Equal("COM1", transport.PortName);
        Assert.Equal(9600, transport.Baudrate);
        Assert.Equal(3000, transport.Timeout);
        Assert.Equal(8, transport.DataBits);
        Assert.Equal(10, transport.ByteTimeout);
        Assert.False(transport.IsConnected);
    }

    [Fact]
    public void SerialTransport_OpenClose_WithMockSerial()
    {
        var mockSerial = new Mock<ISerialPort>();
        mockSerial.Setup(m => m.Open());
        mockSerial.Setup(m => m.Close());

        var transport = new TestSerialTransport { MockSerialPort = mockSerial.Object };
        transport.Open();
        Assert.True(transport.IsConnected);

        transport.Close();
        mockSerial.Verify(m => m.Open(), Times.Once);
    }

    [Fact]
    public void SerialTransport_SendReceive_WithMockSerial()
    {
        var mockSerial = new Mock<ISerialPort>();
        var responseData = new NewLife.Data.ArrayPacket("Echo"u8.ToArray());
        mockSerial.Setup(m => m.Invoke(It.IsAny<NewLife.Data.IPacket>(), It.IsAny<Int32>()))
                  .Returns(responseData);

        var transport = new TestSerialTransport { MockSerialPort = mockSerial.Object };
        transport.Open();
        Assert.True(transport.IsConnected);

        var request = "Hello"u8;
        var response = transport.SendReceive(request);
        Assert.NotNull(response);
        Assert.Equal("Echo", System.Text.Encoding.ASCII.GetString(response));

        transport.Close();
    }
}
