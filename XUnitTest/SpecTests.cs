using System.IO;
using NewLife.IoT.ThingSpecification;
using NewLife.Serialization;
using Xunit;

namespace XUnitTest;

public class SpecTests
{
    [Fact]
    public void Test1()
    {
        var file = "TSL1.json";
        var txt = File.ReadAllText(file.GetFullPath());

        var dic = JsonParser.Decode(txt);

        var pis = JsonHelper.Convert<PropertySpec[]>(dic["properties"]);
        Assert.NotNull(pis);
        Assert.Equal(8, pis.Length);

        var pi = pis[0];
        Assert.Equal("memory_usage", pi.Id);
        Assert.Equal("内存使用率", pi.Name);
        Assert.Equal("r", pi.AccessMode);
        Assert.True(pi.Required);
        Assert.Equal("float", pi.DataType.Type);
        Assert.Equal(0, pi.DataType.Specs.Min);
        Assert.Equal(100, pi.DataType.Specs.Max);
        Assert.Equal(0.1, pi.DataType.Specs.Step);
        Assert.Equal("%", pi.DataType.Specs.Unit);
        Assert.Equal("百分比", pi.DataType.Specs.UnitName);
        Assert.Equal(0, pi.DataType.Specs.Length);

        var eis = JsonHelper.Convert<EventSpec[]>(dic["events"]);
        Assert.NotNull(eis);
        Assert.Single(eis);

        var ei = eis[0];
        Assert.Equal("post", ei.Id);
        Assert.Equal("post", ei.Name);
        Assert.True(ei.Required);
        Assert.Equal("info", ei.Type);
        Assert.Equal("thing.event.property.post", ei.Method);
        Assert.Equal(8, ei.OutputData.Length);

        var sis = JsonHelper.Convert<ServiceSpec[]>(dic["services"]);
        Assert.NotNull(sis);
        Assert.Equal(2, sis.Length);

        var si = sis[0];
        Assert.Equal("set", si.Id);
        Assert.Equal("set", si.Name);
        Assert.True(si.Required);
        Assert.Equal("async", si.Type);
        Assert.Equal("thing.service.property.set", si.Method);
        Assert.Equal(2, si.InputData.Length);
        Assert.Empty(si.OutputData);

        si = sis[1];
        Assert.Equal("get", si.Id);
        Assert.Equal("get", si.Name);
        Assert.True(si.Required);
        Assert.Equal("async", si.Type);
        Assert.Equal("thing.service.property.get", si.Method);
        Assert.Equal(0, si.InputData.Length);
        Assert.Equal(8, si.OutputData.Length);

        var pts = JsonHelper.Convert<PropertyExtend[]>(dic["extendedProperties"]);
        Assert.NotNull(pts);
        Assert.Equal(2, pts.Length);

        var pt = pts[0];
        Assert.Equal("memory_usage", pt.Id);
        Assert.Equal("4x100", pt.Address);
    }

    [Fact]
    public void Test2()
    {
        var file = "TSL1.json";
        var txt = File.ReadAllText(file.GetFullPath());

        var thing = txt.ToJsonEntity<ThingSpec>();
        Assert.Equal("http://iot.feifan.link/schema.json", thing.Schema);
        Assert.Equal("1.6", thing.Profile.Version);
        Assert.Equal("EdgeGateway", thing.Profile.ProductKey);

        var pis = thing.Properties;
        Assert.NotNull(pis);
        Assert.Equal(8, pis.Length);

        var pi = pis[0];
        Assert.Equal("memory_usage", pi.Id);
        Assert.Equal("内存使用率", pi.Name);
        Assert.Equal("r", pi.AccessMode);
        Assert.True(pi.Required);
        Assert.Equal("float", pi.DataType.Type);
        Assert.Equal(0, pi.DataType.Specs.Min);
        Assert.Equal(100, pi.DataType.Specs.Max);
        Assert.Equal(0.1, pi.DataType.Specs.Step);
        Assert.Equal("%", pi.DataType.Specs.Unit);
        Assert.Equal("百分比", pi.DataType.Specs.UnitName);
        Assert.Equal(0, pi.DataType.Specs.Length);

        var eis = thing.Events;
        Assert.NotNull(eis);
        Assert.Single(eis);

        var ei = eis[0];
        Assert.Equal("post", ei.Id);
        Assert.Equal("post", ei.Name);
        Assert.True(ei.Required);
        Assert.Equal("info", ei.Type);
        Assert.Equal("thing.event.property.post", ei.Method);
        Assert.Equal(8, ei.OutputData.Length);

        var sis = thing.Services;
        Assert.NotNull(sis);
        Assert.Equal(2, sis.Length);

        var si = sis[0];
        Assert.Equal("set", si.Id);
        Assert.Equal("set", si.Name);
        Assert.True(si.Required);
        Assert.Equal("async", si.Type);
        Assert.Equal("thing.service.property.set", si.Method);
        Assert.Equal(2, si.InputData.Length);
        Assert.Empty(si.OutputData);

        si = sis[1];
        Assert.Equal("get", si.Id);
        Assert.Equal("get", si.Name);
        Assert.True(si.Required);
        Assert.Equal("async", si.Type);
        Assert.Equal("thing.service.property.get", si.Method);
        Assert.Equal(0, si.InputData.Length);
        Assert.Equal(8, si.OutputData.Length);

        var pts = thing.ExtendedProperties;
        Assert.NotNull(pts);
        Assert.Equal(2, pts.Length);
    }

    [Fact]
    public void Test3()
    {
        var file = "TSL1.json";
        var txt = File.ReadAllText(file.GetFullPath());

        //var thing = txt.ToJsonEntity<ThingSpec>();
        //var txt2 = thing.ToJson(true, false, true);

        var thing = new ThingSpec();
        thing.FromJson(txt);

        var txt2 = thing.ToJson();

        Assert.Equal(txt, txt2);
    }
}