#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NewLife.IoT.Drivers;
using NewLife.IoT.Models;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;

namespace NewLife.IoT;

/// <summary>Json 模型元数据保留。NativeAOT 裁剪下，反射式 JSON 需要保留公开属性与无参构造</summary>
/// <remarks>NewLife.Core 源生成器负责成员访问器表，本类型负责向裁剪器声明成员元数据保留。</remarks>
internal static class JsonPreserve
{
    private const DynamicallyAccessedMemberTypes _members =
        DynamicallyAccessedMemberTypes.PublicProperties |
        DynamicallyAccessedMemberTypes.PublicFields |
        DynamicallyAccessedMemberTypes.PublicParameterlessConstructor;

#pragma warning disable CA2255 // 类库使用模块初始化器声明裁剪保留，属高级场景
    [ModuleInitializer]
    [DynamicDependency(_members, typeof(ThingSpec))]
    [DynamicDependency(_members, typeof(Profile))]
    [DynamicDependency(_members, typeof(SpecBase))]
    [DynamicDependency(_members, typeof(PropertySpec))]
    [DynamicDependency(_members, typeof(EventSpec))]
    [DynamicDependency(_members, typeof(ServiceSpec))]
    [DynamicDependency(_members, typeof(TypeSpec))]
    [DynamicDependency(_members, typeof(DataSpecs))]
    [DynamicDependency(_members, typeof(PointModel))]
    [DynamicDependency(_members, typeof(PropertyModel))]
    [DynamicDependency(_members, typeof(DevicePropertyModel))]
    [DynamicDependency(_members, typeof(EventModel))]
    [DynamicDependency(_members, typeof(DataModel))]
    [DynamicDependency(_members, typeof(ServiceModel))]
    [DynamicDependency(_members, typeof(ServiceReplyModel))]
    [DynamicDependency(_members, typeof(ShadowModel))]
    [DynamicDependency(_members, typeof(PropertyModels))]
    [DynamicDependency(_members, typeof(EventModels))]
    [DynamicDependency(_members, typeof(DataModels))]
    [DynamicDependency(_members, typeof(DeviceInfo))]
    [DynamicDependency(_members, typeof(DeviceModel))]
    [DynamicDependency(_members, typeof(ServiceRequest))]
    [DynamicDependency(_members, typeof(WriteRequest))]
    [DynamicDependency(_members, typeof(ControlRequest))]
    [DynamicDependency(_members, typeof(DriverInfo))]
    [DynamicDependency(_members, typeof(DriverParameter))]
    [DynamicDependency(_members, typeof(ReadResult))]
    [DynamicDependency(_members, typeof(WriteResult))]
    [DynamicDependency(_members, typeof(ControlResult))]
    [DynamicDependency(_members, typeof(DriverDataEventArgs))]
    internal static void Initialize() { }
#pragma warning restore CA2255
}
#endif
