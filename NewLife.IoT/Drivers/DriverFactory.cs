using System.Collections.Concurrent;
using System.Reflection;
using NewLife.IoT.ThingModels;
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
        foreach (var item in AssemblyX.FindAllPlugins(typeof(IDriver), true, true))
        {
            var att = item.GetCustomAttribute<DriverAttribute>();
            var name = att?.Name ?? item.Name.TrimEnd("Procotol");

            XTrace.WriteLine("加载驱动 [{0}] {1} {2}", name, item.FullName, item.GetDisplayName());

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
                var pm = drv?.GetDefaultParameter();
                if (pm != null)
                {
                    // Xml序列化，去掉前面的BOM编码
                    info.DefaultParameter = pm.ToXml(null, true).Trim((Char)0xFEFF);

                    info.Specification = drv?.GetSpecification();
                }
            }
            catch { }

            list.Add(info);
        }

        Drivers = list;

        XTrace.WriteLine("================结束扫描驱动插件================");
    }
    #endregion
}