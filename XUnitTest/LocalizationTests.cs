using System;
using System.Collections.Generic;
using System.Globalization;
using NewLife;
using NewLife.IoT.Drivers;
using NewLife.Log;
using Xunit;

namespace XUnitTest;

[CollectionDefinition("Localization", DisableParallelization = true)]
public class LocalizationCollection { }

[Collection("Localization")]
public class LocalizationTests
{
    private static readonly String[] _allKeys =
    [
        "Cannot find service [{0}]!",
        "Unsupported data type {0}!",
        "================Start scanning driver plugins================",
        "================End scanning driver plugins================",
        "Parameter cannot be null.",
        "Serial port name cannot be null.",
        "Network address cannot be null.",
        "Node or parameter is invalid.",
        "Port number is required in parameters.",
        "UDP broadcast to {0}:{1}, payload: {2}",
        "Received response from {0}:{1}, length: {2}",
        "Error receiving UDP response: {0}",
        "Error during UDP discovery on subnet {0}: {1}",
        "UDP device {0}:{1}",
        "Database connection string cannot be null.",
        "Database driver does not support write operations.",
        "Database driver does not support control operations.",
        "Driver plugin scanning is not supported under NativeAOT. Register drivers explicitly with DriverFactory.Register<T>.",
    ];
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
    {        var old = Localization.Culture;
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
    {        using var serialDriver = new NewLife.IoTSerial.Drivers.IoTSerialDriver();
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

            foreach (var key in _allKeys)
            {
                var text = Localization.Translate(key);
                Assert.NotEqual(key, text);
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
}
