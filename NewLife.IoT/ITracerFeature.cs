#if !NETSTANDARD2_1_OR_GREATER
namespace NewLife.Log;

/// <summary>日志功能接口</summary>
public interface ITracerFeature
{
    /// <summary>性能追踪</summary>
    ITracer Tracer { get; set; }
}
#endif