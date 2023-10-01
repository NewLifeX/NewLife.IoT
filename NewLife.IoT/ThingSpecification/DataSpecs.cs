using NewLife.Reflection;

namespace NewLife.IoT.ThingSpecification;

/// <summary>数据规范</summary>
public class DataSpecs
{
    #region 属性
    /// <summary>最小值</summary>
    public Double Min { get; set; }

    /// <summary>最大值</summary>
    public Double Max { get; set; }

    /// <summary>单位</summary>
    public String? Unit { get; set; }

    /// <summary>单位名称</summary>
    public String? UnitName { get; set; }

    /// <summary>步进</summary>
    public Double Step { get; set; }

    /// <summary>长度</summary>
    public Int32 Length { get; set; }

    /// <summary>枚举映射。布尔型和数字型特有，例如“0=关,1=开”，又如“1=东,2=南,3=西,4=北”</summary>
    public IDictionary<String, String>? Mapping { get; set; }
    #endregion

    #region 方法
    /// <summary>根据指定数据类型获取成员字典，不同类型所需要的字段不一样</summary>
    /// <param name="type">数据类型</param>
    /// <returns></returns>
    public IDictionary<String, Object?> GetDictionary(String type)
    {
        var ds = new Dictionary<String, Object?>();

        var t = TypeHelper.GetNetType(type);
        if (t == null) return this.ToDictionary();

        switch (t.GetTypeCode())
        {
            case TypeCode.Boolean:
                ds[nameof(Mapping)] = Mapping;
                break;
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.Char:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                if (Length > 0)
                    ds[nameof(Length)] = Length;
                if (Min != 0 || Max != 0)
                    ds[nameof(Min)] = Min;
                if (Max != 0)
                    ds[nameof(Max)] = Max;
                if (!Unit.IsNullOrEmpty())
                    ds[nameof(Unit)] = Unit;
                if (!UnitName.IsNullOrEmpty())
                    ds[nameof(UnitName)] = UnitName;
                if (Step != 0)
                    ds[nameof(Step)] = Step;
                if (Mapping != null)
                    ds[nameof(Mapping)] = Mapping;
                break;
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
                if (Length > 0)
                    ds[nameof(Length)] = Length;
                if (Min != 0 || Max != 0)
                    ds[nameof(Min)] = Min;
                if (Max != 0)
                    ds[nameof(Max)] = Max;
                if (!Unit.IsNullOrEmpty())
                    ds[nameof(Unit)] = Unit;
                if (!UnitName.IsNullOrEmpty())
                    ds[nameof(UnitName)] = UnitName;
                if (Step != 0)
                    ds[nameof(Step)] = Step;
                break;
            case TypeCode.String:
                ds[nameof(Length)] = Length;
                break;
            default:
                return this.ToDictionary();
        }

        return ds;
    }
    #endregion
}