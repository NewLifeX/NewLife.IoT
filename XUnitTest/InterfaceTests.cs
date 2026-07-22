using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NewLife.IoT;
using NewLife.IoT.Controllers;
using NewLife.IoT.Drivers;
using NewLife.IoT.Models;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using Xunit;

namespace XUnitTest;

/// <summary>纯接口 Mock 测试。覆盖 SYS-1/SYS-8/SYS-10/SYS-12a/SYS-12b/DEV-1/CTL-1a/CTL-4</summary>
public class InterfaceTests
{
    #region SYS-1 IDriver

    [Fact]
    public async Task IDriver_Moq_AllMethods()
    {
        var mock = new Mock<IDriver>();

        // 事件
        var fired = false;
        mock.SetupAdd(m => m.DataReceived += It.IsAny<EventHandler<DriverDataEventArgs>>());
        mock.Object.DataReceived += (s, e) => fired = true;

        // CreateParameter
        var param = new Mock<IDriverParameter>().Object;
        mock.Setup(m => m.CreateParameter(It.IsAny<String>())).Returns(param);
        Assert.Same(param, mock.Object.CreateParameter("test"));

        // GetSpecification
        var spec = new ThingSpec();
        mock.Setup(m => m.GetSpecification()).Returns(spec);
        Assert.Same(spec, mock.Object.GetSpecification());

        // OpenAsync
        var node = new Mock<INode>().Object;
        mock.Setup(m => m.OpenAsync(It.IsAny<IDevice>(), It.IsAny<IDriverParameter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(node);
        var result = await mock.Object.OpenAsync(Mock.Of<IDevice>(), null);
        Assert.Same(node, result);

        // CloseAsync
        mock.Setup(m => m.CloseAsync(It.IsAny<INode>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        await mock.Object.CloseAsync(node);

        // ReadAsync
        var readResult = ReadResult.Success([], []);
        mock.Setup(m => m.ReadAsync(It.IsAny<INode>(), It.IsAny<IPoint[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(readResult);
        var rr = await mock.Object.ReadAsync(node, []);
        Assert.NotNull(rr);

        // WriteAsync
        var writeResult = WriteResult.Success(1);
        mock.Setup(m => m.WriteAsync(It.IsAny<INode>(), It.IsAny<WriteRequest[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(writeResult);
        var wr = await mock.Object.WriteAsync(node, []);
        Assert.NotNull(wr);

        // ControlAsync
        var controlResult = ControlResult.Success(new Dictionary<String, Object?>());
        mock.Setup(m => m.ControlAsync(It.IsAny<INode>(), It.IsAny<ControlRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(controlResult);
        var cr = await mock.Object.ControlAsync(node, new ControlRequest());
        Assert.NotNull(cr);

        // Verify all setups were called
        mock.VerifyAll();
    }

    #endregion

    #region SYS-8 IDiscoverableDriver

    [Fact]
    public async Task IDiscoverableDriver_Moq()
    {
        var mock = new Mock<IDiscoverableDriver>();
        var devices = new[] { Mock.Of<IDeviceInfo>(d => d.Code == "dev1") };
        mock.Setup(m => m.DiscoverAsync(It.IsAny<Dictionary<String, Object>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(devices);

        var result = await mock.Object.DiscoverAsync(new Dictionary<String, Object>());
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("dev1", result.First().Code);
    }

    #endregion

    #region SYS-10 ITransport

    [Fact]
    public void ITransport_Moq_Properties()
    {
        var mock = new Mock<ITransport>();

        mock.SetupProperty(m => m.Timeout, 5000);
        mock.SetupProperty(m => m.Tracer);
        mock.SetupProperty(m => m.Log);

        Assert.Equal(5000, mock.Object.Timeout);
    }

    [Fact]
    public void ITransport_Moq_AllMethods()
    {
        var mock = new Mock<ITransport>();

        mock.Setup(m => m.IsConnected).Returns(true);
        Assert.True(mock.Object.IsConnected);

        mock.Setup(m => m.Open());
        mock.Object.Open();

        mock.Setup(m => m.Close());
        mock.Object.Close();

        // Send/SendReceive 使用 ReadOnlySpan<Byte> (ref struct)，
        // Moq 无法为其生成有效代理，运行时抛 InvalidProgramException。
        // 这些方法在具体实现测试中覆盖（见 TransportTests.cs）

        var response = new Byte[] { 0x02, 0x03 };
        mock.Setup(m => m.Receive(It.IsAny<Int32>())).Returns(response);
        var r = mock.Object.Receive();
        Assert.Equal(response, r);

        mock.Verify(m => m.Open(), Times.Once);
        mock.Verify(m => m.Close(), Times.Once);
        mock.Verify(m => m.Receive(It.IsAny<Int32>()), Times.Once);
    }

    #endregion

    #region SYS-12a IDriverParameter (empty marker interface)

    [Fact]
    public void IDriverParameter_CanBeImplemented()
    {
        var p = new Mock<IDriverParameter>().Object;
        Assert.NotNull(p);
        Assert.IsAssignableFrom<IDriverParameter>(p);
    }

    [Fact]
    public void IDriverParameter_CanBeExtended()
    {
        // 保证空标记接口不会影响派生类
        var impl = new TestParameterImpl();
        Assert.IsAssignableFrom<IDriverParameter>(impl);
    }

    private class TestParameterImpl : IDriverParameter { }

    #endregion

    #region SYS-12b IDriverParameterKey

    [Fact]
    public void IDriverParameterKey_Moq_GetKey()
    {
        var mock = new Mock<IDriverParameterKey>();
        mock.Setup(m => m.GetKey()).Returns("my-key");
        Assert.Equal("my-key", mock.Object.GetKey());
    }

    [Fact]
    public void IDriverParameterKey_GetKey_ViaExtension()
    {
        // 使用具体实现而非 Mock，因为 is-check 在 Moq 代理上可能失败
        var p = new KeyedParameterImpl { KeyValue = "ext-key" };

        // 通过 IDriverParameter 扩展方法调用 GetKey
        var result = (p as IDriverParameter).GetKey();
        Assert.Equal("ext-key", result);
    }

    private class KeyedParameterImpl : IDriverParameter, IDriverParameterKey
    {
        public String? KeyValue { get; set; }
        public String GetKey() => KeyValue!;
    }

    #endregion

    #region DEV-1 IDevice

    [Fact]
    public async Task IDevice_Moq_Basics()
    {
        var mock = new Mock<IDevice>();

        mock.SetupProperty(m => m.Code, "device-001");
        mock.SetupProperty(m => m.Specification);
        mock.SetupProperty(m => m.Points);

        Assert.Equal("device-001", mock.Object.Code);
    }

    [Fact]
    public async Task IDevice_Moq_AllMethods()
    {
        var mock = new Mock<IDevice>();

        // StartAsync / StopAsync
        mock.Setup(m => m.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        mock.Setup(m => m.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        await mock.Object.StartAsync();
        await mock.Object.StopAsync();

        // SetOnlineAsync
        var info = Mock.Of<IDeviceInfo>(d => d.Code == "sub-device");
        mock.Setup(m => m.SetOnlineAsync(It.IsAny<IDeviceInfo[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IDeviceInfo[] d, CancellationToken _) => d);
        var online = await mock.Object.SetOnlineAsync([info]);
        Assert.Single(online);

        // SetOfflineAsync
        mock.Setup(m => m.SetOfflineAsync(It.IsAny<String[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var offline = await mock.Object.SetOfflineAsync(["dev-001"]);
        Assert.NotNull(offline);

        // PostPropertyAsync
        mock.Setup(m => m.PostPropertyAsync(It.IsAny<String>(), It.IsAny<Object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var pp = await mock.Object.PostPropertyAsync("dev-001", new { temp = 25 });
        Assert.Equal(1, pp);

        // PostEventAsync
        mock.Setup(m => m.PostEventAsync(It.IsAny<String>(), It.IsAny<EventModel[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var pe = await mock.Object.PostEventAsync("dev-001", [new EventModel { Name = "alert" }]);
        Assert.Equal(1, pe);

        // PostDataAsync
        mock.Setup(m => m.PostDataAsync(It.IsAny<String>(), It.IsAny<DataModel[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var pd = await mock.Object.PostDataAsync("dev-001", [new DataModel { Name = "temp", Value = "25" }]);
        Assert.Equal(1, pd);

        // RegisterService
        var captured = "";
        mock.Setup(m => m.RegisterService(It.IsAny<String>(), It.IsAny<Delegate>()))
            .Callback<String, Delegate>((n, _) => captured = n);
        mock.Object.RegisterService("read", () => "ok");
        Assert.Equal("read", captured);

        // PostProperty (void)
        mock.Setup(m => m.PostProperty());
        mock.Object.PostProperty();

        // SetProperty
        mock.Setup(m => m.SetProperty(It.IsAny<String>(), It.IsAny<Object?>()));
        mock.Object.SetProperty("temp", 25);

        // WriteEvent
        mock.Setup(m => m.WriteEvent(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>())).Returns(true);
        var we = mock.Object.WriteEvent("info", "online", "connected");
        Assert.True(we);

        mock.VerifyAll();
    }

    #endregion

    #region CTL-1a ISerialPort

    [Fact]
    public void ISerialPort_Moq_Properties()
    {
        var mock = new Mock<ISerialPort>();

        mock.SetupProperty(m => m.PortName, "COM1");
        mock.SetupProperty(m => m.Baudrate, 115200);
        mock.SetupProperty(m => m.Timeout, 5000);

        Assert.Equal("COM1", mock.Object.PortName);
        Assert.Equal(115200, mock.Object.Baudrate);
        Assert.Equal(5000, mock.Object.Timeout);
    }

    [Fact]
    public void ISerialPort_Moq_Methods()
    {
        var mock = new Mock<ISerialPort>();

        mock.Setup(m => m.Open());
        mock.Object.Open();

        mock.Setup(m => m.Close());
        mock.Object.Close();

        var buffer = new Byte[256];
        mock.Setup(m => m.Read(It.IsAny<Byte[]>(), It.IsAny<Int32>(), It.IsAny<Int32>()))
            .Returns(10);
        var read = mock.Object.Read(buffer, 0, 256);
        Assert.Equal(10, read);

        mock.Setup(m => m.Write(It.IsAny<Byte[]>(), It.IsAny<Int32>(), It.IsAny<Int32>()));
        mock.Object.Write([0x01, 0x02], 0, 2);

        var packet = new Mock<NewLife.Data.IPacket>().Object;
        mock.Setup(m => m.Invoke(It.IsAny<NewLife.Data.IPacket>(), It.IsAny<Int32>())).Returns(packet);
        var result = mock.Object.Invoke(null!);
        Assert.Same(packet, result);

        mock.VerifyAll();
    }

    [Fact]
    public void ISerialPort_Received_Event()
    {
        var mock = new Mock<ISerialPort>();
        mock.SetupAdd(m => m.Received += It.IsAny<EventHandler<NewLife.Net.ReceivedEventArgs>>());

        mock.Object.Received += (s, e) => { };
        mock.Raise(m => m.Received += null, EventArgs.Empty, new NewLife.Net.ReceivedEventArgs());

        Assert.NotNull(mock.Object);
    }

    #endregion

    #region CTL-4 IModbus

    [Fact]
    public async Task IModbus_Moq_ReadMethods()
    {
        var mock = new Mock<IModbus>();

        // 0x01 ReadCoilAsync
        mock.Setup(m => m.ReadCoilAsync(It.IsAny<Byte>(), It.IsAny<UInt16>(), It.IsAny<UInt16>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([true, false]);
        var coils = await mock.Object.ReadCoilAsync(1, 0, 2);
        Assert.Equal(2, coils.Length);

        // 0x02 ReadDiscreteAsync
        mock.Setup(m => m.ReadDiscreteAsync(It.IsAny<Byte>(), It.IsAny<UInt16>(), It.IsAny<UInt16>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([false, true]);
        var discrete = await mock.Object.ReadDiscreteAsync(1, 0, 2);
        Assert.Equal(2, discrete.Length);

        // 0x03 ReadRegisterAsync
        mock.Setup(m => m.ReadRegisterAsync(It.IsAny<Byte>(), It.IsAny<UInt16>(), It.IsAny<UInt16>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UInt16[] { 100, 200 });
        var regs = await mock.Object.ReadRegisterAsync(1, 0, 2);
        Assert.Equal(new UInt16[] { 100, 200 }, regs);

        // 0x04 ReadInputAsync
        mock.Setup(m => m.ReadInputAsync(It.IsAny<Byte>(), It.IsAny<UInt16>(), It.IsAny<UInt16>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([50]);
        var inputs = await mock.Object.ReadInputAsync(1, 0, 1);
        Assert.Single(inputs);

        mock.VerifyAll();
    }

    [Fact]
    public async Task IModbus_Moq_WriteMethods()
    {
        var mock = new Mock<IModbus>();

        // 0x05 WriteCoilAsync
        mock.Setup(m => m.WriteCoilAsync(It.IsAny<Byte>(), It.IsAny<UInt16>(), It.IsAny<UInt16>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0xFF00);
        var wc = await mock.Object.WriteCoilAsync(1, 0, 0xFF00);
        Assert.Equal(0xFF00, wc);

        // 0x06 WriteRegisterAsync
        mock.Setup(m => m.WriteRegisterAsync(It.IsAny<Byte>(), It.IsAny<UInt16>(), It.IsAny<UInt16>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1234);
        var wr = await mock.Object.WriteRegisterAsync(1, 0, 1234);
        Assert.Equal(1234, wr);

        // 0x0F WriteCoilsAsync
        mock.Setup(m => m.WriteCoilsAsync(It.IsAny<Byte>(), It.IsAny<UInt16>(), It.IsAny<UInt16[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        var wcMulti = await mock.Object.WriteCoilsAsync(1, 0, [0xFF00, 0x0000]);
        Assert.Equal(2, wcMulti);

        // 0x10 WriteRegistersAsync
        mock.Setup(m => m.WriteRegistersAsync(It.IsAny<Byte>(), It.IsAny<UInt16>(), It.IsAny<UInt16[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        var wrMulti = await mock.Object.WriteRegistersAsync(1, 0, [100, 200]);
        Assert.Equal(2, wrMulti);

        mock.VerifyAll();
    }

    #endregion
}
