namespace NewLife.IoT.ThingSpecification;

/// <summary>数据规范</summary>
public class DataSpecs
{
    /// <summary>最小值</summary>
    public Double Min { get; set; }

    /// <summary>最大值</summary>
    public Double Max { get; set; }

    /// <summary>单位</summary>
    public String Unit { get; set; }

    /// <summary>单位名称</summary>
    public String UnitName { get; set; }

    /// <summary>步进</summary>
    public Double Step { get; set; }

    /// <summary>长度</summary>
    public Int32 Length { get; set; }

    /// <summary>布尔类型值</summary>
    public Boolean BValue { get; set; }
}