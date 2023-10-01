using NewLife.IoT.ThingSpecification;

namespace NewLife.IoT.Drivers;

/// <summary>驱动信息</summary>
public class DriverInfo
{
    #region 属性
    /// <summary>名称</summary>
    public String Name { get; set; } = null!;

    /// <summary>显示名</summary>
    public String? DisplayName { get; set; }

    /// <summary>类型。编程语言等，例如.NET</summary>
    public String? Type { get; set; }

    /// <summary>类型名</summary>
    public String? ClassName { get; set; }

    /// <summary>驱动版本</summary>
    public String? Version { get; set; }

    /// <summary>该驱动所依赖的IoT版本</summary>
    public String? IoTVersion { get; set; }

    /// <summary>描述</summary>
    public String? Description { get; set; }

    /// <summary>默认参数。可作为设备参数模版，Xml格式带注释</summary>
    public String? DefaultParameter { get; set; }

    /// <summary>产品物模型。如果设备有固定点位属性、服务和事件，则直接返回，否则返回空</summary>
    public ThingSpec? Specification { get; set; }
    #endregion

    /// <summary>友好显示名称</summary>
    /// <returns></returns>
    public override String ToString() => !DisplayName.IsNullOrEmpty() ? DisplayName : Name;
}