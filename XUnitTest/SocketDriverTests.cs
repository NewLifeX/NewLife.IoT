using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NewLife.IoT;
using NewLife.IoT.Drivers;
using NewLife.IoT.Models;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using NewLife.IoTSocket.Drivers;
using NewLife.IoTSocket.Transport;
using NewLife.Net;
using Xunit;

namespace XUnitTest;

/// <summary>网络驱动真实网络测试。覆盖 NET-1/NET-2/NET-3/NET-4/NET-5a/NET-5b/NET-5c/NET-5d</summary>
public class SocketDriverTests
{
    #region 辅助

    private class MiniDevice : IDevice
    {
        public String Code { get; set; } = "test-device";
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

    /// <summary>在本地启动 TCP 响应服务器：接受→读取→写入响应→关闭</summary>
    private static TcpListener StartTcpResponder(Int32 port, Byte[] responseData)
    {
        var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();

        _ = Task.Run(async () =>
        {
            try
            {
                using var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                using var stream = client.GetStream();
                var buffer = new Byte[4096];
                var read = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                if (read > 0)
                {
                    await Task.Delay(50).ConfigureAwait(false);
                    if (responseData.Length > 0)
                        await stream.WriteAsync(responseData, 0, responseData.Length).ConfigureAwait(false);
                }
            }
            catch { }
        });

        return listener;
    }

    /// <summary>在本地启动 UDP 响应服务器：接收→发送响应</summary>
    private static UdpClient StartUdpResponder(Int32 port, Byte[] responseData)
    {
        var server = new UdpClient(new IPEndPoint(IPAddress.Loopback, port));

        _ = Task.Run(async () =>
        {
            try
            {
                var result = await server.ReceiveAsync().ConfigureAwait(false);
                await Task.Delay(50).ConfigureAwait(false);
                await server.SendAsync(responseData, responseData.Length, result.RemoteEndPoint).ConfigureAwait(false);
            }
            catch { }
        });

        return server;
    }

    /// <summary>在本地启动 HTTP 响应服务器</summary>
    private static TcpListener StartHttpResponder(Int32 port)
    {
        var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();

        _ = Task.Run(async () =>
        {
            try
            {
                using var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.ASCII);
                await reader.ReadLineAsync();
                String? line;
                while ((line = await reader.ReadLineAsync()) != null && line != "") { }

                var body = """{"Data":"ok"}""";
                var response = $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {body.Length}\r\nConnection: close\r\n\r\n{body}";
                var bytes = Encoding.ASCII.GetBytes(response);
                await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            }
            catch { }
        });

        return listener;
    }

    /// <summary>找空闲端口</summary>
    private static Int32 GetFreePort()
    {
        using var l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        var port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        return port;
    }

    #endregion

    // ================================================================
    // NET-1 IoTTcpDriver
    // ================================================================

    [Fact]
    public async Task IoTTcpDriver_OpenClose_RealTcp()
    {
        var port = GetFreePort();
        var listener = StartTcpResponder(port, "OK"u8.ToArray());
        try
        {
            var driver = new IoTTcpDriver();
            var param = new SocketParameter
            {
                Server = "127.0.0.1",
                Port = port,
                RequestCommand = "Hello",
                Timeout = 5000,
            };
            var node = await driver.OpenAsync(new MiniDevice(), param);
            Assert.NotNull(node);
            Assert.IsType<SocketNode>(node);
            await driver.CloseAsync(node);
        }
        finally
        {
            listener.Stop();
        }
    }

    [Fact]
    public void IoTTcpDriver_GetSpecification()
    {
        var driver = new IoTTcpDriver();
        var spec = driver.GetSpecification();
        Assert.NotNull(spec);
        Assert.Equal("IoTTcp", spec.Profile.ProductKey);
        Assert.NotNull(spec.Properties);
        Assert.Single(spec.Properties!);
    }

    // ================================================================
    // NET-2 IoTUdpDriver
    // ================================================================

    [Fact]
    public async Task IoTUdpDriver_OpenClose_RealUdp()
    {
        var port = GetFreePort();
        var server = StartUdpResponder(port, "OK"u8.ToArray());
        try
        {
            var driver = new IoTUdpDriver();
            var param = new SocketParameter
            {
                Server = "127.0.0.1",
                Port = port,
                RequestCommand = "Hello",
                Timeout = 5000,
            };
            var node = await driver.OpenAsync(new MiniDevice(), param);
            Assert.NotNull(node);
            Assert.IsType<SocketNode>(node);
            await driver.CloseAsync(node);
        }
        finally
        {
            server.Dispose();
        }
    }

    [Fact]
    public void IoTUdpDriver_GetSpecification()
    {
        var driver = new IoTUdpDriver();
        var spec = driver.GetSpecification();
        Assert.NotNull(spec);
        Assert.Equal("IoTUdp", spec.Profile.ProductKey);
    }

    [Fact]
    public void IoTUdpDriver_Implements_IDiscoverableDriver()
    {
        Assert.IsAssignableFrom<IDiscoverableDriver>(new IoTUdpDriver());
    }

    // ================================================================
    // NET-3 IoTHttpDriver
    // ================================================================

    [Fact]
    public async Task IoTHttpDriver_OpenClose_RealHttp()
    {
        var port = GetFreePort();
        var listener = StartHttpResponder(port);
        try
        {
            var driver = new IoTHttpDriver();
            var param = new HttpParameter
            {
                Address = $"http://127.0.0.1:{port}",
                Method = "GET",
                Timeout = 5000,
            };
            var node = await driver.OpenAsync(new MiniDevice(), param);
            Assert.NotNull(node);

            var result = await driver.ReadAsync(node, []);
            Assert.NotNull(result);

            await driver.CloseAsync(node);
        }
        finally
        {
            listener.Stop();
        }
    }

    [Fact]
    public void IoTHttpDriver_GetSpecification()
    {
        var driver = new IoTHttpDriver();
        // IoTHttpDriver 未重写 OnGetSpecification，基类返回 null
        var spec = driver.GetSpecification();
        Assert.Null(spec);
    }

    // ================================================================
    // NET-4 IoTSocketDriver (abstract - tested via IoTTcpDriver)
    // ================================================================

    [Fact]
    public async Task IoTSocketDriver_AbstractBase_OpenAndClose()
    {
        var port = GetFreePort();
        using var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();

        var driver = new IoTTcpDriver();
        var param = new SocketParameter
        {
            Server = "127.0.0.1",
            Port = port,
            Timeout = 1000,
        };
        var node = await driver.OpenAsync(new MiniDevice(), param);

        Assert.NotNull(node);
        Assert.Same(driver, node.Driver);
        Assert.NotNull(node.Device);
        Assert.Same(param, node.Parameter);
        Assert.True(node is SocketNode);

        await driver.CloseAsync(node);
        listener.Stop();
    }

    [Fact]
    public void IoTSocketDriver_GetSpecification()
    {
        var driver = new IoTTcpDriver();
        var spec = driver.GetSpecification();
        Assert.NotNull(spec);
        Assert.NotNull(spec.Profile);
        Assert.NotNull(spec.Properties);
    }

    // ================================================================
    // NET-5a TcpTransport
    // ================================================================

    [Fact]
    public void TcpTransport_DefaultProperties()
    {
        var transport = new TcpTransport();
        Assert.Equal("127.0.0.1", transport.Server);
        Assert.Equal(5500, transport.Port);
        Assert.Equal(3000, transport.Timeout);
        Assert.False(transport.IsConnected);
    }

    [Fact]
    public void TcpTransport_OpenClose()
    {
        var port = GetFreePort();
        var listener = StartTcpResponder(port, []);
        try
        {
            using var transport = new TcpTransport
            {
                Server = "127.0.0.1",
                Port = port,
                Timeout = 5000,
            };
            transport.Open();
            Assert.True(transport.IsConnected);
            transport.Close();
        }
        finally
        {
            listener.Stop();
        }
    }

    // ================================================================
    // NET-5b UdpTransport
    // ================================================================

    [Fact]
    public void UdpTransport_DefaultProperties()
    {
        var transport = new UdpTransport();
        Assert.Equal("127.0.0.1", transport.Server);
        Assert.Equal(5500, transport.Port);
        Assert.Equal(3000, transport.Timeout);
    }

    [Fact]
    public void UdpTransport_OpenClose()
    {
        using var transport = new UdpTransport
        {
            Server = "127.0.0.1",
            Port = 0,
            Timeout = 1000,
        };
        transport.Open();
        Assert.True(transport.IsConnected);
        transport.Close();
    }

    // ================================================================
    // NET-5c HttpTransport
    // ================================================================

    [Fact]
    public void HttpTransport_DefaultProperties()
    {
        var transport = new HttpTransport();
        Assert.Equal(5000, transport.Timeout);
        Assert.Equal("POST", transport.Method);
        Assert.False(transport.IsConnected);
    }

    [Fact]
    public void HttpTransport_OpenClose()
    {
        var transport = new HttpTransport
        {
            Address = "http://127.0.0.1:12345",
            Timeout = 1000,
        };
        transport.Open();
        Assert.True(transport.IsConnected);
        transport.Close();
    }

    // ================================================================
    // NET-5d SocketTransport (abstract - tested via TcpTransport)
    // ================================================================

    [Fact]
    public void SocketTransport_Abstract_Defaults()
    {
        var transport = new TcpTransport
        {
            Server = "10.0.0.1",
            Port = 502,
            Timeout = 5000,
        };
        Assert.Equal("10.0.0.1", transport.Server);
        Assert.Equal(502, transport.Port);
        Assert.Equal(5000, transport.Timeout);
        Assert.False(transport.IsConnected);
    }

    [Fact]
    public void SocketTransport_Dispose_NotOpen_NoException()
    {
        var transport = new TcpTransport();
        transport.Dispose();
        Assert.True(true);
    }
}
