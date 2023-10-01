namespace NewLife.IoT.ThingModels;

/// <summary>服务响应模型</summary>
public class ServiceReplyModel
{
    /// <summary>编号。用于服务调用请求与结果响应配对</summary>
    public Int64 Id { get; set; }

    /// <summary>状态</summary>
    public ServiceStatus Status { get; set; }

    /// <summary>返回数据</summary>
    public String? Data { get; set; }
}