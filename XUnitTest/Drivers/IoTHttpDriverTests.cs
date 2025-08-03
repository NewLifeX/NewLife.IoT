using System;
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

public class IoTHttpDriverTests
{
    [Fact]
    public async Task FullTest()
    {
        var driver = new IoTHttpDriver();

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
        Assert.NotEmpty(rs);
        Assert.Equal(points.Length, rs.Count);

        {
            Assert.True(rs.TryGetValue(points[0].Name, out var value));
            Assert.NotNull(value);
            Assert.Equal(typeof(String), value.GetType());
        }
        {
            Assert.True(rs.TryGetValue(points[1].Name, out var value));
            Assert.NotNull(value);
            Assert.Equal(typeof(DateTime), value.GetType());
        }
        {
            Assert.True(rs.TryGetValue(points[2].Address, out var value));
            Assert.NotNull(value);
            Assert.Equal(typeof(Int16), value.GetType());
        }

        // 关闭设备
        await driver.CloseAsync(node);
    }
}
