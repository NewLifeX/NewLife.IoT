namespace NewLife.IoT.ThingModels;

/// <summary>
/// 服务响应模型
/// </summary>
public class ServiceReplyModel
{
    /// <summary>服务编号</summary>
    public Int64 Id { get; set; }

    /// <summary>状态</summary>
    public ServiceStatus Status { get; set; }

    /// <summary>返回数据</summary>
    public String Data { get; set; }
}