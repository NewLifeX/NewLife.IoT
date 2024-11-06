using System.Reflection;
using System.Runtime.Serialization;
#if NETCOREAPP
using System.Text.Json.Serialization;
#endif

namespace NewLife.IoT.ThingSpecification;

/// <summary>服务规范</summary>
/// <remarks>
/// 指设备可供外部调用的指令或方法。服务调用中可设置输入和输出参数。输入参数是服务执行时的参数，输出参数是服务执行后的结果。
/// 相比于属性，服务可通过一条指令实现更复杂的业务逻辑，例如执行某项特定的任务。
/// 服务分为异步和同步两种调用方式。
/// </remarks>
public class ServiceSpec : SpecBase
{
    #region 属性
    /// <summary>
    /// 调用类型。sync/async
    /// </summary>
#if NETCOREAPP
    [JsonPropertyName("callType")]
#endif
    [DataMember(Name = "callType")]
    public String? Type { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
#if NETCOREAPP
    [JsonPropertyName("desc")]
#endif
    [DataMember(Name = "desc")]
    public String? Description { get; set; }

    /// <summary>
    /// 方法
    /// </summary>
    public String? Method { get; set; }

    /// <summary>
    /// 输入
    /// </summary>
    public PropertySpec[]? InputData { get; set; }

    /// <summary>
    /// 输出
    /// </summary>
    public PropertySpec[]? OutputData { get; set; }
    #endregion

    #region 方法
    /// <summary>快速创建服务</summary>
    /// <param name="delegate"></param>
    /// <returns></returns>
    public static ServiceSpec Create(Delegate @delegate) => Create(@delegate.Method);

    /// <summary>快速创建服务</summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static ServiceSpec Create(MethodBase method)
    {
        //if (method == null) return null;

        var ss = new ServiceSpec
        {
            Id = method.Name,
            Name = method.GetDisplayName() ?? method.GetDescription(),
        };

        var pis = method.GetParameters();
        if (pis.Length > 0)
        {
            var ps = new List<PropertySpec>();
            foreach (var pi in pis)
            {
                ps.Add(Create(pi));
            }

            ss.InputData = ps.Where(e => e != null).ToArray();
        }

        return ss;
    }

    /// <summary>快速创建属性</summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public static PropertySpec Create(ParameterInfo member)
    {
        //if (member == null) return null;

        var ps = new PropertySpec
        {
            Id = member.Name!,
            DataType = new TypeSpec { Type = member.ParameterType.Name }
        };

        return ps;
    }
    #endregion
}