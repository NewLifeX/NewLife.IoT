using NewLife.IoT.ThingSpecification;
using NewLife.Reflection;

namespace NewLife.IoT.ThingModels;

/// <summary>点位</summary>
public interface IPoint
{
    /// <summary>名称</summary>
    String? Name { get; set; }

    /// <summary>地址。表示点位的地址，具体含义由设备驱动决定。例如常规地址6，字母地址DI07，Modbus地址4x0015，位域地址D012.05，比特位置0~15</summary>
    /// <remarks>在某些场景中，特殊点位地址（如#）表示虚拟地址，该点位数值由ReadRule表达式动态计算得到，点位信息并不会传递给驱动层</remarks>
    String? Address { get; set; }

    /// <summary>数据类型。来自物模型</summary>
    String? Type { get; set; }

    /// <summary>大小。数据字节数，或字符串长度，Modbus寄存器一般占2个字节</summary>
    Int32 Length { get; set; }
}

/// <summary>点位扩展</summary>
public static class PointHelper
{
    /// <summary>
    /// 根据点位类型长度，解析字节数组为目标类型。默认小端字节序，大端需要用Swap提前处理
    /// </summary>
    /// <param name="point">点位</param>
    /// <param name="data">字节数据</param>
    /// <returns></returns>
    public static Object Convert(this IPoint point, Byte[] data)
    {
        var type = point.GetNetType() ?? throw new NotSupportedException();
        if (type == typeof(Byte[])) return data;

        return type.GetTypeCode() switch
        {
            TypeCode.Boolean => BitConverter.ToBoolean(data, 0),
            TypeCode.Byte => data[0],
            TypeCode.Char => BitConverter.ToChar(data, 0),
            TypeCode.Double => BitConverter.ToDouble(data, 0),
            TypeCode.Int16 => BitConverter.ToInt16(data, 0),
            TypeCode.Int32 => BitConverter.ToInt32(data, 0),
            TypeCode.Int64 => BitConverter.ToInt64(data, 0),
            TypeCode.Single => BitConverter.ToSingle(data, 0),
            TypeCode.UInt16 => BitConverter.ToUInt16(data, 0),
            TypeCode.UInt32 => BitConverter.ToUInt32(data, 0),
            TypeCode.UInt64 => BitConverter.ToUInt64(data, 0),
            _ => throw new NotSupportedException(),
        };
    }

    /// <summary>
    /// 根据点位类型长度，把目标对象转为字节数组。默认小端字节序，大端需要对返回值用Swap处理
    /// </summary>
    /// <param name="point">点位</param>
    /// <param name="value">数据对象</param>
    /// <returns></returns>
    public static Byte[]? GetBytes(this IPoint point, Object value)
    {
        var type = point.GetNetType() ?? throw new NotSupportedException();
        if (type == typeof(Byte[]) && value is Byte[] data) return data;

        var val = value.ChangeType(type);

        return type.GetTypeCode() switch
        {
            TypeCode.Boolean => BitConverter.GetBytes((Boolean)(val ?? false)),
            TypeCode.Byte => [(Byte)(val ?? 0)],
            TypeCode.Char => BitConverter.GetBytes((Char)(val ?? 0)),
            TypeCode.Double => BitConverter.GetBytes((Double)(val ?? 0)),
            TypeCode.Int16 => BitConverter.GetBytes((Int16)(val ?? 0)),
            TypeCode.Int32 => BitConverter.GetBytes((Int32)(val ?? 0)),
            TypeCode.Int64 => BitConverter.GetBytes((Int64)(val ?? 0)),
            TypeCode.Single => BitConverter.GetBytes((Single)(val ?? 0)),
            TypeCode.UInt16 => BitConverter.GetBytes((UInt16)(val ?? 0)),
            TypeCode.UInt32 => BitConverter.GetBytes((UInt32)(val ?? 0)),
            TypeCode.UInt64 => BitConverter.GetBytes((UInt64)(val ?? 0)),
            _ => null,
        };
    }

    /// <summary>根据点位信息和物模型信息，把原始数据转为线圈/位</summary>
    /// <remarks>一般用在向设备写入点位数据之前，例如Modbus.WriteCoil</remarks>
    /// <param name="point">点位</param>
    /// <param name="data">原始数据，一般是字符串</param>
    /// <param name="spec">物模型</param>
    /// <returns></returns>
    public static UInt16[]? ConvertToBit(this IPoint point, Object data, ThingSpec? spec = null)
    {
        var type = TypeHelper.GetNetType(point);
        if (type == null)
        {
            // 找到物属性定义
            var pi = spec?.GetProperty(point.Name);
            type = TypeHelper.GetNetType(pi?.DataType?.Type);
        }
        if (type == null) return null;

        return type.GetTypeCode() switch
        {
            TypeCode.Boolean or TypeCode.Byte or TypeCode.SByte => data.ToBoolean() ? [0x01] : [0x00],
            TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 => data.ToInt() > 0 ? [0x01] : [0x00],
            TypeCode.Int64 or TypeCode.UInt64 => data.ToLong() > 0 ? [0x01] : [0x00],
            _ => data.ToBoolean() ? [0x01] : [0x00],
        };
    }

    /// <summary>根据点位信息和物模型信息，把原始数据转寄存器/字</summary>
    /// <remarks>一般用在向设备写入点位数据之前，例如Modbus.WriteRegister</remarks>
    /// <param name="point">点位</param>
    /// <param name="data">原始数据，一般是字符串</param>
    /// <param name="spec">物模型</param>
    /// <returns>返回短整型数组，有可能一个整数拆分为双字</returns>
    public static UInt16[]? ConvertToWord(this IPoint point, Object data, ThingSpec? spec = null)
    {
        var type = TypeHelper.GetNetType(point);
        if (type == null)
        {
            // 找到物属性定义
            var pi = spec?.GetProperty(point.Name);
            type = TypeHelper.GetNetType(pi?.DataType?.Type);
        }
        if (type == null) return null;

        switch (type.GetTypeCode())
        {
            case TypeCode.Boolean:
            case TypeCode.Byte:
            case TypeCode.SByte:
                return data.ToBoolean() ? [0x01] : [0x00];
            case TypeCode.Int16:
            case TypeCode.UInt16:
                return [(UInt16)data.ToInt()];
            case TypeCode.Int32:
            case TypeCode.UInt32:
                {
                    var n = data.ToInt();
                    return [(UInt16)(n >> 16), (UInt16)(n & 0xFFFF)];
                }
            case TypeCode.Int64:
            case TypeCode.UInt64:
                {
                    var n = data.ToLong();
                    return [(UInt16)(n >> 48), (UInt16)(n >> 32), (UInt16)(n >> 16), (UInt16)(n & 0xFFFF)];
                }
            case TypeCode.Single:
                {
                    var d = (Single)data.ToDouble();
                    //var n = BitConverter.SingleToInt32Bits(d);
                    var n = (UInt32)d;
                    return [(UInt16)(n >> 16), (UInt16)(n & 0xFFFF)];
                }
            case TypeCode.Double:
                {
                    var d = (Double)data.ToDouble();
                    //var n = BitConverter.DoubleToInt64Bits(d);
                    var n = (UInt64)d;
                    return [(UInt16)(n >> 48), (UInt16)(n >> 32), (UInt16)(n >> 16), (UInt16)(n & 0xFFFF)];
                }
            case TypeCode.Decimal:
                {
                    var d = data.ToDecimal();
                    var n = (UInt64)d;
                    return [(UInt16)(n >> 48), (UInt16)(n >> 32), (UInt16)(n >> 16), (UInt16)(n & 0xFFFF)];
                }
            //case TypeCode.String:
            //    break;
            default:
                return null;
        }
    }
}