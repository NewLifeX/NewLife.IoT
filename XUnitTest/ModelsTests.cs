using System;
using NewLife.IoT.Drivers;
using NewLife.IoT.Models;
using NewLife.IoT.ThingSpecification;
using Xunit;

namespace XUnitTest;

public class ModelsTests
{
    #region DeviceInfo

    [Fact]
    public void DeviceInfo_ImplementsIDeviceInfo()
    {
        var d = new DeviceInfo();
        Assert.IsAssignableFrom<IDeviceInfo>(d);
    }

    [Fact]
    public void DeviceInfo_DefaultValues()
    {
        var d = new DeviceInfo();
        Assert.Null(d.Code);
        Assert.Null(d.Name);
        Assert.Null(d.ProductCode);
        Assert.Null(d.Protocol);
        Assert.Null(d.Parameter);
    }

    [Fact]
    public void DeviceInfo_SetProperties()
    {
        var d = new DeviceInfo
        {
            Code = "Dev001",
            Name = "温度传感器",
            ProductCode = "TH-100",
            Protocol = "ModbusTcp",
            Parameter = "<ModbusParameter><Address>192.168.1.1</Address></ModbusParameter>"
        };
        Assert.Equal("Dev001", d.Code);
        Assert.Equal("温度传感器", d.Name);
        Assert.Equal("TH-100", d.ProductCode);
        Assert.Equal("ModbusTcp", d.Protocol);
        Assert.Contains("192.168.1.1", d.Parameter);
    }

    #endregion

    #region DeviceModel

    [Fact]
    public void DeviceModel_InheritsIDeviceInfo()
    {
        var d = new DeviceModel();
        Assert.IsAssignableFrom<IDeviceInfo>(d);
    }

    [Fact]
    public void DeviceModel_DefaultValues()
    {
        var d = new DeviceModel();
        Assert.Null(d.Code);
        Assert.Equal(0, d.PostPeriod);
        Assert.Equal(PostKinds.Changed, d.PostKind);
        Assert.Equal(0, d.PollingTime);
        Assert.False(d.Enable);
        Assert.Equal(default, d.UpdateTime);
    }

    [Fact]
    public void DeviceModel_SetProperties()
    {
        var now = DateTime.UtcNow;
        var d = new DeviceModel
        {
            Code = "Dev002",
            Name = "PLC1",
            ProductCode = "S7-1200",
            PostPeriod = 30,
            PostKind = PostKinds.Always,
            PollingTime = 500,
            Protocol = "Siemens",
            Parameter = "<SiemensParameter><Address>192.168.1.100</Address></SiemensParameter>",
            Enable = true,
            UpdateTime = now
        };
        Assert.Equal("Dev002", d.Code);
        Assert.Equal(30, d.PostPeriod);
        Assert.Equal(PostKinds.Always, d.PostKind);
        Assert.Equal(500, d.PollingTime);
        Assert.Equal("Siemens", d.Protocol);
        Assert.True(d.Enable);
        Assert.Equal(now, d.UpdateTime);
    }

    #endregion

    #region ServiceRequest

    [Fact]
    public void ServiceRequest_DefaultValues()
    {
        var r = new ServiceRequest();
        Assert.Equal(0, r.DeviceId);
        Assert.Null(r.DeviceCode);
        Assert.Null(r.ServiceName);
        Assert.Equal(0, r.Timeout);
        Assert.Equal(default, r.StartTime);
        Assert.Equal(default, r.Expire);
    }

    [Fact]
    public void ServiceRequest_SetProperties()
    {
        var now = DateTime.UtcNow;
        var r = new ServiceRequest
        {
            DeviceId = 100,
            DeviceCode = "SubDev",
            ServiceName = "Reboot",
            InputData = "{}",
            StartTime = now,
            Expire = now.AddSeconds(30),
            Timeout = 5000
        };
        Assert.Equal(100, r.DeviceId);
        Assert.Equal("SubDev", r.DeviceCode);
        Assert.Equal("Reboot", r.ServiceName);
        Assert.Equal(5000, r.Timeout);
    }

    #endregion

    #region PostKinds

    [Fact]
    public void PostKinds_Values()
    {
        Assert.Equal(0, (Int32)PostKinds.Changed);
        Assert.Equal(1, (Int32)PostKinds.Always);
        Assert.Equal(2, (Int32)PostKinds.Never);
    }

    #endregion

    #region DriverInfo (SYS-9c)

    [Fact]
    public void DriverInfo_DefaultValues()
    {
        var d = new DriverInfo();
        Assert.Null(d.Name);
        Assert.Null(d.DisplayName);
        Assert.Null(d.ClassName);
        Assert.Null(d.Version);
        Assert.Null(d.Specification);
        Assert.Null(d.DefaultParameter);
    }

    [Fact]
    public void DriverInfo_ToString_UsesDisplayName()
    {
        var d = new DriverInfo { Name = "ModbusTcp", DisplayName = "Modbus TCP驱动" };
        Assert.Equal("Modbus TCP驱动", d.ToString());
    }

    [Fact]
    public void DriverInfo_ToString_FallsBackToName()
    {
        var d = new DriverInfo { Name = "ModbusTcp" };
        Assert.Equal("ModbusTcp", d.ToString());
    }

    [Fact]
    public void DriverInfo_WithSpecification()
    {
        var spec = new ThingSpec();
        var d = new DriverInfo
        {
            Name = "TestDriver",
            Version = "1.0.0",
            IoTVersion = "3.0.0",
            Type = ".NET",
            ClassName = "MyDriver",
            Specification = spec,
            DefaultParameter = "<TestParameter><Port>502</Port></TestParameter>"
        };
        Assert.Equal("TestDriver", d.Name);
        Assert.Equal("1.0.0", d.Version);
        Assert.Same(spec, d.Specification);
        Assert.Contains("502", d.DefaultParameter);
    }

    #endregion

    #region SpecBase (TSL-2d)

    [Fact]
    public void SpecBase_DefaultValues()
    {
        var s = new TestSpec();
        Assert.Null(s.Id);
        Assert.Null(s.Name);
        Assert.False(s.Required);
    }

    [Fact]
    public void SpecBase_ToString()
    {
        var s = new TestSpec { Id = "temp", Name = "Temperature" };
        Assert.Equal("temp Temperature", s.ToString());
    }

    private class TestSpec : SpecBase { }

    #endregion
}
