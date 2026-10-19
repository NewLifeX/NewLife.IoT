using System.ComponentModel;

namespace NewLife.IoT;

/// <summary>本地化描述。构造参数为英文源文键，读取时按当前语言翻译</summary>
/// <remarks>
/// 继承自 <see cref="DescriptionAttribute"/>，现有 <c>GetDescription</c> 反射与 Xml 注释生成逻辑可直接识别。
/// 翻译在属性读取时发生，不在特性构造时缓存，因此支持进程内切换语言。
/// </remarks>
[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
public class LocalizedDescriptionAttribute : DescriptionAttribute
{
    /// <summary>使用英文源文键实例化</summary>
    /// <param name="key">英文源文键</param>
    public LocalizedDescriptionAttribute(String key) : base(key) { }

    /// <summary>描述。按当前语言翻译</summary>
    public override String Description => Localization.Translate(base.Description);
}
