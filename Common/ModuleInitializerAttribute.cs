using System;

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices;

/// <summary>模块初始化器特性补丁。.NET 5 之前没有该特性，NewLife.Core 内置的同类补丁为 internal 不可见</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
internal sealed class ModuleInitializerAttribute : Attribute { }
#endif
