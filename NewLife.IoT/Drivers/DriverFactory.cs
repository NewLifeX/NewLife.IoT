using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NewLife.Log;
using NewLife.Reflection;

namespace NewLife.IoT.Drivers;

/// <summary>驱动工厂。根据协议名称创建实例</summary>
public class DriverFactory
{
    #region 工厂
    private static readonly ConcurrentDictionary<String, Type> _map = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<String, Func<IDriver>> _factories = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>驱动表</summary>
    public static IDictionary<String, Type> Map => _map;

    /// <summary>驱动集合</summary>
    public static IList<DriverInfo> Drivers { get; private set; } = null!;

    /// <summary>注册协议实现</summary>
    /// <param name="name"></param>
    /// <param name="type">驱动类型。裁剪器保留公开无参构造，供 Activator 创建实例</param>
    public static void Register(String? name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type), L.T("Parameter cannot be null."));
        if (name.IsNullOrEmpty()) name = type.Name;

        _map[name] = type;
        _factories[name] = () => (IDriver)Activator.CreateInstance(type)!;
    }

    /// <summary>注册协议实现</summary>
    /// <param name="name">驱动名称。为空时使用类型名</param>
    /// <param name="type">驱动类型</param>
    /// <param name="factory">驱动工厂。NativeAOT 下推荐此重载，避免依赖运行期反射创建实例</param>
    public static void Register(String? name, Type type, Func<IDriver> factory)
    {
        if (type == null) throw new ArgumentNullException(nameof(type), L.T("Parameter cannot be null."));
        if (factory == null) throw new ArgumentNullException(nameof(factory), L.T("Parameter cannot be null."));
        if (name.IsNullOrEmpty()) name = type.Name;

        _map[name] = type;
        _factories[name] = factory;
    }

    /// <summary>注册协议实现。使用泛型约束生成直接构造调用，NativeAOT 下可正常创建实例</summary>
    /// <typeparam name="T">驱动类型</typeparam>
    /// <param name="name">驱动名称。为空时使用类型名</param>
    public static void Register<T>(String? name = null) where T : IDriver, new()
        => Register(name, typeof(T), () => new T());

    private static readonly ConcurrentDictionary<String, IDriver?> _cache = new();
    /// <summary>创建协议实例，根据地址确保唯一，多设备共用同一个串口</summary>
    /// <param name="name">驱动名称。一般由DriverAttribute特性确定</param>
    /// <param name="identifier">唯一标识。没有传递标识时，每次返回新实例</param>
    /// <returns></returns>
    public static IDriver? Create(String name, String identifier)
    {
        if (!_map.TryGetValue(name, out var type) || type == null) return null;

        var factory = _factories.TryGetValue(name, out var f) ? f : null;
        IDriver? NewInstance() => factory != null ? factory() : type.CreateInstance() as IDriver;

        // 没有传递标识时，每次返回新实例
        if (identifier.IsNullOrEmpty()) return NewInstance();

        var key = $"{name}-{identifier}";
        return _cache.GetOrAdd(key, k => NewInstance());
    }

    private static readonly ConcurrentDictionary<String, IDriver?> _defaults = new();
    /// <summary>创建驱动参数对象，分析参数配置或创建默认参数</summary>
    /// <param name="name">驱动名称。一般由DriverAttribute特性确定</param>
    /// <param name="parameter">Xml/Json参数配置</param>
    /// <returns></returns>
    public static IDriverParameter? CreateParameter(String name, String parameter)
    {
        if (!_map.TryGetValue(name, out var type) || type == null) return null;

        var driver = _defaults.GetOrAdd(name, k => _factories.TryGetValue(k, out var f) ? f() : type.CreateInstance() as IDriver);
        return driver?.CreateParameter(parameter);
    }
    #endregion

    #region 插件
    /// <summary>
    /// 扫描所有程序集，加载插件
    /// </summary>
    /// <remarks>依赖程序集与类型反射，NativeAOT 下不支持；AOT 应用请使用 <see cref="Register{T}(String?)"/> 显式注册。</remarks>
    [RequiresUnreferencedCode("插件扫描依赖程序集与类型反射，NativeAOT 下请使用 Register<T> 显式注册。")]
    [RequiresDynamicCode("插件实例化依赖运行期类型创建，NativeAOT 下不可用。")]
    public static void ScanAll()
    {
#if NET5_0_OR_GREATER
        if (!System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
        {
            XTrace.WriteLine("Driver plugin scanning is not supported under NativeAOT. Register drivers explicitly with DriverFactory.Register<T>.");
            return;
        }
#endif

        XTrace.WriteLine("================Start scanning driver plugins================");

        var iot = Assembly.GetExecutingAssembly();
        var iotVersion = iot.GetName().Version + "";

        var list = new List<DriverInfo>();
        foreach (var type in AssemblyX.FindAllPlugins(typeof(IDriver), true, true))
        {
            var att = type.GetCustomAttribute<DriverAttribute>();
            var name = att?.Name ?? type.Name.TrimSuffix("Protocol", "Driver");

            var name2 = $"[{name}]";
            XTrace.WriteLine("{0,16}\t{1,-14}\t{2}", name2, type.GetDisplayName(), type.FullName);

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
                var driver = type.CreateInstance() as IDriver;
                var driverParameter = driver?.CreateParameter();
                info.ParameterClassName = driverParameter?.GetType().FullName;

                // Xml序列化，去掉前面的BOM编码
                info.DefaultParameter = driverParameter?.EncodeParameter();

                info.Specification = driver?.GetSpecification();
            }
            catch { }

            list.Add(info);
        }

        Drivers = list;

        XTrace.WriteLine("================End scanning driver plugins================");
    }
    #endregion
}