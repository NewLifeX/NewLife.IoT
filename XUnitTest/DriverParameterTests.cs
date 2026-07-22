using System;
using System.Collections.Generic;
using NewLife.IoT.Drivers;
using Xunit;

namespace XUnitTest;

public class DriverParameterTests
{
    private class TestParameter : IDriverParameter
    {
        public String? Address { get; set; }
        public Int32 Port { get; set; }
        public Int32 Timeout { get; set; } = 3000;
    }

    [Fact]
    public void EncodeParameter_ToXml()
    {
        var p = new TestParameter { Address = "192.168.1.1", Port = 502, Timeout = 5000 };
        var xml = p.EncodeParameter();
        Assert.NotNull(xml);
        Assert.Contains("192.168.1.1", xml);
        Assert.Contains("502", xml);
    }

    [Fact]
    public void DecodeParameter_FromXml()
    {
        var xml = "<TestParameter><Address>192.168.1.1</Address><Port>502</Port><Timeout>5000</Timeout></TestParameter>";
        var dic = xml.DecodeParameter();
        Assert.NotNull(dic);
        Assert.Equal("192.168.1.1", dic!["Address"]);
        Assert.Equal("502", dic["Port"] + "");
    }

    [Fact]
    public void DecodeParameter_FromJson()
    {
        var json = """{"Address":"192.168.1.1","Port":502,"Timeout":5000}""";
        var dic = json.DecodeParameter();
        Assert.NotNull(dic);
        Assert.Equal("192.168.1.1", dic!["Address"]);
        Assert.Equal(502, Convert.ToInt32(dic["Port"]));
    }

    [Fact]
    public void DecodeParameter_EmptyString_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => "".DecodeParameter());
    }

    [Fact]
    public void GetKey_FromAddress()
    {
        var p = new TestParameter { Address = "192.168.1.1:502" };
        var key = p.GetKey();
        Assert.Equal("192.168.1.1:502", key);
    }

    [Fact]
    public void GetKey_FromIDriverParameterKey()
    {
        var p = new KeyedParameter { Key = "my-custom-key", Address = "ignored" };
        var key = p.GetKey();
        Assert.Equal("my-custom-key", key);
    }

    private class KeyedParameter : IDriverParameter, IDriverParameterKey
    {
        public String? Key { get; set; }
        public String? Address { get; set; }
        public String GetKey() => Key!;
    }

    [Fact]
    public void EncodeParameter_Dictionary()
    {
        var dic = new Dictionary<String, Object>
        {
            ["Address"] = "192.168.1.1",
            ["Port"] = 502,
            ["Timeout"] = 5000,
        };
        var xml = dic.EncodeParameter();
        Assert.NotNull(xml);
        Assert.Contains("Dictionary", xml);
    }

    [Fact]
    public void DriverParameter_IsIDriverParameter()
    {
        var p = new DriverParameter();
        Assert.IsAssignableFrom<IDriverParameter>(p);
    }
}
