using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NewLife.IoT;
using NewLife.IoT.Drivers;
using NewLife.IoT.ThingModels;
using NewLife.IoTSocket.Drivers;
using NewLife.Security;
using NewLife.Serialization;
using Xunit;

namespace XUnitTest.Drivers;

/// <summary>IoTHttpDriver 单元测试。使用本地假 HttpClient 处理器，避免依赖外部网络</summary>
public class IoTHttpDriverTests
{
    private sealed class FakeHttpDriver : IoTHttpDriver
    {
        protected override HttpClient CreateClient(HttpParameter parameter) => new(new FakeHttpHandler())
        {
            BaseAddress = new Uri(parameter.Address),
            Timeout = TimeSpan.FromMilliseconds(parameter.Timeout),
        };
    }

    private sealed class FakeHttpHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var state = "";
            if (request.Content != null)
            {
                var body = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                foreach (var item in body.Split('&'))
                {
                    var ss = item.Split('=');
                    if (ss.Length == 2 && ss[0] == "state") state = ss[1];
                }
            }

            var json = $"{{\"fileVersion\":\"3.1.0\",\"compile\":\"2026-10-19\",\"port\":6600,\"state\":\"{state}\"}}";
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };
        }
    }

    [Fact]
    public async Task GetTest()
    {
        var driver = new FakeHttpDriver();

        // 验证基础
        {
            Assert.Null(driver.Client);
            Assert.Null(driver.ServiceProvider);
            Assert.NotNull(driver.Log);
            Assert.Null(driver.Tracer);

            var spec = driver.GetSpecification();
            Assert.Null(spec);
        }

        // 验证参数
        IDriverParameter parameter = null;
        {
            var hp = new HttpParameter
            {
                Address = "http://star.newlifex.com:6600",
                Method = "get",
                PathAndQuery = "/api",
            };
            var json = hp.ToJson();

            parameter = driver.CreateParameter(json);
            Assert.NotNull(parameter);

            var p = parameter as HttpParameter;
            Assert.NotNull(p);
        }

        // 模拟设备对象
        var mb = new Mock<IDevice>() { CallBase = true };
        var device = mb.Object;
        Rand.Fill(device);

        // 打开设备
        var node = await driver.OpenAsync(device, parameter);
        Assert.NotNull(node);
        Assert.Equal(driver, node.Driver);
        Assert.Equal(device, node.Device);
        Assert.Equal(parameter, node.Parameter);
        Assert.NotNull(driver.Client);

        // 读取数据
        var points = new IPoint[]
        {
            new PointModel { Name = "fileVersion", Type = "" },
            new PointModel { Name = "compile", Type = "date" },
            new PointModel { Address = "port", Type = "short" }
        };
        var rs = await driver.ReadAsync(node, points);
        Assert.NotNull(rs);
        Assert.True(rs.IsSuccess);
        Assert.NotEmpty(rs.Values);
        Assert.Equal(points.Length, rs.Values.Length);

        {
            var value = rs.Values[0];
            Assert.NotNull(value);
            Assert.Equal(typeof(String), value!.GetType());
        }
        {
            var value = rs.Values[1];
            Assert.NotNull(value);
            Assert.Equal(typeof(DateTime), value!.GetType());
        }
        {
            var value = rs.Values[2];
            Assert.NotNull(value);
            Assert.Equal(typeof(Int16), value!.GetType());
        }

        // 关闭设备
        await driver.CloseAsync(node);
    }

    [Fact]
    public async Task PostTest()
    {
        using var driver = new FakeHttpDriver();

        // 验证参数
        var hp = new HttpParameter
        {
            Address = "https://newlifex.com",
            Method = "Post",
            PathAndQuery = "/cube/info",
        };
        var parameter = driver.CreateParameter(hp.ToJson());

        var node = await driver.OpenAsync(null, parameter);

        // 读取数据
        var points = new IPoint[]
        {
            new PointModel { Name = "fileVersion", Type = "" },
            new PointModel { Name = "compile", Type = "date" },
            new PointModel { Address = "port", Type = "short" }
        };
        var rs = await driver.ReadAsync(node, points);
        Assert.True(rs.IsSuccess);
        Assert.Equal(points.Length, rs.Values.Length);
    }

    [Fact]
    public async Task PostTest2()
    {
        using var driver = new FakeHttpDriver();

        // 验证参数
        var state = Rand.NextString(16);
        var hp = new HttpParameter
        {
            Address = "https://newlifex.com",
            Method = "Post",
            PathAndQuery = "/cube/info",
            PostData = $"state={state}",
        };
        var parameter = driver.CreateParameter(hp.ToJson());

        var node = await driver.OpenAsync(null, parameter);

        // 读取数据
        var points = new IPoint[]
        {
            new PointModel { Name = "state", Type = "" },
            new PointModel { Name = "compile", Type = "date" },
            new PointModel { Address = "port", Type = "short" }
        };
        var rs = await driver.ReadAsync(node, points);
        Assert.True(rs.IsSuccess);
        Assert.Equal(points.Length, rs.Values.Length);
        Assert.Equal(state, rs.GetValue("state"));
    }

    [Fact]
    public async Task CaptureAll()
    {
        using var driver = new FakeHttpDriver();

        // 验证参数
        var state = Rand.NextString(16);
        var hp = new HttpParameter
        {
            Address = "https://newlifex.com",
            Method = "Post",
            PathAndQuery = "/cube/info",
            PostData = $"state={state}",
            CaptureAll = true,
        };
        var parameter = driver.CreateParameter(hp.ToJson());

        var node = await driver.OpenAsync(null, parameter);

        // 读取数据
        var rs = await driver.ReadAsync(node, null);
        Assert.True(rs.IsSuccess);
        Assert.NotEmpty(rs.Values);
        Assert.Equal(state, rs.GetValue("state"));
    }
}
