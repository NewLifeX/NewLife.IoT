namespace NewLife.IoT.Models;

/// <summary>
/// 数据上报类型
/// </summary>
public enum PostKinds
{
    /// <summary>
    /// 变更上报
    /// </summary>
    [LocalizedDescription("Report on change")]
    Changed = 0,

    /// <summary>
    /// 总是上报
    /// </summary>
    [LocalizedDescription("Always report")]
    Always = 1,

    /// <summary>
    /// 绝不上报
    /// </summary>
    [LocalizedDescription("Never report")]
    Never = 2,
}