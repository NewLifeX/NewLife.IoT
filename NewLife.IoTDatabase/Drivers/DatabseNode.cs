using NewLife.IoT.Drivers;
using XCode.DataAccessLayer;

namespace NewLife.IoTDatabase.Drivers;

/// <summary>
/// 数据库节点。多设备共用驱动时，以节点区分
/// </summary>
public class DatabseNode : Node
{
    /// <summary>客户端</summary>
    public DAL Dal { get; set; } = null!;

    /// <summary>
    /// 参数。设备使用的专用参数
    /// </summary>
    public DatabaseParameter DatabaseParameter { get; set; }
}