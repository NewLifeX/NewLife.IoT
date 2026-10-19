namespace NewLife.IoT.ThingModels;

/// <summary>服务状态</summary>
public enum ServiceStatus
{
    /// <summary>就绪</summary>
    [LocalizedDescription("Ready")]
    就绪 = 0,

    /// <summary>处理中</summary>
    [LocalizedDescription("Processing")]
    处理中 = 1,

    /// <summary>已完成</summary>
    [LocalizedDescription("Completed")]
    已完成 = 2,

    /// <summary>取消</summary>
    [LocalizedDescription("Cancelled")]
    取消 = 3,

    /// <summary>错误</summary>
    [LocalizedDescription("Error")]
    错误 = 4,
}