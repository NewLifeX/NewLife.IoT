using System.Runtime.Serialization;
using System.Xml.Serialization;
using NewLife.IoT.ThingModels;

namespace NewLife.IoT.Drivers;

/// <summary>驱动信息</summary>
public class DriverInfo
{
    #region 属性
    /// <summary>名称</summary>
    public String Name { get; set; }

    /// <summary>显示名</summary>
    public String DisplayName { get; set; }

    /// <summary>类型</summary>
    [XmlIgnore, IgnoreDataMember]
    public Type Type { get; set; }

    /// <summary>类型名</summary>
    public String ClassName { get; set; }

    /// <summary>驱动版本</summary>
    public String Version { get; set; }

    /// <summary>该驱动所依赖的IoT版本</summary>
    public String IoTVersion { get; set; }

    /// <summary>描述</summary>
    public String Description { get; set; }

    /// <summary>默认参数。可作为设备参数模版，Xml格式带注释</summary>
    public String DefaultParameter { get; set; }

    /// <summary>默认点位。某些设备驱动中指定了该类设备所拥有的点位信息</summary>
    public IPoint[] DefaultPoints { get; set; }
    #endregion
}