// NewLife.IoT NativeAOT 兼容性冒烟测试。
// 同一份二进制分别以 JIT（dotnet run）与 NativeAOT（dotnet publish -r <rid>）运行，逐场景输出 PASS/FAIL，非零退出码即失败。
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using NewLife;
using NewLife.IoT;
using NewLife.IoT.Drivers;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using NewLife.IoTDatabase.Drivers;
using NewLife.IoTSerial.Drivers;
using NewLife.IoTSerial.Transport;
using NewLife.IoTSocket.Drivers;
using NewLife.IoTSocket.Transport;
using NewLife.Log;
using NewLife.Serialization;

try { Console.OutputEncoding = Encoding.UTF8; } catch { }

Console.WriteLine("=== NewLife.IoT AOT smoke ===");
Console.WriteLine($"framework={System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
Console.WriteLine($"rid={System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier} os={System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
Console.WriteLine($"arch={System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
Console.WriteLine($"IsDynamicCodeSupported={System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported}");

var failed = 0;
void Run(String name, Action action)
{
    try
    {
        action();
        Console.WriteLine($"[PASS] {name}");
    }
    catch (Exception ex)
    {
        failed++;
        Console.WriteLine($"[FAIL] {name} -> {ex.GetType().Name}: {ex.Message}");
    }
}

Run("Localization 多语言词表", () =>
{
    // 触发程序集加载，模块初始化器完成语言表注册
    _ = typeof(IoTException).Assembly;

    var saved = Localization.Culture;
    try
    {
        Localization.Culture = new CultureInfo("zh-CN");
        var text = Localization.Translate("Parameter cannot be null.");
        if (text != "参数不能为空。") throw new Exception("简体中文译文不匹配: " + text);

        Localization.Culture = new CultureInfo("sw-KE");
        var fallback = Localization.Translate("Parameter cannot be null.");
        if (fallback != "Parameter cannot be null.") throw new Exception("未回退英文: " + fallback);
    }
    finally
    {
        Localization.Culture = saved;
    }
});

Run("全语言词表（31 种）", () =>
{
    _ = typeof(IoTException).Assembly;

    var cultures = new[]
    {
        "ar", "bn", "cs", "da", "de", "el", "es", "fa", "fi", "fr", "he", "hi", "hu", "id", "it",
        "ja", "ko", "ms", "nb", "nl", "pl", "pt", "ro", "ru", "sv", "th", "tr", "uk", "vi", "zh-Hans", "zh-Hant",
    };

    var saved = Localization.Culture;
    try
    {
        Localization.Culture = new CultureInfo("en-US");
        var english = Localization.Translate("Parameter cannot be null.");

        foreach (var code in cultures)
        {
            Localization.Culture = new CultureInfo(code);
            var text = Localization.Translate("Parameter cannot be null.");
            if (text == english) throw new Exception($"{code} 未命中译文");
        }
    }
    finally
    {
        Localization.Culture = saved;
    }
});

Run("带前缀日志与本地化", () =>
{
    var saved = Localization.Culture;
    try
    {
        Localization.Culture = new CultureInfo("zh-CN");

        var log = new MemoryLog();
        using var driver = new IoTSerialDriver { Log = log };
        driver.WriteLog("Parameter cannot be null.");

        if (log.Messages.Count != 1) throw new Exception("日志数量不匹配");
        if (log.Messages[0] != "[IoTSerial]参数不能为空。") throw new Exception("前缀或译文不匹配: " + log.Messages[0]);
    }
    finally
    {
        Localization.Culture = saved;
    }
});

Run("TypeHelper 类型映射", () =>
{
    if (TypeHelper.GetIoTType(typeof(Int16)) != "int") throw new Exception("Int16 → int");
    if (TypeHelper.GetIoTType(typeof(Single)) != "float") throw new Exception("Single → float");
    if (TypeHelper.GetIoTType(typeof(Int16), true) != "short") throw new Exception("Int16 → short(full)");

    var point = new PointModel { Name = "temp", Type = "short", Length = 2 };
    if (point.GetNetType() != typeof(Int16)) throw new Exception("short → Int16");
});

Run("DataHelper 字节序编解码", () =>
{
    var buf = ((UInt16)0x1234).GetBytes(EndianType.BigEndian);
    if (buf[0] != 0x12 || buf[1] != 0x34) throw new Exception("BigEndian 编码错误");
    if (buf.ToUInt16(EndianType.BigEndian) != 0x1234) throw new Exception("BigEndian 解码错误");
});

Run("DriverCodec 编解码", () =>
{
    var pk = DriverCodec.Encode("0x010300000002C40B");
    if (pk == null || pk.Length != 8) throw new Exception("HEX 编码错误");

    var text = DriverCodec.Decode(pk, "HEX") as String;
    if (text != "010300000002C40B") throw new Exception("HEX 解码错误: " + text);
});

Run("ThingSpec JSON 往返", () =>
{
    var spec = new ThingSpec
    {
        Profile = new Profile { ProductKey = "demo", Version = "1.0" },
    };
    spec.AddProperty("temp", "Temperature", "float");

    var json = spec.ToJson();
    var spec2 = new ThingSpec();
    spec2.FromJson(json);

    var pi = spec2.GetProperty("temp");
    if (pi == null) throw new Exception("属性丢失");
    if (pi.Name != "Temperature") throw new Exception("属性名称不匹配: " + pi.Name);
});

Run("结果类型与异常翻译", () =>
{
    var saved = Localization.Culture;
    try
    {
        Localization.Culture = new CultureInfo("zh-Hans");

        var wr = WriteResult.Fail(IoTErrorCode.InvalidParameter, "Node or parameter is invalid.");
        if (wr.Message != "节点或参数无效。") throw new Exception("WriteResult 未翻译: " + wr.Message);

        var rr = ReadResult.Fail(IoTErrorCode.Timeout, "Node or parameter is invalid.");
        if (rr.Message != "节点或参数无效。") throw new Exception("ReadResult 未翻译: " + rr.Message);

        var cr = ControlResult.Fail(IoTErrorCode.NotSupported, "Database driver does not support control operations.");
        if (cr.Message != "数据库驱动不支持服务控制操作。") throw new Exception("ControlResult 未翻译: " + cr.Message);

        var ex = new IoTException(IoTErrorCode.ConnectionFailed, "Parameter cannot be null.");
        if (ex.Message != "参数不能为空。") throw new Exception("IoTException 未翻译: " + ex.Message);
    }
    finally
    {
        Localization.Culture = saved;
    }
});

Run("驱动注册与参数创建", () =>
{
    DriverFactory.Register<IoTSerialDriver>("IoTSerial");

    var driver = DriverFactory.Create("IoTSerial", "aot-com1");
    if (driver is not IoTSerialDriver) throw new Exception("驱动创建失败");
    (driver as IDisposable)?.Dispose();

    var parameter = DriverFactory.CreateParameter("IoTSerial", """{"PortName":"COM1","Baudrate":9600}""");
    if (parameter is not IoTSerialParameter p) throw new Exception("参数创建失败");
    if (p.PortName != "COM1" || p.Baudrate != 9600) throw new Exception("参数值不匹配");
});

Run("驱动物模型", () =>
{
    using var driver = new IoTSerialDriver();
    var spec = driver.GetSpecification();
    if (spec == null) throw new Exception("物模型为空");
    if (spec.Profile?.ProductKey != "IoTSerial") throw new Exception("ProductKey 不匹配: " + spec.Profile?.ProductKey);
    if (spec.GetProperty("Data") == null) throw new Exception("属性 Data 缺失");
});

Run("传输层前缀", () =>
{
    using var serial = new SerialTransport { PortName = "COM7" };
    if (serial.LogPrefix != "[Serial:COM7]") throw new Exception("串口前缀不匹配: " + serial.LogPrefix);

    using var tcp = new TcpTransport { Server = "10.0.0.1", Port = 502 };
    if (tcp.LogPrefix != "[Tcp:10.0.0.1:502]") throw new Exception("TCP 前缀不匹配: " + tcp.LogPrefix);

    using var http = new HttpTransport { Address = "http://localhost:8080" };
    if (http.LogPrefix != "[Http:http://localhost:8080]") throw new Exception("HTTP 前缀不匹配: " + http.LogPrefix);
});

Run("网络驱动参数", () =>
{
    DriverFactory.Register<IoTTcpDriver>("IoTTcp");
    var socket = DriverFactory.CreateParameter("IoTTcp", """{"Server":"192.168.1.10","Port":502,"Timeout":1500}""");
    if (socket is not SocketParameter sp) throw new Exception("TCP 参数创建失败");
    if (sp.Server != "192.168.1.10" || sp.Port != 502 || sp.Timeout != 1500) throw new Exception("TCP 参数值不匹配");

    DriverFactory.Register<IoTHttpDriver>("IoTHttp");
    var http = DriverFactory.CreateParameter("IoTHttp", """{"Address":"http://localhost:8080","PathAndQuery":"/api"}""");
    if (http is not HttpParameter hp) throw new Exception("HTTP 参数创建失败");
    if (hp.Address != "http://localhost:8080" || hp.PathAndQuery != "/api") throw new Exception("HTTP 参数值不匹配");
});

Run("结果类型 JSON", () =>
{
    var wr = WriteResult.Fail(IoTErrorCode.Timeout, "Node or parameter is invalid.");
    var json = wr.ToJson();
    var raw = JsonHelper.Default.Parse(json) ?? throw new Exception("WriteResult JSON 解析失败");
    var back = JsonHelper.Default.Convert(raw, typeof(WriteResult));
    if (back is not WriteResult wr2) throw new Exception("WriteResult 反序列化失败");
    if (wr2.Code != IoTErrorCode.Timeout) throw new Exception("WriteResult 错误码不匹配: " + wr2.Code);
});

Run("AOT 插件扫描守卫", () =>
{
    // JIT 下跳过真实扫描，避免加载全部程序集；AOT 下 ScanAll 应直接返回
    if (System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported) return;

    ScanAllForAotGuard();
});

Run("串口实现加载", () =>
{
    var portName = OperatingSystem.IsWindows() ? "COM255" : "/dev/ttyNONEXISTENT";
    using var port = new NewLife.IoT.Controllers.DefaultSerialPort { PortName = portName, Baudrate = 9600 };

    try
    {
        port.Open();
    }
    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or PlatformNotSupportedException)
    {
        // 预期：测试环境不存在该串口
    }
});

Run("数据库参数", () =>
{
    using var driver = new IoTDatabaseDriver();
    var parameter = driver.CreateParameter("""{"ConnectionString":"Data Source=:memory:","DbType":"SQLite"}""");
    if (parameter is not DatabaseParameter p) throw new Exception("参数创建失败");
    if (p.ConnectionString != "Data Source=:memory:") throw new Exception("连接字符串不匹配");
});

Console.WriteLine(failed == 0 ? "ALL PASS" : $"FAILED {failed}");
return failed == 0 ? 0 : 1;

[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "冒烟件显式验证 AOT 守卫，调用前已判断 IsDynamicCodeSupported")]
[UnconditionalSuppressMessage("Aot", "IL3050", Justification = "冒烟件显式验证 AOT 守卫，调用前已判断 IsDynamicCodeSupported")]
static void ScanAllForAotGuard() => DriverFactory.ScanAll();

/// <summary>内存日志器。用于验证带前缀日志</summary>
sealed class MemoryLog : Logger
{
    public List<String> Messages { get; } = [];

    protected override void OnWrite(LogLevel level, String format, params Object?[] args)
        => Messages.Add(Logger.FormatMessage(this, format, args!));
}
