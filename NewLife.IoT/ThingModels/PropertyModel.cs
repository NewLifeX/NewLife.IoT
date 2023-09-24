namespace NewLife.IoT.ThingModels;

/// <summary>设备属性模型</summary>
public class PropertyModel
{
    /// <summary>名称</summary>
    public String Name { get; set; }

    /// <summary>数值</summary>
    public Object Value { get; set; }

    /// <summary>启用状态 true 启用 false 停用</summary>
    public Boolean Enable { get; set; }
}