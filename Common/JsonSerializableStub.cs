#if !NET6_0_OR_GREATER
using System;

// System.Text.Json 的 JsonSerializableAttribute 随 .NET6 提供的源生成器一同引入，低版本目标无法引用。
// 这里提供最小垫片，让模型类型可以无条件打上 [JsonSerializable] 标记；
// 该标记仅被 NewLife.Core 源生成器识别（且只在 NET5_0_OR_GREATER 下产出代码），低版本下不产生任何行为。
namespace System.Text.Json.Serialization;

/// <summary>Json 序列化类型标记。低版本目标的占位实现</summary>
/// <remarks>仅用于承载 NewLife.Core 源生成器的成员访问器生成标记</remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = true)]
internal sealed class JsonSerializableAttribute : Attribute
{
    /// <summary>类型</summary>
    public Type Type { get; }

    /// <summary>实例化</summary>
    /// <param name="type">要序列化的类型</param>
    public JsonSerializableAttribute(Type type) => Type = type;
}

#endif
