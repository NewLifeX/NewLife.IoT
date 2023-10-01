using System.Collections.Concurrent;
using System.Reflection;
using NewLife.Log;
using NewLife.Reflection;

namespace NewLife.IoT.Drivers;

/// <summary>驱动工厂。根据协议名称创建实例</summary>
public class DriverFactory
{
    #region 工厂
    private static readonly Dictionary<String, Type> _map = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>驱动表</summary>
    public static IDictionary<String, Type> Map => _map;

    /// <summary>驱动集合</summary>
    public static IList<DriverInfo> Drivers { get; private set; } = null!;

    /// <summary>注册协议实现</summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    public static void Register(String? name, Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (name.IsNullOrEmpty()) name = type.Name;

        _map[name] = type;
    }

    /// <summary>注册协议实现</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    public static void Register<T>(String? name = null) where T : IDriver => Register(name, typeof(T));

    private static readonly ConcurrentDictionary<String, IDriver?> _cache = new();
    /// <summary>创建协议实例，根据地址确保唯一，多设备共用同一个串口</summary>
    /// <param name="name">驱动名称。一般由DriverAttribute特性确定</param>
    /// <param name="identifier">唯一标识。没有传递标识时，每次返回新实例</param>
    /// <returns></returns>
    public static IDriver? Create(String name, String identifier)
    {
        if (!_map.TryGetValue(name, out var type) || type == null) return null;

        // 没有传递标识时，每次返回新实例
        if (identifier.IsNullOrEmpty()) return type.CreateInstance() as IDriver;

        var key = $"{name}-{identifier}";
        return _cache.GetOrAdd(key, k => type.CreateInstance() as IDriver);
    }

    private static readonly ConcurrentDictionary<String, IDriver?> _defaults = new();
    /// <summary>创建驱动参数对象，分析参数配置或创建默认参数</summary>
    /// <param name="name">驱动名称。一般由DriverAttribute特性确定</param>
    /// <param name="parameter">Xml/Json参数配置</param>
    /// <returns></returns>
    public static IDriverParameter? CreateParameter(String name, String parameter)
    {
        if (!_map.TryGetValue(name, out var type) || type == null) return null;

        var driver = _defaults.GetOrAdd(name, k => type.CreateInstance() as IDriver);
        return driver?.CreateParameter(parameter);
    }
    #endregion

    #region 插件
    /// <summary>
    /// 扫描所有程序集，加载插件
    /// </summary>
    public static void ScanAll()
    {
        XTrace.WriteLine("================开始扫描驱动插件================");

        var iot = Assembly.GetExecutingAssembly();
        var iotVersion = iot.GetName().Version + "";

        var list = new List<DriverInfo>();
        foreach (var type in AssemblyX.FindAllPlugins(typeof(IDriver), true, true))
        {
            var att = type.GetCustomAttribute<DriverAttribute>();
            var name = att?.Name ?? type.Name.TrimEnd("Procotol", "Driver");

            XTrace.WriteLine("加载驱动 [{0}]\t{1}\t{2}", name, type.FullName, type.GetDisplayName());

            Register(name, type);

            var info = new DriverInfo
            {
                Name = name,
                DisplayName = type.GetDisplayName(),
                Type = ".NET",
                ClassName = type.FullName,
                Version = type.Assembly.GetName().Version + "",
                IoTVersion = iotVersion,
                Description = type.GetDescription(),
            };

            try
            {
                var drv = type.CreateInstance() as IDriver;

                // Xml序列化，去掉前面的BOM编码
                info.DefaultParameter = drv?.CreateParameter()?.EncodeParameter();

                info.Specification = drv?.GetSpecification();
            }
            catch { }

            list.Add(info);
        }

        Drivers = list;

        XTrace.WriteLine("================结束扫描驱动插件================");
    }
    #endregion
}