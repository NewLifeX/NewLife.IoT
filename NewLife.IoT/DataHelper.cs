using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using NewLife.Log;
using NewLife.Reflection;

namespace NewLife.IoT;

/// <summary>数据处理助手</summary>
public static class DataHelper
{
    /// <summary>短整数转为指定字节序的字节数组，仅大小端两种</summary>
    /// <param name="value"></param>
    /// <param name="endian"></param>
    /// <returns></returns>
    public static Byte[] GetBytes(this UInt16 value, EndianType endian) => value.GetBytes(endian is EndianType.LittleEndian or EndianType.LittleSwap);

    /// <summary>短整数转为指定字节序的字节数组，仅AB/BA两种</summary>
    /// <param name="value"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    public static Byte[] GetBytes(this UInt16 value, ByteOrder order) => value.GetBytes(order is ByteOrder.DCBA or ByteOrder.CDAB);

    /// <summary>整数转为指定字节序的字节数组</summary>
    /// <param name="value"></param>
    /// <param name="endian"></param>
    /// <returns></returns>
    public static Byte[] GetBytes(this UInt32 value, EndianType endian)
    {
        var buf = value.GetBytes(endian is EndianType.LittleEndian or EndianType.LittleSwap);
        if (endian is EndianType.BigSwap or EndianType.LittleSwap)
        {
#if NET40
            var tmp = buf[0];
            buf[0] = buf[1];
            buf[1] = tmp;

            tmp = buf[2];
            buf[2] = buf[3];
            buf[3] = tmp;
#else
            (buf[0], buf[1]) = (buf[1], buf[0]);
            (buf[2], buf[3]) = (buf[3], buf[2]);
#endif
        }

        return buf;
    }

    /// <summary>按字节序交换字节数组</summary>
    /// <param name="buf"></param>
    /// <param name="endian"></param>
    /// <returns></returns>
    public static Byte[] Swap(this Byte[] buf, EndianType endian)
    {
        var rs = new Byte[buf.Length];
        switch (endian)
        {
            case EndianType.BigEndian:
                break;
            case EndianType.LittleEndian:
                break;
            case EndianType.BigSwap:
                break;
            case EndianType.LittleSwap:
                break;
            default:
                break;
        }

        return rs;
    }

    /// <summary>整数转为指定字节序的字节数组</summary>
    /// <param name="value"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    public static Byte[] GetBytes(this UInt32 value, ByteOrder order) => GetBytes(value, (EndianType)order);

    /// <summary>把点位数据编码成为字节数组。常用于Modbus等协议</summary>
    /// <param name="spec">物模型</param>
    /// <param name="data">数据</param>
    /// <param name="point">点位信息</param>
    /// <returns></returns>
    public static Object? EncodeByThingModel(this ThingSpec spec, Object data, IPoint point)
    {
        // 仅支持数字类型
        var type = point.GetNetType();
        if (type == null || !type.IsNumber()) return data;

        using var span = DefaultTracer.Instance?.NewSpan(nameof(EncodeByThingModel), $"name={point.Name} data={data} type={type.Name} rawType={point.Type}");

        // 找到物属性定义
        var pt = point.Name.IsNullOrEmpty() ? null : spec?.ExtendedProperties?.FirstOrDefault(e => e.Id.EqualIgnoreCase(point.Name));
        if (pt != null)
        {
            // 反向操作常量因子和缩放因子
            if (type.IsInt())
            {
                var scaling = pt.Scaling != 0 ? pt.Scaling : 1;
                var v = data.ToLong();
                //if (pt.Constant != 0 || scaling != 1) v = (Int64)Math.Round(v * scaling + pt.Constant);
                // 编码是反向操作，先减去常量，再除以缩放因子。为了避免精度问题，单精度范围先计算缩放因子倒数，再相乘
                if (pt.Constant != 0 || scaling != 1) v = (Int64)Math.Round(((Double)v - pt.Constant) * (1 / scaling));

                // 常见的2字节和4字节整型，直接转字节数组返回
                var rs = type.GetTypeCode() switch
                {
                    TypeCode.Byte or TypeCode.SByte => new[] { (Byte)v },
                    // Swap16为false表示大端
                    TypeCode.Int16 or TypeCode.UInt16 => ((UInt16)v).GetBytes(pt.Endian),
                    // 先按照小端读取出来，如果Swap16/Swap32是大端false，则需要交换字节序
                    TypeCode.Int32 or TypeCode.UInt32 => ((UInt32)v).GetBytes(pt.Endian),
                    _ => v.ChangeType(type),
                };
                span?.AppendTag($"result={(rs is Byte[] bts ? bts.ToHex() : rs)} v={v} type={type.Name} scaling={scaling}");

                return rs;
            }
            else if (type == typeof(Boolean))
            {
                var rs = data.ToBoolean();
                span?.AppendTag($"result={rs}");

                return rs;
            }
            else
            {
                var v = data.ToDouble();
                if (pt.Constant > 0) v -= pt.Constant;
                if (pt.Scaling != 0) v /= pt.Scaling;

                span?.AppendTag($"result={v} type={type.Name} scaling={pt.Scaling} constant={pt.Constant}");

                return v;
            }
        }

        return data;
    }

    /// <summary>借助物模型解析数值</summary>
    /// <param name="spec">物模型</param>
    /// <param name="data">数据</param>
    /// <param name="point">点位信息</param>
    /// <returns></returns>
    public static Object? DecodeByThingModel(this ThingSpec spec, Byte[] data, IPoint point)
    {
        var type = point.GetNetType();
        if (type == null) return data.ToHex();

        using var span = DefaultTracer.Instance?.NewSpan(nameof(DecodeByThingModel), $"name={point.Name} data={data.ToHex()} type={type.Name} rawType={point.Type}");
        try
        {
            // 找到物属性定义
            var pt = point.Name.IsNullOrEmpty() ? null : spec?.ExtendedProperties?.FirstOrDefault(e => e.Id.EqualIgnoreCase(point.Name));
            if (pt != null)
            {
                if (type == typeof(Boolean)) return data[0];

                // 字节序交换，需要转换为小端字节序，以适应NET中各种转换方法
                // 一般Modbus之类协议使用大端字节序，此时Swap16/Swap32都是false，交换后得到小端字节序，方便使用NET内部转换方法
                data = data.Swap(!pt.Swap16, !pt.Swap32);
                span?.AppendTag($"swap: {data.ToHex()}");

                // 操作常量因子和缩放因子
                if (type.IsNumber())
                {
                    // 2字节小数强行使用UInt16
                    if (type == typeof(Single) && point.Length == 2) type = typeof(UInt16);

                    // 常见的2字节和4字节整型，直接转
                    var scaling = pt.Scaling != 0 ? pt.Scaling : 1;
                    var rs = type.GetTypeCode() switch
                    {
                        TypeCode.Int16 or TypeCode.UInt16 => data.ToUInt16() * scaling + pt.Constant,
                        TypeCode.Int32 or TypeCode.UInt32 => data.ToUInt32() * scaling + pt.Constant,
                        TypeCode.Single => BitConverter.ToSingle(data, 0) * scaling + pt.Constant,
                        TypeCode.Double => BitConverter.ToDouble(data, 0) * scaling + pt.Constant,
                        _ => (Object)(data.ToUInt64() * scaling + pt.Constant),
                    };
                    span?.AppendTag($"result={rs} type={type.Name} scaling={scaling}");

                    return rs;
                }
            }

            return point.Convert(data);
        }
        catch (Exception ex)
        {
            span?.SetError(ex, null);
            //throw;
            return null;
        }
    }
}
