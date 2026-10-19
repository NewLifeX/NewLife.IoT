#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NewLife.IoTSocket.Drivers;

namespace NewLife.IoTSocket;

/// <summary>Json 模型元数据保留。NativeAOT 裁剪下，反射式 JSON 需要保留公开属性与无参构造</summary>
internal static class JsonPreserve
{
    private const DynamicallyAccessedMemberTypes _members =
        DynamicallyAccessedMemberTypes.PublicProperties |
        DynamicallyAccessedMemberTypes.PublicFields |
        DynamicallyAccessedMemberTypes.PublicParameterlessConstructor;

#pragma warning disable CA2255 // 类库使用模块初始化器声明裁剪保留，属高级场景
    [ModuleInitializer]
    [DynamicDependency(_members, typeof(SocketParameter))]
    [DynamicDependency(_members, typeof(HttpParameter))]
    internal static void Initialize() { }
#pragma warning restore CA2255
}
#endif
