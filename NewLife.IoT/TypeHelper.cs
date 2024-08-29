using NewLife.IoT.ThingModels;
using NewLife.Reflection;

namespace NewLife.IoT;

/// <summary>
/// 类型助手。处理IoT数据中的各种类型
/// </summary>
public static class TypeHelper
{
    /// <summary>
    /// 获取指定类型的数据长度
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Int32 GetLength(String? type)
    {
        if (type.IsNullOrEmpty()) return 0;

        return type.ToLower() switch
        {
            "bit" or "bool" or "boolean" or "char" or "byte" or "sbyte" => 1,
            "short" or "ushort" or "int16" or "uint16" or "number" => 2,
            "int" or "uint" or "int32" or "uint32" or "float" or "single" => 4,
            "long" or "ulong" or "int64" or "uint64" or "double" or "decimal" => 8,
            _ => 0,
        };
    }

    /// <summary>
    /// 获取指定类型的数据长度
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Int32 GetLength(Type? type)
    {
        if (type == null) return 0;

        return type.GetTypeCode() switch
        {
            TypeCode.Boolean => 1,
            TypeCode.Char or TypeCode.Byte or TypeCode.SByte => 1,
            TypeCode.Int16 or TypeCode.UInt16 => 2,
            TypeCode.Int32 or TypeCode.UInt32 => 4,
            TypeCode.Int64 or TypeCode.UInt64 => 8,
            TypeCode.Single => 4,
            TypeCode.Double or TypeCode.Decimal => 8,
            TypeCode.String => 0,
            TypeCode.DateTime => 4,
            _ => 0,
        };
    }

    /// <summary>
    /// 获取点位数据长度，若未设置则根据类型自动计算
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Int32 GetLength(this IPoint point) => point.Length > 0 ? point.Length : GetLength(point.Type);

    /// <summary>
    /// 获取指定IoT类型的本地类型。可用于格式化各种非标类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static Type? GetNetType(String? type)
    {
        if (type.IsNullOrEmpty()) return null;

        return type.ToLower() switch
        {
            "bit" or "bool" or "boolean" => typeof(Boolean),
            "char" => typeof(Char),
            "byte" or "sbyte" => typeof(Byte),
            "short" or "int16" or "number" => typeof(Int16),
            "ushort" or "uint16" => typeof(UInt16),
            "int" or "int32" => typeof(Int32),
            "uint" or "uint32" => typeof(UInt32),
            "float" or "single" => typeof(Single),
            "long" or "int64" => typeof(Int64),
            "ulong" or "uint64" => typeof(UInt64),
            "double" => typeof(Double),
            "decimal" => typeof(Decimal),
            "string" or "text" => typeof(String),
            "date" or "time" or "datetime" => typeof(DateTime),
            "byte[]" or "hex" => typeof(Byte[]),
            _ => null,
        };
    }

    /// <summary>
    /// 获取指定点位的本地类型，依赖于点位IoT类型和长度。可用于格式化各种非标类型
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Type? GetNetType(this IPoint point)
    {
        if ((point?.Type).IsNullOrEmpty()) return null;

        var type = GetNetType(point.Type);
        if (type == null) return null;

        if (point.Length > 0)
        {
            // 如果长度一致，直接返回
            if (point.Length == GetLength(type)) return type;

            // 数字类型，最终类型取决于长度。有的场景习惯用2字节int
            if (type.IsInt())
            {
                return point.Length switch
                {
                    1 => typeof(Byte),
                    2 => typeof(Int16),
                    3 or 4 => typeof(Int32),
                    _ => type,
                };
            }
            // 小数类型，最终类型取决于长度。有的场景习惯用2字节float或4字节double
            else if (type == typeof(Single) || type == typeof(Double) || type == typeof(Decimal))
            {
                return point.Length <= 4 ? typeof(Single) : typeof(Double);
            }
        }

        return type;
    }

    /// <summary>
    /// 设置点位的IoT类型和长度
    /// </summary>
    /// <param name="point"></param>
    /// <param name="type"></param>
    public static void SetNetType(this IPoint point, Type type)
    {
        point.Type = GetIoTType(type);
        point.Length = GetLength(type);
    }

    /// <summary>
    /// 获取指定类型的IoT类型，简化可用类型。可用于格式化各种非标类型
    /// </summary>
    /// <param name="type"></param>
    /// <param name="full">是否返回完成类型，默认false返回精简类型</param>
    /// <returns></returns>
    public static String? GetIoTType(Type? type, Boolean full = false)
    {
        if (type == null) return null;

        if (full)
        {
            if (type == typeof(Byte[])) return "hex";

            return type.GetTypeCode() switch
            {
                TypeCode.Boolean => "bool",
                TypeCode.Char or TypeCode.Byte or TypeCode.SByte => "byte",
                TypeCode.Int16 or TypeCode.UInt16 => "short",
                TypeCode.Int32 or TypeCode.UInt32 => "int",
                TypeCode.Int64 or TypeCode.UInt64 => "long",
                TypeCode.Single => "float",
                TypeCode.Double or TypeCode.Decimal => "double",
                TypeCode.String => "text",
                TypeCode.DateTime => "time",
                _ => type?.Name.ToLower(),
            };
        }
        else
        {
            return type.GetTypeCode() switch
            {
                TypeCode.Boolean => "bool",
                TypeCode.Char or TypeCode.Byte or TypeCode.SByte => "int",
                TypeCode.Int16 or TypeCode.UInt16 => "int",
                TypeCode.Int32 or TypeCode.UInt32 => "int",
                TypeCode.Int64 or TypeCode.UInt64 => "int",
                TypeCode.Single => "float",
                TypeCode.Double or TypeCode.Decimal => "float",
                TypeCode.String => "text",
                //TypeCode.DateTime => "time",
                _ => null,
            };
        }
    }

    /// <summary>
    /// 获取指定点位的标准IoT类型，依据原类型及长度
    /// </summary>
    /// <param name="point"></param>
    /// <param name="full">是否返回完成类型，默认false返回精简类型</param>
    /// <returns></returns>
    public static String? GetIoTType(this IPoint point, Boolean full = false)
    {
        var type = point.GetNetType();
        return GetIoTType(type, full);
    }

    private static IDictionary<String, String>? _fullTypes;
    private static IDictionary<String, String>? _iotTypes;
    /// <summary>
    /// 获取所有可用IoT类型
    /// </summary>
    /// <param name="full">是否返回完成类型，默认false返回精简类型</param>
    /// <returns></returns>
    public static IDictionary<String, String> GetIoTTypes(Boolean full = false)
    {
        if (full)
        {
            if (_fullTypes != null) return _fullTypes;

            var dic = new Dictionary<String, String>
            {
                ["short"] = "短整数",
                ["int"] = "整数",
                ["float"] = "小数",
                ["bool"] = "布尔型",
                ["byte"] = "字节",
                ["long"] = "长整数",
                ["double"] = "双精度",
                ["text"] = "文本",
                ["time"] = "时间",
            };

            return _fullTypes = dic;
        }
        else
        {
            if (_iotTypes != null) return _iotTypes;

            var dic = new Dictionary<String, String>
            {
                ["int"] = "整数",
                ["float"] = "小数",
                ["bool"] = "布尔型",
                ["text"] = "文本",
            };

            return _iotTypes = dic;
        }
    }
}