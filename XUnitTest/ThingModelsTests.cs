using System;
using NewLife.IoT.ThingModels;
using Xunit;

namespace XUnitTest;

public class ThingModelsTests
{
    #region PropertyModel

    [Fact]
    public void PropertyModel_DefaultValues()
    {
        var m = new PropertyModel();
        Assert.Null(m.Name);
        Assert.Null(m.Value);
    }

    [Fact]
    public void PropertyModel_SetProperties()
    {
        var m = new PropertyModel { Name = "Temperature", Value = 25.5 };
        Assert.Equal("Temperature", m.Name);
        Assert.Equal(25.5, m.Value);
    }

    #endregion

    #region DevicePropertyModel

    [Fact]
    public void DevicePropertyModel_InheritsPropertyModel()
    {
        var m = new DevicePropertyModel { Name = "Temp", Value = 30, DeviceCode = "Dev001" };
        Assert.Equal("Temp", m.Name);
        Assert.Equal(30, m.Value);
        Assert.Equal("Dev001", m.DeviceCode);
        Assert.IsAssignableFrom<PropertyModel>(m);
    }

    #endregion

    #region EventModel

    [Fact]
    public void EventModel_DefaultValues()
    {
        var m = new EventModel();
        Assert.Equal(0, m.Time);
        Assert.Null(m.Type);
        Assert.Null(m.Name);
        Assert.Null(m.Remark);
        Assert.Null(m.Data);
    }

    [Fact]
    public void EventModel_SetProperties()
    {
        var m = new EventModel
        {
            Time = 1700000000000,
            Type = "info",
            Name = "DeviceOnline",
            Remark = "device connected",
            Data = new { Port = 1 }
        };
        Assert.Equal(1700000000000, m.Time);
        Assert.Equal("info", m.Type);
        Assert.Equal("DeviceOnline", m.Name);
        Assert.Equal("device connected", m.Remark);
        Assert.NotNull(m.Data);
    }

    #endregion

    #region EventModels

    [Fact]
    public void EventModels_DefaultValues()
    {
        var m = new EventModels();
        Assert.Null(m.DeviceCode);
        Assert.Null(m.Items);
    }

    [Fact]
    public void EventModels_WithItems()
    {
        var items = new[] { new EventModel { Name = "e1" }, new EventModel { Name = "e2" } };
        var m = new EventModels { DeviceCode = "D1", Items = items };
        Assert.Equal("D1", m.DeviceCode);
        Assert.Equal(2, m.Items!.Length);
    }

    #endregion

    #region DataModel

    [Fact]
    public void DataModel_DefaultValues()
    {
        var m = new DataModel();
        Assert.Equal(0, m.Time);
        Assert.Null(m.Name);
        Assert.Null(m.Value);
    }

    [Fact]
    public void DataModel_SetProperties()
    {
        var m = new DataModel { Time = 1700000000000, Name = "counter", Value = "42" };
        Assert.Equal(1700000000000, m.Time);
        Assert.Equal("counter", m.Name);
        Assert.Equal("42", m.Value);
    }

    #endregion

    #region DataModels

    [Fact]
    public void DataModels_DefaultValues()
    {
        var m = new DataModels();
        Assert.Equal(0, m.Id);
        Assert.Null(m.DeviceCode);
        Assert.Null(m.Items);
    }

    [Fact]
    public void DataModels_WithItems()
    {
        var items = new[] { new DataModel { Name = "d1" } };
        var m = new DataModels { Id = 123, DeviceCode = "D1", Items = items };
        Assert.Equal(123, m.Id);
        Assert.Equal("D1", m.DeviceCode);
        Assert.Single(m.Items!);
    }

    #endregion

    #region EventModes

    [Fact]
    public void EventModes_Values()
    {
        Assert.Equal(0, (Int32)EventModes.None);
        Assert.Equal(1, (Int32)EventModes.Client);
        Assert.Equal(2, (Int32)EventModes.Server);
    }

    #endregion

    #region PropertyModels

    [Fact]
    public void PropertyModels_DefaultValues()
    {
        var m = new PropertyModels();
        Assert.Null(m.DeviceCode);
        Assert.Null(m.Items);
    }

    [Fact]
    public void PropertyModels_WithItems()
    {
        var items = new[] { new PropertyModel { Name = "p1", Value = 100 } };
        var m = new PropertyModels { DeviceCode = "Dev1", Items = items };
        Assert.Equal("Dev1", m.DeviceCode);
        Assert.Single(m.Items!);
        Assert.Equal(100, m.Items![0].Value);
    }

    #endregion

    #region ShadowModel

    [Fact]
    public void ShadowModel_DefaultValues()
    {
        var m = new ShadowModel();
        Assert.Null(m.DeviceCode);
        Assert.Null(m.Shadow);
    }

    [Fact]
    public void ShadowModel_SetProperties()
    {
        var m = new ShadowModel { DeviceCode = "D1", Shadow = new { state = "online" } };
        Assert.Equal("D1", m.DeviceCode);
        Assert.NotNull(m.Shadow);
    }

    #endregion

    #region ServiceModel

    [Fact]
    public void ServiceModel_DefaultValues()
    {
        var m = new ServiceModel();
        Assert.Equal(0, m.Id);
        Assert.Null(m.Name);
        Assert.Null(m.InputData);
        Assert.Equal(default, m.StartTime);
        Assert.Equal(default, m.Expire);
        Assert.Null(m.DeviceCode);
        Assert.Null(m.Type);
        Assert.Null(m.TraceId);
    }

    [Fact]
    public void ServiceModel_SetProperties()
    {
        var now = DateTime.UtcNow;
        var m = new ServiceModel
        {
            Id = 1,
            Name = "SetProperty",
            InputData = "{\"key\":\"value\"}",
            StartTime = now,
            Expire = now.AddMinutes(5),
            DeviceCode = "SubDev1",
            Type = "async",
            TraceId = "trace-123"
        };
        Assert.Equal(1, m.Id);
        Assert.Equal("SetProperty", m.Name);
        Assert.Contains("key", m.InputData);
        Assert.Equal(now, m.StartTime);
        Assert.Equal("SubDev1", m.DeviceCode);
        Assert.Equal("async", m.Type);
        Assert.Equal("trace-123", m.TraceId);
    }

    #endregion

    #region ServiceReplyModel

    [Fact]
    public void ServiceReplyModel_DefaultValues()
    {
        var m = new ServiceReplyModel();
        Assert.Equal(0, m.Id);
        Assert.Equal(ServiceStatus.就绪, m.Status);
        Assert.Null(m.Data);
    }

    [Fact]
    public void ServiceReplyModel_SetProperties()
    {
        var m = new ServiceReplyModel { Id = 99, Status = ServiceStatus.已完成, Data = "ok" };
        Assert.Equal(99, m.Id);
        Assert.Equal(ServiceStatus.已完成, m.Status);
        Assert.Equal("ok", m.Data);
    }

    #endregion

    #region ServiceEventArgs

    [Fact]
    public void ServiceEventArgs_DefaultValues()
    {
        var e = new ServiceEventArgs();
        Assert.Null(e.Model);
        Assert.Null(e.Reply);
        Assert.IsAssignableFrom<EventArgs>(e);
    }

    [Fact]
    public void ServiceEventArgs_WithModel()
    {
        var model = new ServiceModel { Id = 1, Name = "test" };
        var e = new ServiceEventArgs { Model = model };
        Assert.Same(model, e.Model);
        Assert.Null(e.Reply);
    }

    #endregion

    #region ServiceStatus

    [Fact]
    public void ServiceStatus_Values()
    {
        Assert.Equal(0, (Int32)ServiceStatus.就绪);
        Assert.Equal(1, (Int32)ServiceStatus.处理中);
        Assert.Equal(2, (Int32)ServiceStatus.已完成);
        Assert.Equal(3, (Int32)ServiceStatus.取消);
        Assert.Equal(4, (Int32)ServiceStatus.错误);
    }

    #endregion

    #region PointModel (additional coverage)

    [Fact]
    public void PointModel_DefaultValues()
    {
        var p = new PointModel();
        Assert.Null(p.Name);
        Assert.Null(p.Address);
        Assert.Null(p.Type);
        Assert.Equal(0, p.Length);
    }

    [Fact]
    public void PointModel_ReadWriteRule()
    {
        var p = new PointModel { ReadRule = "raw * 0.1", WriteRule = "val / 0.1" };
        Assert.Equal("raw * 0.1", p.ReadRule);
        Assert.Equal("val / 0.1", p.WriteRule);
    }

    #endregion
}
