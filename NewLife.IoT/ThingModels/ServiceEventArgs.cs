namespace NewLife.IoT.ThingModels;

/// <summary>
/// 命令事件参数
/// </summary>
public class ServiceEventArgs : EventArgs
{
    /// <summary>
    /// 命令
    /// </summary>
    public ServiceModel? Model { get; set; }

    /// <summary>
    /// 响应
    /// </summary>
    public ServiceReplyModel? Reply { get; set; }
}