using NewLife.IoT.ThingModels;
using NewLife.Reflection;

namespace NewLife.IoT;

/// <summary>
/// 类型助手。处理IoT数据中的各种类型
/// </summary>
public static class TypeHelper
{
    private static readonly IDictionary<String, Int32> _lengths = new Dictionary<String, Int32>(StringComparer.OrdinalIgnoreCase)
    {
        ["bit"] = 1, ["bool"] = 1, ["boolean"] = 1, ["char"] = 1, ["byte"] = 1, ["sbyte"] = 1,
        ["short"] = 2, ["ushort"] = 2, ["int16"] = 2, ["uint16"] = 2, ["number"] = 2,
        ["int"] = 4, ["uint"] = 4, ["int32"] = 4, ["uint32"] = 4, ["float"] = 4, ["single"] = 4,
        ["long"] = 8, ["ulong"] = 8, ["int64"] = 8, ["uint64"] = 8, ["double"] = 8, ["decimal"] = 8,
    };

    private static readonly IDictionary<String, Type> _netTypes = new Dictionary<String, Type>(StringComparer.OrdinalIgnoreCase)
    {
        ["bit"] = typeof(Boolean), ["bool"] = typeof(Boolean), ["boolean"] = typeof(Boolean),
        ["char"] = typeof(Char),
        ["byte"] = typeof(Byte), ["sbyte"] = typeof(Byte),
        ["short"] = typeof(Int16), ["int16"] = typeof(Int16), ["number"] = typeof(Int16),
        ["ushort"] = typeof(UInt16), ["uint16"] = typeof(UInt16),
        ["int"] = typeof(Int32), ["int32"] = typeof(Int32),
        ["uint"] = typeof(UInt32), ["uint32"] = typeof(UInt32),
        ["float"] = typeof(Single), ["single"] = typeof(Single),
        ["long"] = typeof(Int64), ["int64"] = typeof(Int64),
        ["ulong"] = typeof(UInt64), ["uint64"] = typeof(UInt64),
        ["double"] = typeof(Double),
        ["decimal"] = typeof(Decimal),
        ["string"] = typeof(String), ["text"] = typeof(String),
        ["date"] = typeof(DateTime), ["time"] = typeof(DateTime), ["datetime"] = typeof(DateTime),
        ["byte[]"] = typeof(Byte[]), ["hex"] = typeof(Byte[]),
    };

    /// <summary>
    /// 获取指定类型的数据长度
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Int32 GetLength(String? type)
    {
        if (type.IsNullOrEmpty()) return 0;

        return _lengths.TryGetValue(type, out var len) ? len : 0;
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
    /// 获取指定IoT类型的本地类型。可用于格式化各种非标类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type? GetNetType(String? type)
    {
        if (type.IsNullOrEmpty()) return null;

        return _netTypes.TryGetValue(type, out var netType) ? netType : null;
    }

    /// <summary>
    /// 获取点位数据长度，若未设置则根据类型自动计算
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Int32 GetLength(this IPoint point) => point.Length > 0 ? point.Length : GetLength(point.Type);



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
                _ => type?.Name.ToLowerInvariant(),
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

    /// <summary>
    /// 获取所有可用IoT类型。显示名按 <see cref="Localization.Culture"/> 翻译，不跨语言缓存
    /// </summary>
    /// <param name="full">是否返回完成类型，默认false返回精简类型</param>
    /// <returns></returns>
    public static IDictionary<String, String> GetIoTTypes(Boolean full = false)
    {
        if (full)
        {
            return new Dictionary<String, String>
            {
                ["short"] = Localization.Translate("Short integer"),
                ["int"] = Localization.Translate("Integer"),
                ["float"] = Localization.Translate("Decimal number"),
                ["bool"] = Localization.Translate("Boolean"),
                ["byte"] = Localization.Translate("Byte"),
                ["long"] = Localization.Translate("Long integer"),
                ["double"] = Localization.Translate("Double precision"),
                ["text"] = Localization.Translate("Text"),
                ["time"] = Localization.Translate("Time"),
            };
        }
        else
        {
            return new Dictionary<String, String>
            {
                ["int"] = Localization.Translate("Integer"),
                ["float"] = Localization.Translate("Decimal number"),
                ["bool"] = Localization.Translate("Boolean"),
                ["text"] = Localization.Translate("Text"),
            };
        }
    }
}