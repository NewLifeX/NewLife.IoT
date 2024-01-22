﻿using NewLife.IoT.ThingModels;
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
    public static Byte[] GetBytes(this UInt16 value, EndianType endian) => value.GetBytes(endian is EndianType.LittleEndian or EndianType.BigSwap);

    /// <summary>短整数转为指定字节序的字节数组，仅AB/BA两种</summary>
    /// <param name="value"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    public static Byte[] GetBytes(this UInt16 value, ByteOrder order) => value.GetBytes(order is ByteOrder.DCBA or ByteOrder.BADC);

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

    /// <summary>整数转为指定字节序的字节数组</summary>
    /// <param name="value"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    public static Byte[] GetBytes(this UInt32 value, ByteOrder order) => GetBytes(value, (EndianType)order);

    /// <summary>字节数组按照指定字节序转为短整数，仅大小端两种</summary>
    /// <param name="buffer"></param>
    /// <param name="endian"></param>
    /// <returns></returns>
    public static UInt16 ToUInt16(this Byte[] buffer, EndianType endian) => buffer.ToUInt16(0, endian is EndianType.LittleEndian or EndianType.BigSwap);

    /// <summary>字节数组按照指定字节序转为短整数，仅AB/BA两种</summary>
    /// <param name="buffer"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    public static UInt16 ToUInt16(this Byte[] buffer, ByteOrder order) => buffer.ToUInt16(0, order is ByteOrder.DCBA or ByteOrder.BADC);

    /// <summary>字节数组按照指定字节序转为整数</summary>
    /// <param name="buffer"></param>
    /// <param name="endian"></param>
    /// <returns></returns>
    public static UInt32 ToUInt32(this Byte[] buffer, EndianType endian)
    {
        if (endian is EndianType.BigSwap or EndianType.LittleSwap)
        {
            var buf = buffer;
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

        return buffer.ToUInt32(0, endian is EndianType.LittleEndian or EndianType.LittleSwap);
    }

    /// <summary>字节数组按指定字节序转为整数</summary>
    /// <param name="buffer"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    public static UInt32 ToUInt32(this Byte[] buffer, ByteOrder order) => ToUInt32(buffer, (EndianType)order);

    /// <summary>按字节序交换字节数组</summary>
    /// <param name="buf"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    public static Byte[] Swap(this Byte[] buf, ByteOrder order)
    {
        var rs = new Byte[buf.Length];
        switch (order)
        {
            case ByteOrder.ABCD:
            default:
                for (var i = 0; i < buf.Length; i++)
                {
                    rs[i] = buf[i];
                }
                break;
            case ByteOrder.DCBA:
                for (var i = 0; i < buf.Length; i++)
                {
                    rs[i] = buf[buf.Length - i - 1];
                }
                break;
            case ByteOrder.BADC:
                for (var i = 0; i < buf.Length - 1; i += 2)
                {
                    rs[i] = buf[i + 1];
                    rs[i + 1] = buf[i];
                }
                // 奇数个字节时，最后一个字节不变
                if (buf.Length % 2 == 1)
                    rs[buf.Length - 1] = buf[buf.Length - 1];
                break;
            case ByteOrder.CDAB:
                for (var i = 0; i < buf.Length - 1; i += 2)
                {
                    rs[i] = buf[buf.Length - i - 2];
                    rs[i + 1] = buf[buf.Length - i - 1];
                }
                // 奇数个字节时，最后一个字节不变
                if (buf.Length % 2 == 1)
                    rs[buf.Length - 1] = buf[0];
                break;
        }

        return rs;
    }

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
        try
        {
            // 找到物属性定义
            var pt = point.Name.IsNullOrEmpty() ? null : spec?.ExtendedProperties?.FirstOrDefault(e => e.Id.EqualIgnoreCase(point.Name));
            if (pt != null)
            {
                // 反向操作常量因子和缩放因子
                if (type.IsInt())
                {
                    var v = data.ToLong();
                    //if (pt.Constant != 0 || scaling != 1) v = (Int64)Math.Round(v * scaling + pt.Constant);
                    // 编码是反向操作，先减去常量，再除以缩放因子。为了避免精度问题，单精度范围先计算缩放因子倒数，再相乘
                    if (pt.Constant != 0 || pt.Scaling != 0) v = (Int64)Math.Round(((Double)v - pt.Constant) * (1 / pt.Scaling));

                    // 常见的2字节和4字节整型，直接转字节数组返回
                    var rs = type.GetTypeCode() switch
                    {
                        TypeCode.Byte or TypeCode.SByte => new[] { (Byte)v },
                        TypeCode.Int16 or TypeCode.UInt16 => ((UInt16)v).GetBytes(pt.Endian),
                        TypeCode.Int32 or TypeCode.UInt32 => ((UInt32)v).GetBytes(pt.Endian),
                        _ => throw new NotImplementedException(),
                    };
                    span?.AppendTag($"result={(rs is Byte[] bts ? bts.ToHex() : rs)} v={v} type={type.Name} scaling={pt.Scaling}");

                    return rs;
                }
                else if (type == typeof(Boolean))
                {
                    var rs = data.ToBoolean();
                    span?.AppendTag($"result={rs}");

                    return rs;
                }
                else if (type == typeof(Single))
                {
                    var v = (Single)data.ToDouble();
                    if (pt.Constant > 0) v -= pt.Constant;
                    if (pt.Scaling != 0) v /= pt.Scaling;

                    span?.AppendTag($"result={v} type={type.Name} scaling={pt.Scaling} constant={pt.Constant}");

                    // 按照小端读取出来，如果不是小端，则需要交换字节序
                    return BitConverter.GetBytes(v).Swap((ByteOrder)pt.Endian);
                }
                else if (type == typeof(Double))
                {
                    var v = data.ToDouble();
                    if (pt.Constant > 0) v -= pt.Constant;
                    if (pt.Scaling != 0) v /= pt.Scaling;

                    span?.AppendTag($"result={v} type={type.Name} scaling={pt.Scaling} constant={pt.Constant}");

                    // 按照小端读取出来，如果不是小端，则需要交换字节序
                    return BitConverter.GetBytes(v).Swap((ByteOrder)pt.Endian);
                }
            }

            return point.GetBytes(data);
        }
        catch (Exception ex)
        {
            span?.SetError(ex, null);
            throw;
            //return null;
        }
    }

    /// <summary>借助物模型解析数值</summary>
    /// <param name="spec">物模型</param>
    /// <param name="data">数据</param>
    /// <param name="point">点位信息</param>
    /// <returns></returns>
    public static Object? DecodeByThingModel(this ThingSpec spec, Byte[] data, IPoint point)
    {
        var type = point.GetNetType();
        if (type == null) return data;
        if (type == typeof(Byte[]) || !type.IsNumber()) return data;

        using var span = DefaultTracer.Instance?.NewSpan(nameof(DecodeByThingModel), $"name={point.Name} data={data.ToHex()} type={type.Name} rawType={point.Type}");
        try
        {
            // 找到物属性定义
            var pt = point.Name.IsNullOrEmpty() ? null : spec?.ExtendedProperties?.FirstOrDefault(e => e.Id.EqualIgnoreCase(point.Name));
            if (pt != null)
            {
                if (type == typeof(Boolean)) return data[0] > 0;
                if (type == typeof(Byte)) return data[0];

                // 操作常量因子和缩放因子
                if (type.IsInt())
                {
                    // 常见的2字节和4字节整型，直接转
                    var rs = type.GetTypeCode() switch
                    {
                        TypeCode.Int16 or TypeCode.UInt16 => data.ToUInt16(pt.Endian),
                        TypeCode.Int32 or TypeCode.UInt32 => data.ToUInt32(pt.Endian),
                        _ => throw new NotImplementedException(),
                    };
                    if (pt.Constant != 0 || pt.Scaling != 0) rs = (UInt32)Math.Round(rs * pt.Scaling + pt.Constant);
                    span?.AppendTag($"result={rs} type={type.Name}");

                    return rs.ChangeType(type);
                }
                else if (type == typeof(Single))
                {
                    var rs = BitConverter.ToSingle(data.Swap((ByteOrder)pt.Endian), 0);
                    if (pt.Constant != 0 || pt.Scaling != 0) rs = (Single)Math.Round(rs * pt.Scaling + pt.Constant);
                    return rs;
                }
                else if (type == typeof(Double))
                {
                    var rs = BitConverter.ToDouble(data.Swap((ByteOrder)pt.Endian), 0);
                    if (pt.Constant != 0 || pt.Scaling != 0) rs = Math.Round(rs * pt.Scaling + pt.Constant);
                    return rs;
                }
            }

            return point.Convert(data);
        }
        catch (Exception ex)
        {
            span?.SetError(ex, null);
            throw;
            //return null;
        }
    }
}
