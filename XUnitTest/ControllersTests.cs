using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NewLife.IoT.Controllers;
using Xunit;

namespace XUnitTest;

public class ControllersTests
{
    #region DefaultSerialPort

    [Fact]
    public void DefaultSerialPort_DefaultProperties()
    {
        var sp = new DefaultSerialPort();
        Assert.Equal(9600, sp.Baudrate);
        Assert.Equal(3000, sp.Timeout);
    }

    [Fact]
    public void DefaultSerialPort_SetProperties()
    {
        var sp = new DefaultSerialPort
        {
            PortName = "COM3",
            Baudrate = 115200,
            DataBits = 7,
            Timeout = 5000,
            ByteTimeout = 20,
            BufferSize = 512
        };
        Assert.Equal("COM3", sp.PortName);
        Assert.Equal(115200, sp.Baudrate);
        Assert.Equal(7, sp.DataBits);
        Assert.Equal(5000, sp.Timeout);
    }

    [Fact]
    public void DefaultSerialPort_RaisesDisposeWithoutOpen()
    {
        var sp = new DefaultSerialPort();
        sp.Dispose();
        Assert.True(true); // No exception = pass
    }

    #endregion

    #region Board

    [Fact]
    public void Board_Map_ReturnsSameInput()
    {
        var board = new Board();
        Assert.Equal("COM1", board.Map("COM1"));
        Assert.Equal("/dev/ttyS0", board.Map("/dev/ttyS0"));
    }

    [Fact]
    public void Board_CreateInput_ReturnsFileInputPort()
    {
        var board = new Board();
        var port = board.CreateInput("test");
        Assert.NotNull(port);
        Assert.IsType<FileInputPort>(port);
    }

    [Fact]
    public void Board_CreateOutput_ReturnsFileOutputPort()
    {
        var board = new Board();
        var port = board.CreateOutput("test");
        Assert.NotNull(port);
        Assert.IsType<FileOutputPort>(port);
    }

    #endregion

    #region FileInputPort

    [Fact]
    public void FileInputPort_DefaultProperties()
    {
        var port = new FileInputPort("test.txt");
        Assert.EndsWith("test.txt", port.FileName);
        Assert.Equal(100, port.Period);
    }

    [Fact]
    public void FileInputPort_CanDispose()
    {
        var port = new FileInputPort("test.txt");
        port.Dispose();
        Assert.True(true); // No exception
    }

    #endregion

    #region FileOutputPort

    [Fact]
    public void FileOutputPort_DefaultProperties()
    {
        var port = new FileOutputPort("test.txt");
        Assert.EndsWith("test.txt", port.FileName);
    }

    [Fact]
    public void FileOutputPort_CanDispose()
    {
        var port = new FileOutputPort("test.txt");
        port.Dispose();
        Assert.True(true);
    }

    #endregion

    #region KeyEventArgs

    [Fact]
    public void KeyEventArgs_DefaultValues()
    {
        var e = new KeyEventArgs(true);
        Assert.True(e.Value);
        Assert.False(e.Handled);
    }

    [Fact]
    public void KeyEventArgs_SetHandled()
    {
        var e = new KeyEventArgs(false) { Handled = true };
        Assert.False(e.Value);
        Assert.True(e.Handled);
    }

    #endregion

    #region RelayController

    [Fact]
    public void RelayController_DefaultProperties()
    {
        var ctrl = new RelayController();
        Assert.Equal(1, ctrl.Host);
        Assert.Equal(0x0000, ctrl.StartAddress);
        Assert.Equal(8, ctrl.Count);
    }

    [Fact]
    public async Task RelayController_WriteAsync_DelegatesToModbus()
    {
        var mockModbus = new Mock<IModbus>();
        mockModbus.Setup(m => m.WriteCoilAsync(It.IsAny<Byte>(), It.IsAny<UInt16>(), It.IsAny<UInt16>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(1);

        var ctrl = new RelayController { Modbus = mockModbus.Object };
        await ctrl.WriteAsync(3, true);

        mockModbus.Verify(m => m.WriteCoilAsync(0x01, (UInt16)0x0003, (UInt16)0xFF00, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RelayController_ReadAsync_DelegatesToModbus()
    {
        var mockModbus = new Mock<IModbus>();
        mockModbus.Setup(m => m.ReadCoilAsync(It.IsAny<Byte>(), It.IsAny<UInt16>(), It.IsAny<UInt16>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new Boolean[] { true });

        var ctrl = new RelayController { Modbus = mockModbus.Object };
        var result = await ctrl.ReadAsync(0);

        Assert.True(result);
        mockModbus.Verify(m => m.ReadCoilAsync(0x01, ctrl.StartAddress, 1, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
