using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using NewLife;
using NewLife.IoT;
using NewLife.IoT.Drivers;
using NewLife.IoT.Models;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using NewLife.Log;
using NewLife.Serialization;
using Xunit;

namespace XUnitTest;

[CollectionDefinition("Localization", DisableParallelization = true)]
public class LocalizationCollection { }

[Collection("Localization")]
public class LocalizationTests
{
    private static String GetRepositoryRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "NewLife.IoT.slnx"))) return dir.FullName;
        }

        throw new InvalidOperationException("无法定位仓库根目录");
    }

    private static Dictionary<String, String> ReadLocale(String culture)
    {
        var path = Path.Combine(GetRepositoryRoot(), "Locales", culture + ".yml");
        var map = new Dictionary<String, String>(StringComparer.Ordinal);
        foreach (var line in File.ReadAllLines(path))
        {
            var text = line.Trim();
            if (text.Length == 0 || text.StartsWith("#")) continue;

            var match = Regex.Match(text, "^\"" + "(?<key>(?:[^\"\\\\]|\\\\.)*)" + "\"\\s*:\\s*\"" + "(?<value>(?:[^\"\\\\]|\\\\.)*)" + "\"\\s*$");
            if (match.Success) map[match.Groups["key"].Value] = match.Groups["value"].Value;
        }

        return map;
    }

    private sealed class MemoryLog : Logger
    {
        public List<String> Messages { get; } = [];

        protected override void OnWrite(LogLevel level, String format, Object[] args)
            => Messages.Add(Logger.FormatMessage(this, format, args));
    }

    private sealed class TestDriver : DriverBase
    {
        public void LogMessage(String format, params Object[] args) => WriteLog(format, args);
    }

    [Fact]
    public void TranslateSimplifiedChinese()
    {
        var old = Localization.Culture;
        try
        {
            Localization.Culture = new CultureInfo("zh-CN");

            Assert.Equal("参数不能为空。", Localization.Translate("Parameter cannot be null."));
            Assert.Equal("找不到服务[abc]！", Localization.Translate("Cannot find service [{0}]!", "abc"));
            Assert.Equal("数据库驱动不支持写入操作。", Localization.Translate("Database driver does not support write operations."));
        }
        finally
        {
            Localization.Culture = old;
        }
    }

    [Fact]
    public void TranslateTraditionalChinese()
    {
        var old = Localization.Culture;
        try
        {
            Localization.Culture = new CultureInfo("zh-Hant");

            Assert.Equal("參數不能為空。", Localization.Translate("Parameter cannot be null."));
        }
        finally
        {
            Localization.Culture = old;
        }
    }

    [Fact]
    public void FallbackToEnglish()
    {
        var old = Localization.Culture;
        try
        {
            Localization.Culture = new CultureInfo("sw-KE");

            Assert.Equal("Parameter cannot be null.", Localization.Translate("Parameter cannot be null."));
        }
        finally
        {
            Localization.Culture = old;
        }
    }

    [Fact]
    public void PrefixedLogKeepsPrefixAndTranslates()
    {
        var old = Localization.Culture;
        try
        {
            Localization.Culture = new CultureInfo("zh-CN");

            var log = new MemoryLog();
            var driver = new TestDriver { Log = log };
            driver.LogMessage("Parameter cannot be null.");

            Assert.Single(log.Messages);
            Assert.Equal("[Test]参数不能为空。", log.Messages[0]);
        }
        finally
        {
            Localization.Culture = old;
        }
    }

    [Fact]
    public void DriverAndTransportPrefixes()
    {
        using var serialDriver = new NewLife.IoTSerial.Drivers.IoTSerialDriver();
        using var udpDriver = new NewLife.IoTSocket.Drivers.IoTUdpDriver();
        using var serial = new NewLife.IoTSerial.Transport.SerialTransport { PortName = "COM9" };
        using var tcp = new NewLife.IoTSocket.Transport.TcpTransport { Server = "10.0.0.1", Port = 502 };

        Assert.Equal("[IoTSerial]", serialDriver.LogPrefix);
        Assert.Equal("[IoTUdp]", udpDriver.LogPrefix);
        Assert.Equal("[Serial:COM9]", serial.LogPrefix);
        Assert.Equal("[Tcp:10.0.0.1:502]", tcp.LogPrefix);
    }

    [Theory]
    [InlineData("ar")]
    [InlineData("bn")]
    [InlineData("cs")]
    [InlineData("da")]
    [InlineData("de")]
    [InlineData("el")]
    [InlineData("es")]
    [InlineData("fa")]
    [InlineData("fi")]
    [InlineData("fr")]
    [InlineData("he")]
    [InlineData("hi")]
    [InlineData("hu")]
    [InlineData("id")]
    [InlineData("it")]
    [InlineData("ja")]
    [InlineData("ko")]
    [InlineData("ms")]
    [InlineData("nb")]
    [InlineData("nl")]
    [InlineData("pl")]
    [InlineData("pt")]
    [InlineData("ro")]
    [InlineData("ru")]
    [InlineData("sv")]
    [InlineData("th")]
    [InlineData("tr")]
    [InlineData("uk")]
    [InlineData("vi")]
    [InlineData("zh-Hans")]
    [InlineData("zh-Hant")]
    public void AllLocalesTranslateAllKeys(String culture)
    {
        var old = Localization.Culture;
        try
        {
            Localization.Culture = new CultureInfo(culture);

            var table = ReadLocale(culture);
            var keys = ReadLocale("zh-Hans").Keys;
            Assert.NotEmpty(keys);
            foreach (var key in keys)
            {
                Assert.True(table.ContainsKey(key), $"{culture} 缺少词表键：{key}");
                Assert.Equal(table[key], Localization.Translate(key));
            }
        }
        finally
        {
            Localization.Culture = old;
        }
    }

    [Fact]
    public void LogCultureOverridesCultureForPrefixedLog()
    {
        var culture = Localization.Culture;
        var logCulture = Localization.LogCulture;
        try
        {
            Localization.Culture = new CultureInfo("zh-Hans");
            Localization.LogCulture = new CultureInfo("ja");

            var log = new MemoryLog();
            var driver = new TestDriver { Log = log };
            driver.LogMessage("Parameter cannot be null.");

            Assert.Single(log.Messages);
            Assert.Equal("[Test]パラメーターを null にすることはできません。", log.Messages[0]);
        }
        finally
        {
            Localization.Culture = culture;
            Localization.LogCulture = logCulture;
        }
    }

    [Fact]
    public void ExceptionAndResultMessagesAreTranslated()
    {
        var old = Localization.Culture;
        try
        {
            Localization.Culture = new CultureInfo("zh-CN");

            var ex = new IoTException(IoTErrorCode.InvalidParameter, "Parameter cannot be null.");
            Assert.Equal("参数不能为空。", ex.Message);

            var wr = WriteResult.Fail(IoTErrorCode.InvalidParameter, "Node or parameter is invalid.");
            Assert.Equal("节点或参数无效。", wr.Message);

            var cr = ControlResult.Fail(IoTErrorCode.NotSupported, "Database driver does not support control operations.");
            Assert.Equal("数据库驱动不支持服务控制操作。", cr.Message);

            var rr = ReadResult.Fail(IoTErrorCode.Timeout, "Node or parameter is invalid.");
            Assert.Equal("节点或参数无效。", rr.Message);
        }
        finally
        {
            Localization.Culture = old;
        }
    }

    [Fact]
    [DisplayName("不支持的类型与服务名异常消息按语言翻译")]
    public void UnsupportedOperationMessagesAreTranslated()
    {
        var old = Localization.Culture;
        try
        {
            Localization.Culture = new CultureInfo("zh-CN");

            Assert.Contains("参数不能为空。", Assert.Throws<ArgumentNullException>(() => DriverFactory.Register("null-type", (Type)null!)).Message);

            var point = new PointModel { Type = "unknown" };
            Assert.Equal("不支持的数据类型 unknown！", Assert.Throws<NotSupportedException>(() => point.Convert([1])).Message);
            Assert.Equal("不支持的数据类型 unknown！", Assert.Throws<NotSupportedException>(() => point.GetBytes(1)).Message);

            var ps = new PropertySpec { DataType = new TypeSpec { Type = "long" } };
            Assert.Equal("不支持的数据类型 Int64！", Assert.Throws<NotImplementedException>(() => ps.Decode(new Byte[8])).Message);

            var driver = new TestDriver();
            Assert.Equal("不支持该操作。", Assert.Throws<NotImplementedException>(() =>
            {
                _ = driver.ReadAsync(null!, []);
            }).Message);

            using var serial = new NewLife.IoTSerial.Drivers.IoTSerialDriver();
            Assert.Equal("服务名称不能为空。", Assert.Throws<NotImplementedException>(() =>
            {
                _ = serial.ControlAsync(null!, new ControlRequest());
            }).Message);
        }
        finally
        {
            Localization.Culture = old;
        }
    }

    [Fact]
    [DisplayName("IoT类型显示名随语言切换，不使用跨语言缓存")]
    public void TypeHelperDisplayNamesFollowCulture()
    {
        var old = Localization.Culture;
        try
        {
            Localization.Culture = new CultureInfo("zh-CN");
            var zh = TypeHelper.GetIoTTypes(true);
            Assert.Equal("整数", zh["int"]);
            Assert.Equal("小数", zh["float"]);
            Assert.Equal("布尔型", zh["bool"]);
            Assert.Equal("字节", zh["byte"]);
            Assert.Equal("文本", zh["text"]);
            Assert.Equal("时间", zh["time"]);

            Localization.Culture = new CultureInfo("en-US");
            var en = TypeHelper.GetIoTTypes(true);
            Assert.Equal("Integer", en["int"]);
            Assert.Equal("Decimal number", en["float"]);
            Assert.Equal("Boolean", en["bool"]);

            Localization.Culture = new CultureInfo("zh-Hans");
            Assert.Equal("整数", TypeHelper.GetIoTTypes(true)["int"]);
        }
        finally
        {
            Localization.Culture = old;
        }
    }

    [Fact]
    [DisplayName("驱动物模型属性名随语言切换")]
    public void DriverSpecificationNamesFollowCulture()
    {
        var old = Localization.Culture;
        try
        {
            using var driver = new NewLife.IoTSerial.Drivers.IoTSerialDriver();

            Localization.Culture = new CultureInfo("zh-CN");
            Assert.Equal("响应数据", driver.GetSpecification()!.Properties![0].Name);

            Localization.Culture = new CultureInfo("en-US");
            Assert.Equal("Response data", driver.GetSpecification()!.Properties![0].Name);
        }
        finally
        {
            Localization.Culture = old;
        }
    }

    [Fact]
    [DisplayName("驱动显示名与枚举描述随语言切换")]
    public void DriverMetadataFollowsCulture()
    {
        var old = Localization.Culture;
        try
        {
            Localization.Culture = new CultureInfo("zh-CN");
            Assert.Equal("通用串口驱动", typeof(NewLife.IoTSerial.Drivers.IoTSerialDriver).GetDisplayName());
            Assert.Equal("变更上报", PostKinds.Changed.GetDescription());
            Assert.Equal("就绪", ServiceStatus.就绪.GetDescription());

            Localization.Culture = new CultureInfo("en-US");
            Assert.Equal("Serial driver", typeof(NewLife.IoTSerial.Drivers.IoTSerialDriver).GetDisplayName());
            Assert.Equal("Report on change", PostKinds.Changed.GetDescription());
            Assert.Equal("Ready", ServiceStatus.就绪.GetDescription());

            var xml = new NewLife.IoTSerial.Drivers.IoTSerialParameter { PortName = "COM1" }.EncodeParameter();
            Assert.Contains("Serial port", xml);
        }
        finally
        {
            Localization.Culture = old;
        }
    }

    [Fact]
    [DisplayName("IoT类型兜底名称不受土耳其语大小写规则影响")]
    public void IoTTypeFallbackNameIsCultureInvariant()
    {
        var old = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("tr-TR");

            Assert.Equal("intptr", TypeHelper.GetIoTType(typeof(IntPtr), true));
        }
        finally
        {
            CultureInfo.CurrentCulture = old;
        }
    }

    [Fact]
    [DisplayName("JSON 往返不受当前区域性小数点符号影响")]
    public void JsonRoundTripUsesInvariantNumbers()
    {
        var old = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");

            var spec = new ThingSpec { Profile = new Profile { ProductKey = "demo", Version = "1.0" } };
            var ps = spec.AddProperty("temp", "Temperature", "float");
            ps.DataType!.Specs = new DataSpecs { Step = 0.1, Min = -40.5, Max = 125.5 };

            var json = spec.ToJson();
            Assert.Contains("0.1", json);
            Assert.DoesNotContain("0,1", json);

            var spec2 = new ThingSpec();
            spec2.FromJson(json);
            Assert.Equal(0.1, spec2.GetProperty("temp")!.DataType!.Specs!.Step);
        }
        finally
        {
            CultureInfo.CurrentCulture = old;
        }
    }

    [Fact]
    [DisplayName("物模型步进精度不受当前区域性小数点符号影响")]
    public void DecodeStepDigitsUseInvariantCulture()
    {
        var old = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");

            var ps = new PropertySpec
            {
                DataType = new TypeSpec { Type = "float", Specs = new DataSpecs { Step = 0.1, Order = ByteOrder.DCBA } },
                Scaling = 2,
            };
            Assert.Equal(2.5f, (Single)ps.Decode(1.2345f.GetBytes(ByteOrder.DCBA))!);

            var ps2 = new PropertySpec
            {
                DataType = new TypeSpec { Type = "double", Specs = new DataSpecs { Step = 0.01, Order = ByteOrder.DCBA } },
                Scaling = 2,
            };
            Assert.Equal(2.47, (Double)ps2.Decode(1.234567.GetBytes(ByteOrder.DCBA))!);
        }
        finally
        {
            CultureInfo.CurrentCulture = old;
        }
    }
}
