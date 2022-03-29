namespace NewLife.IoT.Drivers;

/// <summary>驱动特性</summary>
public class DriverAttribute : Attribute
{
    /// <summary>
    /// 名称
    /// </summary>
    public String Name { get; set; }

    /// <summary>
    /// 指定驱动名称
    /// </summary>
    /// <param name="name"></param>
    public DriverAttribute(String name) => Name = name;
}