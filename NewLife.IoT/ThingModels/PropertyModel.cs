namespace NewLife.IoT.ThingModels;

/// <summary>设备属性模型</summary>
public class PropertyModel
{
    /// <summary>名称</summary>
    public String Name { get; set; } = null!;

    /// <summary>数值</summary>
    public Object? Value { get; set; }
}