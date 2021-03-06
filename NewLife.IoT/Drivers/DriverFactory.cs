using System.Collections.Concurrent;
using System.Reflection;
using NewLife.Log;
using NewLife.Reflection;
using NewLife.Xml;

namespace NewLife.IoT.Drivers;

/// <summary>驱动工厂。根据协议名称创建实例</summary>
public class DriverFactory
{
    #region 工厂
    private static readonly Dictionary<String, Type> _map = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>驱动表</summary>
    public static IDictionary<String, Type> Map => _map;

    /// <summary>驱动集合</summary>
    public static IList<DriverInfo> Drivers { get; private set; }

    /// <summary>注册协议实现</summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    public static void Register(String name, Type type)
    {
        if (name.IsNullOrEmpty()) name = type.Name;

        _map[name] = type;
    }

    /// <summary>注册协议实现</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    public static void Register<T>(String name = null) where T : IDriver => Register(name, typeof(T));

    private static readonly ConcurrentDictionary<String, IDriver> _cache = new();
    /// <summary>创建协议实例，根据地址确保唯一，多设备共用同一个串口</summary>
    /// <param name="name">名称</param>
    /// <param name="identifier">唯一标识</param>
    /// <returns></returns>
    public static IDriver Create(String name, String identifier)
    {
        if (!_map.TryGetValue(name, out var type)) return null;

        var key = $"{name}-{identifier}";
        return _cache.GetOrAdd(key, k => type.CreateInstance() as IDriver);
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
#if NETSTANDARD2_1_OR_GREATER
        foreach (var item in AssemblyX.FindAllPlugins(typeof(IDriver), true, true))
#else
        foreach (var item in FindAllPlugins(typeof(IDriver), true, true))
#endif
        {
            var att = item.GetCustomAttribute<DriverAttribute>();
            var name = att?.Name ?? item.Name.TrimEnd("Procotol");

            XTrace.WriteLine("加载驱动 [{0}] {1}", name, item.FullName);

            Register(name, item);

            var info = new DriverInfo
            {
                Name = name,
                DisplayName = item.GetDisplayName(),
                Type = item,
                ClassName = item.FullName,
                Version = item.Assembly.GetName().Version + "",
                IoTVersion = iotVersion,
                Description = item.GetDescription(),
            };

            try
            {
                var drv = item.CreateInstance() as IDriver;
                var pm = drv?.CreateParameter();
                if (pm != null)
                {
                    info.DefaultParameter = pm.ToXml(null, true).Trim((Char)0xFEFF);
                }
            }
            catch { }

            list.Add(info);
        }

        Drivers = list;

        XTrace.WriteLine("================结束扫描驱动插件================");
    }

    /// <summary>查找所有非系统程序集中的所有插件</summary>
    /// <remarks>继承类所在的程序集会引用baseType所在的程序集，利用这一点可以做一定程度的性能优化。</remarks>
    /// <param name="baseType"></param>
    /// <param name="isLoadAssembly">是否从未加载程序集中获取类型。使用仅反射的方法检查目标类型，如果存在，则进行常规加载</param>
    /// <param name="excludeGlobalTypes">指示是否应检查来自所有引用程序集的类型。如果为 false，则检查来自所有引用程序集的类型。 否则，只检查来自非全局程序集缓存 (GAC) 引用的程序集的类型。</param>
    /// <returns></returns>
    private static IEnumerable<Type> FindAllPlugins(Type baseType, Boolean isLoadAssembly = false, Boolean excludeGlobalTypes = true)
    {
        var baseAssemblyName = baseType.Assembly.GetName().Name;

        // 如果基类所在程序集没有强命名，则搜索时跳过所有强命名程序集
        // 因为继承类程序集的强命名要求基类程序集必须强命名
        var signs = baseType.Assembly.GetName().GetPublicKey();
        var hasNotSign = signs == null || signs.Length <= 0;

        var list = new List<Type>();
        foreach (var item in AssemblyX.GetAssemblies())
        {
            signs = item.Asm.GetName().GetPublicKey();
            if (hasNotSign && signs != null && signs.Length > 0) continue;

            // 如果excludeGlobalTypes为true，则指检查来自非GAC引用的程序集
            if (excludeGlobalTypes && item.Asm.GlobalAssemblyCache) continue;

            // 不搜索系统程序集，不搜索未引用基类所在程序集的程序集，优化性能
            if (item.IsSystemAssembly || !IsReferencedFrom(item.Asm, baseAssemblyName)) continue;

            var ts = FindPlugins(baseType, item.Types);
            foreach (var elm in ts)
            {
                if (!list.Contains(elm))
                {
                    list.Add(elm);
                    yield return elm;
                }
            }
        }
        if (isLoadAssembly)
        {
            var baseType2 = baseType;
#if NET40_OR_GREATER
            try
            {
                var asm = Assembly.ReflectionOnlyLoadFrom(baseType.Assembly.Location);

                // 基类也改为只反射，否则判断某类是否集成指定基类时，一个反射一个正常加载无法通过
                baseType2 = asm.GetType(baseType.FullName);
                if (baseType2 == null) yield break;
            }
            catch { yield break; }
#endif

            foreach (var item in AssemblyX.ReflectionOnlyGetAssemblies())
            {
                // 如果excludeGlobalTypes为true，则指检查来自非GAC引用的程序集
                if (excludeGlobalTypes && item.Asm.GlobalAssemblyCache) continue;

                // 不搜索系统程序集，不搜索未引用基类所在程序集的程序集，优化性能
                if (item.IsSystemAssembly || !IsReferencedFrom(item.Asm, baseAssemblyName)) continue;

                var ts = FindPlugins(baseType2, item.Types);
                if (ts.Any())
                {
                    // 真实加载
                    if (XTrace.Debug)
                    {
                        // 如果是本目录的程序集，去掉目录前缀
                        var file = item.Asm.Location;
                        var root = ".".GetFullPath();
                        if (file.StartsWithIgnoreCase(root)) file = file.Substring(root.Length).TrimStart("\\");
                        XTrace.WriteLine("AssemblyX.FindAllPlugins(\"{0}\") => {1}", baseType.FullName, file);
                    }
                    try
                    {
                        var asm = Assembly.LoadFrom(item.Asm.Location);
                        ts = FindPlugins(baseType, asm.GetTypes());
                    }
                    catch { continue; }

                    foreach (var elm in ts)
                    {
                        if (!list.Contains(elm))
                        {
                            list.Add(elm);
                            yield return elm;
                        }
                    }
                }
            }
        }
    }

    /// <summary><paramref name="asm"/> 是否引用了 <paramref name="baseAsmName"/></summary>
    /// <param name="asm">程序集</param>
    /// <param name="baseAsmName">被引用程序集全名</param>
    /// <returns></returns>
    private static Boolean IsReferencedFrom(Assembly asm, String baseAsmName)
    {
        //if (asm.FullName.EqualIgnoreCase(baseAsmName)) return true;
        if (asm.GetName().Name.EqualIgnoreCase(baseAsmName)) return true;

        foreach (var item in asm.GetReferencedAssemblies())
        {
            //if (item.FullName.EqualIgnoreCase(baseAsmName)) return true;
            if (item.Name.EqualIgnoreCase(baseAsmName)) return true;
        }

        return false;
    }

    /// <summary>查找插件，带缓存</summary>
    /// <param name="baseType">类型</param>
    /// <param name="types">所有类型</param>
    /// <returns></returns>
    static IEnumerable<Type> FindPlugins(Type baseType, IEnumerable<Type> types)
    {
        foreach (var item in types)
        {
            if (item.IsInterface || item.IsAbstract || item.IsGenericType) continue;
            if (item != baseType && item.As(baseType)) yield return item;
        }
    }
    #endregion
}