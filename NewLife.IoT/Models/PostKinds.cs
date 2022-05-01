using System.ComponentModel;

namespace NewLife.IoT.Models;

/// <summary>
/// 数据上报类型
/// </summary>
public enum PostKinds
{
    /// <summary>
    /// 变更上报
    /// </summary>
    [Description("变更上报")]
    Changed = 0,

    /// <summary>
    /// 总是上报
    /// </summary>
    [Description("总是上报")]
    Always = 1,

    /// <summary>
    /// 绝不上报
    /// </summary>
    [Description("绝不上报")]
    Never = 2,
}