using NewLife.IoT.ThingModels;
using NewLife.Reflection;

namespace NewLife.IoT
{
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
        public static Int32 GetLength(String type)
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
        /// 获取点位数据长度，若未设置则根据类型自动计算
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Int32 GetLength(this IPoint point) => point.Length > 0 ? point.Length : GetLength(point.Type);

        /// <summary>
        /// 获取指定类型的本地类型。可用于格式化各种非标类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Type GetNetType(String type)
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
                "double" or "decimal" => typeof(Double),
                "string" or "text" => typeof(String),
                "date" or "time" or "datetime" => typeof(DateTime),
                _ => null,
            };
        }

        /// <summary>
        /// 获取指定点位的本地类型，依赖于点位类型和长度。可用于格式化各种非标类型
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Type GetNetType(this IPoint point)
        {
            if ((point?.Type).IsNullOrEmpty()) return null;

            var type = GetNetType(point.Type);

            // 数字类型，最终类型取决于长度
            if (type.IsInt() && point.Length > 0)
            {
                return point.Length switch
                {
                    1 => typeof(Byte),
                    2 => typeof(UInt16),
                    3 or 4 => typeof(UInt32),
                    _ => type,
                };
            }
            // 小数类型，最终类型取决于长度
            else if (type == typeof(Single) || type == typeof(Double) || type == typeof(Decimal))
            {
                return point.Length switch
                {
                    1 or 2 => typeof(Single),
                    3 or 4 => typeof(Double),
                    _ => type,
                };
            }

            return type;
        }

        /// <summary>
        /// 获取指定类型的IoT类型，简化可用类型。可用于格式化各种非标类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static String GetIoTType(Type type)
        {
            if (type == null) return null;

            return type.GetTypeCode() switch
            {
                TypeCode.Boolean or TypeCode.Char or TypeCode.Byte or TypeCode.SByte => "bit",
                TypeCode.Int16 or TypeCode.UInt16 => "short",
                TypeCode.Int32 or TypeCode.UInt32 => "int",
                TypeCode.Int64 or TypeCode.UInt64 => "long",
                TypeCode.Single => "float",
                TypeCode.Double or TypeCode.Decimal => "double",
                TypeCode.String => "text",
                TypeCode.DateTime => "time",
                _ => null,
            };
        }
    }
}