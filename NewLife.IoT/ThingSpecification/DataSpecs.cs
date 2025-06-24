using NewLife.IoT.ThingModels;
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

    /// <summary>字节序</summary>
    public ByteOrder Order { get; set; }

    /// <summary>缩放因子。不能是0，默认1，n*scaling+constant</summary>
    public Single Scaling { get; set; } = 1;

    /// <summary>常量因子。默认0，n*scaling+constant</summary>
    public Single Constant { get; set; }

    /// <summary>采集点位置信息。常规地址6，Modbus地址 4x0023，位域地址D12.05，虚拟点位地址#</summary>
    public String? Address { get; set; }

    /// <summary>读取规则。数据解析规则，表达式或脚本</summary>
    public String? ReadRule { get; set; }

    /// <summary>写入规则。数据反解析规则，表达式或脚本</summary>
    public String? WriteRule { get; set; }

    /// <summary>事件模式。在客户端或服务端生成属性变更事件</summary>
    public EventModes EventMode { get; set; }
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
                if (Order != 0)
                    ds[nameof(Order)] = Order;
                if (Scaling != 1)
                    ds[nameof(Scaling)] = Scaling;
                if (Constant != 0)
                    ds[nameof(Constant)] = Constant;
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
                if (Order != 0)
                    ds[nameof(Order)] = Order;
                if (Scaling != 1)
                    ds[nameof(Scaling)] = Scaling;
                if (Constant != 0)
                    ds[nameof(Constant)] = Constant;
                break;
            case TypeCode.String:
                ds[nameof(Length)] = Length;
                break;
            default:
                return this.ToDictionary();
        }

        if (!Address.IsNullOrEmpty())
            ds[nameof(Address)] = Address;
        if (!ReadRule.IsNullOrEmpty())
            ds[nameof(ReadRule)] = ReadRule;
        if (!WriteRule.IsNullOrEmpty())
            ds[nameof(WriteRule)] = WriteRule;
        if (EventMode != EventModes.None)
            ds[nameof(EventMode)] = EventMode;

        return ds;
    }
    #endregion
}