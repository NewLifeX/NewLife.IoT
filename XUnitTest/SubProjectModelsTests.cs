using System;
using System.IO.Ports;
using NewLife.IoT.Drivers;
using NewLife.IoTDatabase.Drivers;
using NewLife.IoTSerial.Drivers;
using NewLife.IoTSocket.Drivers;
using Xunit;

namespace XUnitTest;

/// <summary>子项目数据模型测试 — SER-3a~c / NET-6a~c / DB-2a~b</summary>
public class SubProjectModelsTests
{
    #region SER-3a: IoTSerialParameter

    [Fact]
    public void IoTSerialParameter_DefaultValues()
    {
        var p = new IoTSerialParameter();
        Assert.Equal("COM1", p.PortName);
        Assert.Equal(9600, p.Baudrate);
        Assert.Equal(8, p.DataBits);
        Assert.Equal(Parity.None, p.Parity);
        Assert.Equal(StopBits.One, p.StopBits);
        Assert.Equal(3000, p.Timeout);
        Assert.Equal("", p.RequestCommand);
        Assert.Equal("HEX", p.ResponseEncoding);
        Assert.False(p.CaptureAll);
        Assert.IsAssignableFrom<SerialParameter>(p);
        Assert.IsAssignableFrom<IDriverParameter>(p);
        Assert.IsAssignableFrom<IDriverParameterKey>(p);
    }

    [Fact]
    public void IoTSerialParameter_SetProperties()
    {
        var p = new IoTSerialParameter
        {
            PortName = "COM3",
            Baudrate = 115200,
            DataBits = 7,
            Parity = Parity.Even,
            StopBits = StopBits.OnePointFive,
            Timeout = 5000,
            RequestCommand = "0x010300000001",
            ResponseEncoding = "ASCII",
            CaptureAll = true,
        };
        Assert.Equal("COM3", p.PortName);
        Assert.Equal(115200, p.Baudrate);
        Assert.Equal(7, p.DataBits);
        Assert.Equal(Parity.Even, p.Parity);
        Assert.Equal(5000, p.Timeout);
        Assert.Equal("0x010300000001", p.RequestCommand);
        Assert.Equal("ASCII", p.ResponseEncoding);
        Assert.True(p.CaptureAll);
    }

    [Fact]
    public void IoTSerialParameter_GetKey_ReturnsPortName()
    {
        var p = new IoTSerialParameter { PortName = "COM5" };
        Assert.Equal("COM5", p.GetKey());
    }

    #endregion

    #region SER-3b: SerialNode

    [Fact]
    public void SerialNode_DefaultValues()
    {
        var n = new SerialNode();
        Assert.Null(n.SerialPort);
        Assert.Null(n.SerialParameter);
        Assert.True(n.IsConnected);
        Assert.IsAssignableFrom<Node>(n);
        Assert.IsAssignableFrom<INode>(n);
    }

    #endregion

    #region SER-3c: SerialParameter

    [Fact]
    public void SerialParameter_DefaultValues()
    {
        var p = new SerialParameter();
        Assert.Equal("COM1", p.PortName);
        Assert.Equal(9600, p.Baudrate);
        Assert.Equal(8, p.DataBits);
        Assert.Equal(3000, p.Timeout);
    }

    [Fact]
    public void SerialParameter_ImplementsInterfaces()
    {
        var p = new SerialParameter();
        Assert.IsAssignableFrom<IDriverParameter>(p);
        Assert.IsAssignableFrom<IDriverParameterKey>(p);
    }

    #endregion

    #region NET-6a: SocketParameter

    [Fact]
    public void SocketParameter_DefaultValues()
    {
        var p = new SocketParameter();
        Assert.Equal("127.0.0.1", p.Server);
        Assert.Equal(5500, p.Port);
        Assert.Equal(3000, p.Timeout);
        Assert.Equal("", p.RequestCommand);
        Assert.Equal("HEX", p.ResponseEncoding);
        Assert.IsAssignableFrom<IDriverParameter>(p);
        Assert.IsAssignableFrom<IDriverParameterKey>(p);
    }

    [Fact]
    public void SocketParameter_SetProperties()
    {
        var p = new SocketParameter
        {
            Server = "192.168.1.100",
            Port = 502,
            Timeout = 5000,
            RequestCommand = "0x010300000001",
            ResponseEncoding = "UTF8",
        };
        Assert.Equal("192.168.1.100", p.Server);
        Assert.Equal(502, p.Port);
        Assert.Equal(5000, p.Timeout);
        Assert.Contains("0103", p.RequestCommand);
        Assert.Equal("UTF8", p.ResponseEncoding);
    }

    [Fact]
    public void SocketParameter_GetKey_ReturnsServerPort()
    {
        var p = new SocketParameter { Server = "10.0.0.1", Port = 502 };
        Assert.Equal("10.0.0.1:502", p.GetKey());
    }

    #endregion

    #region NET-6b: HttpParameter

    [Fact]
    public void HttpParameter_DefaultValues()
    {
        var p = new HttpParameter();
        Assert.Null(p.Address);
        Assert.Equal("GET", p.Method);
        Assert.Null(p.PathAndQuery);
        Assert.Null(p.Token);
        Assert.Equal(5000, p.Timeout);
        Assert.IsAssignableFrom<IDriverParameter>(p);
    }

    [Fact]
    public void HttpParameter_SetProperties()
    {
        var p = new HttpParameter
        {
            Address = "http://star.newlifex.com:6600",
            Method = "POST",
            PathAndQuery = "/api/device/report",
            Token = "Bearer xxx",
            Timeout = 10000,
        };
        Assert.Equal("http://star.newlifex.com:6600", p.Address);
        Assert.Equal("POST", p.Method);
        Assert.Equal("/api/device/report", p.PathAndQuery);
        Assert.Equal("Bearer xxx", p.Token);
        Assert.Equal(10000, p.Timeout);
    }

    #endregion

    #region NET-6c: SocketNode

    [Fact]
    public void SocketNode_DefaultValues()
    {
        var n = new SocketNode();
        Assert.Null(n.Client);
        Assert.Null(n.SocketParameter);
        Assert.True(n.IsConnected);
        Assert.IsAssignableFrom<Node>(n);
        Assert.IsAssignableFrom<INode>(n);
    }

    #endregion

    #region DB-2a: DatabaseParameter

    [Fact]
    public void DatabaseParameter_DefaultValues()
    {
        var p = new DatabaseParameter();
        Assert.Null(p.ConnectionString);
        Assert.Null(p.QuerySql);
        Assert.False(p.CaptureAll);
        Assert.IsAssignableFrom<IDriverParameter>(p);
        Assert.IsAssignableFrom<IDriverParameterKey>(p);
    }

    [Fact]
    public void DatabaseParameter_SetProperties()
    {
        var p = new DatabaseParameter
        {
            ConnectionString = "server=.;database=iot;user=iot;password=iot",
            QuerySql = "SELECT * FROM Points",
            CaptureAll = true,
        };
        Assert.Contains("database=iot", p.ConnectionString);
        Assert.Equal("SELECT * FROM Points", p.QuerySql);
        Assert.True(p.CaptureAll);
    }

    [Fact]
    public void DatabaseParameter_GetKey_ReturnsConnectionString()
    {
        var p = new DatabaseParameter { ConnectionString = "server=localhost;database=test" };
        Assert.Equal("server=localhost;database=test", p.GetKey());
    }

    #endregion

    #region DB-2b: DatabseNode

    [Fact]
    public void DatabseNode_DefaultValues()
    {
        var n = new DatabseNode();
        Assert.Null(n.Dal);
        Assert.Null(n.DatabaseParameter);
        Assert.True(n.IsConnected);
        Assert.IsAssignableFrom<Node>(n);
        Assert.IsAssignableFrom<INode>(n);
    }

    #endregion
}
