namespace NewLife.IoT.Drivers;

/// <summary>
/// 驱动参数接口。控制设备驱动的参数，可转为字典便于传输与存储
/// </summary>
public interface IDriverParameter
{
}

/// <summary>默认驱动参数实现</summary>
public class DriverParameter : IDriverParameter { }

/// <summary>
/// 驱动参数扩展
/// </summary>
public static class DriverParameterExtensions
{
    /// <summary>
    /// 序列化参数对象为名值对
    /// </summary>
    /// <param name="driverParameter"></param>
    /// <returns></returns>
    public static IDictionary<String, Object> Serialize(this IDriverParameter driverParameter) => driverParameter.ToDictionary();
}