namespace NewLife.IoT.Models;

/// <summary>设备模型。完整版，主要用于服务端下发或者接收</summary>
public class DeviceModel : IDeviceInfo
{
    /// <summary>编码</summary>
    public String Code { get; set; } = null!;

    /// <summary>名称</summary>
    public String? Name { get; set; }

    /// <summary>产品编码</summary>
    public String? ProductCode { get; set; }

    /// <summary>上报间隔。属性上报间隔，采集后暂存数据，定时上报，默认60秒，-1表示不上报属性</summary>
    public Int32 PostPeriod { get; set; }

    /// <summary>上报模式。数据上报模式，默认0表示变更上报，1每次上报，2不上报</summary>
    public PostKinds PostKind { get; set; }

    /// <summary>采集间隔。默认1000ms</summary>
    public Int32 PollingTime { get; set; }

    /// <summary>协议。选择使用哪一种驱动协议</summary>
    public String? Protocol { get; set; }

    /// <summary>设备参数。Xml/Json格式配置，根据协议驱动来解析</summary>
    public String? Parameter { get; set; }

    /// <summary>是否启用</summary>
    public Boolean Enable { get; set; }

    /// <summary>更新时间。用于判断数据变化</summary>
    public DateTime UpdateTime { get; set; }
}