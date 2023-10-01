namespace NewLife.IoT.ThingModels;

/// <summary>点位模型</summary>
public class PointModel : IPoint
{
    /// <summary>名称</summary>
    public String? Name { get; set; }

    /// <summary>地址。表示点位的地址，具体含义由设备驱动决定。例如常规地址6，字母地址DI07，Modbus地址4x0015，位域地址D012.05，比特位置0~15</summary>
    /// <remarks>在某些场景中，特殊点位地址（如#）表示虚拟地址，该点位数值由ReadRule表达式动态计算得到，点位信息并不会传递给驱动层</remarks>
    public String? Address { get; set; }

    /// <summary>数据类型。来自物模型</summary>
    public String? Type { get; set; }

    /// <summary>大小。数据字节数，或字符串长度，Modbus寄存器一般占2个字节</summary>
    public Int32 Length { get; set; }

    /// <summary>读取规则。数据解析规则，表达式或脚本</summary>
    public String? ReadRule { get; set; }

    /// <summary>写入规则。数据反解析规则，表达式或脚本</summary>
    public String? WriteRule { get; set; }

    /// <summary>已重载。友好显示</summary>
    /// <returns></returns>
    public override String ToString() => $"{Name} {Type} {Address}";
}