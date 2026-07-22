using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NewLife.IoT;
using NewLife.IoT.Drivers;
using NewLife.IoT.Models;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using Xunit;

namespace XUnitTest;

public class DriverBaseTests
{
    #region Test Driver

    private class TestConcreteDriver : DriverBase<TestNode, TestParameter>
    {
        public Boolean DiagnosticsRead => Diagnostics;

        protected override Boolean OnGetSpecification(ThingSpec ts)
        {
            ts.Properties = [PropertySpec.Create("Val", "value", "int")];
            return true;
        }
    }

    private class TestNode : Node { }
    private class TestParameter : IDriverParameter
    {
        public String? Address { get; set; }
        public Int32 Timeout { get; set; } = 3000;
    }

    #endregion

    #region Properties

    [Fact]
    public void DriverBase_Diagnostics_DefaultFalse()
    {
        var driver = new TestConcreteDriver();
        Assert.False(driver.Diagnostics);
        Assert.False(driver.DiagnosticsRead);
    }

    [Fact]
    public void DriverBase_Diagnostics_Settable()
    {
        var driver = new TestConcreteDriver { Diagnostics = true };
        Assert.True(driver.Diagnostics);
        Assert.True(driver.DiagnosticsRead);
    }

    [Fact]
    public void DriverBase_ServiceProvider_DefaultNull()
    {
        var driver = new TestConcreteDriver();
        Assert.Null(driver.ServiceProvider);
    }

    [Fact]
    public void DriverBase_ServiceProvider_Settable()
    {
        var sp = new Microsoft.Extensions.DependencyInjection.ServiceCollection().BuildServiceProvider();
        var driver = new TestConcreteDriver { ServiceProvider = sp };
        Assert.Same(sp, driver.ServiceProvider);
    }

    #endregion

    #region RaiseDataReceived

    [Fact]
    public void RaiseDataReceived_FiresEvent()
    {
        var driver = new TestConcreteDriver();
        DriverDataEventArgs? captured = null;
        driver.DataReceived += (s, e) => captured = e;

        var node = new Node();
        var points = new IPoint[] { new PointModel { Name = "test", Type = "int" } };
        var result = ReadResult.Success(points, [42]);
        driver.TestRaiseDataReceived(node, points, result);

        Assert.NotNull(captured);
        Assert.Same(node, captured!.Node);
        Assert.Same(points, captured.Points);
        Assert.Same(result, captured.Result);
    }

    [Fact]
    public void RaiseDataReceived_NoSubscribers_DoesNotThrow()
    {
        var driver = new TestConcreteDriver();
        driver.TestRaiseDataReceived(new Node(), [], ReadResult.Success([], []));
        Assert.True(true);
    }

    #endregion

    #region CreateParameter

    [Fact]
    public void CreateParameter_EmptyString_CreatesDefault()
    {
        var driver = new TestConcreteDriver();
        var p = driver.CreateParameter(null);
        Assert.NotNull(p);
        Assert.IsType<TestParameter>(p);
    }

    [Fact]
    public void CreateParameter_FromXml()
    {
        var driver = new TestConcreteDriver();
        var xml = "<TestParameter><Address>192.168.1.1</Address><Timeout>5000</Timeout></TestParameter>";
        var p = driver.CreateParameter(xml);
        Assert.NotNull(p);
        var tp = Assert.IsType<TestParameter>(p);
        Assert.Equal("192.168.1.1", tp.Address);
        Assert.Equal(5000, tp.Timeout);
    }

    #endregion

    #region GetSpecification

    [Fact]
    public void GetSpecification_ReturnsThingSpec()
    {
        var driver = new TestConcreteDriver();
        var spec = driver.GetSpecification();
        Assert.NotNull(spec);
        Assert.NotNull(spec.Properties);
        Assert.Single(spec.Properties!);
    }

    #endregion

    #region OpenAsync

    [Fact]
    public async Task OpenAsync_CreatesNode()
    {
        var driver = new TestConcreteDriver();
        var device = new TestDevice();
        var parameter = new TestParameter { Address = "192.168.1.1" };

        var node = await driver.OpenAsync(device, parameter);
        Assert.NotNull(node);
        Assert.Same(driver, node.Driver);
        Assert.Same(device, node.Device);
        Assert.Same(parameter, node.Parameter);
    }

    private class TestDevice : IDevice
    {
        public String Code { get; set; } = "test";
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

    #endregion
}

/// <summary>暴露受保护方法的测试辅助类</summary>
public static class DriverBaseTestExtensions
{
    public static void TestRaiseDataReceived(this DriverBase driver, INode node, IPoint[] points, ReadResult result)
        => driver.GetType().GetMethod("RaiseDataReceived",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(driver, [node, points, result]);
}
