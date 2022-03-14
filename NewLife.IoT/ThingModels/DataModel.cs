namespace NewLife.IoT.ThingModels;

/// <summary>数据模型</summary>
public class DataModel
{
    /// <summary>时间。数据采集时间，UTC毫秒</summary>
    public Int64 Time { get; set; }

    /// <summary>主题</summary>
    public String Topic { get; set; }

    /// <summary>数据</summary>
    public String Data { get; set; }
}