using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using NewLife.Log;
using NewLife.Reflection;

namespace NewLife.IoT;

/// <summary>数据处理助手</summary>
public static class DataHelper
{
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
                var v = (Int64)Math.Round((data.ToLong() - pt.Constant) / scaling);

                // 常见的2字节和4字节整型，直接转字节数组返回
                var rs = type.GetTypeCode() switch
                {
                    // Swap16为false表示大端
                    TypeCode.Int16 or TypeCode.UInt16 => ((UInt16)v).GetBytes(pt.Swap16),
                    // 先按照小端读取出来，如果Swap16/Swap32是大端false，则需要交换字节序
                    TypeCode.Int32 or TypeCode.UInt32 => ((UInt32)v).GetBytes().Swap(!pt.Swap16, !pt.Swap32),
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
