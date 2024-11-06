namespace NewLife.IoT.Drivers;

/// <summary>驱动特性</summary>
/// <param name="name">驱动名称</param>
public class DriverAttribute(String name) : Attribute
{
    /// <summary>名称</summary>
    public String Name { get; set; } = name;
}