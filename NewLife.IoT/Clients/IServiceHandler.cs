using NewLife.IoT.ThingModels;
using NewLife.Log;
using NewLife.Remoting;
using NewLife.Serialization;

namespace NewLife.IoT.Clients;

/// <summary>命令服务处理器接口</summary>
public interface IServiceHandler
{
    ///// <summary>收到命令时触发</summary>
    //event EventHandler<ServiceEventArgs> Received;

    /// <summary>命令集合</summary>
    IDictionary<String, Delegate> Services { get; }
}

/// <summary>命令服务处理器助手</summary>
public static class ServiceHandlerHelper
{
    /// <summary>
    /// 注册服务。收到平台下发的服务调用时，执行注册的方法
    /// </summary>
    /// <param name="client">命令客户端</param>
    /// <param name="service"></param>
    /// <param name="method"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RegisterService(this IServiceHandler client, String service, Func<String, String> method)
    {
        if (service.IsNullOrEmpty()) service = method.Method.Name;

        if (client is IDevice device)
            device.RegisterService(service, method);
        else
            client.Services[service] = method;
    }

    /// <summary>
    /// 注册服务。收到平台下发的服务调用时，执行注册的方法
    /// </summary>
    /// <param name="client">命令客户端</param>
    /// <param name="service"></param>
    /// <param name="method"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RegisterService(this IServiceHandler client, String service, Func<String, Task<String>> method)
    {
        if (service.IsNullOrEmpty()) service = method.Method.Name;

        if (client is IDevice device)
            device.RegisterService(service, method);
        else
            client.Services[service] = method;
    }

    /// <summary>
    /// 注册服务。收到平台下发的服务调用时，执行注册的方法
    /// </summary>
    /// <param name="client">命令客户端</param>
    /// <param name="service"></param>
    /// <param name="method"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RegisterService(this IServiceHandler client, String service, Func<ServiceModel, ServiceReplyModel> method)
    {
        if (service.IsNullOrEmpty()) service = method.Method.Name;

        if (client is IDevice device)
            device.RegisterService(service, method);
        else
            client.Services[service] = method;
    }

    /// <summary>
    /// 注册服务。收到平台下发的服务调用时，执行注册的方法
    /// </summary>
    /// <param name="client">命令客户端</param>
    /// <param name="service"></param>
    /// <param name="method"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RegisterService(this IServiceHandler client, String service, Func<ServiceModel, Task<ServiceReplyModel>> method)
    {
        if (service.IsNullOrEmpty()) service = method.Method.Name;

        if (client is IDevice device)
            device.RegisterService(service, method);
        else
            client.Services[service] = method;
    }

    /// <summary>执行命令</summary>
    /// <param name="client">命令客户端</param>
    /// <param name="model"></param>
    /// <returns></returns>
    public static async Task<ServiceReplyModel> ExecuteService(this IServiceHandler client, ServiceModel model)
    {
        using var span = DefaultTracer.Instance?.NewSpan("ExecuteService", model);
        var rs = new ServiceReplyModel { Id = model.Id, Status = ServiceStatus.已完成 };
        try
        {
            var result = await OnService(client, model).ConfigureAwait(false);
            if (result is ServiceReplyModel reply)
            {
                reply.Id = model.Id;
                if (reply.Status is ServiceStatus.就绪 or ServiceStatus.处理中)
                    reply.Status = ServiceStatus.已完成;

                return reply;
            }

            rs.Data = result?.ToJson();
            return rs;
        }
        catch (Exception ex)
        {
            span?.SetError(ex, null);

            XTrace.WriteException(ex);

            rs.Data = ex.Message;
            if (ex is ApiException aex && aex.Code == 400)
                rs.Status = ServiceStatus.取消;
            else
                rs.Status = ServiceStatus.错误;
        }

        return rs;
    }

    /// <summary>分发执行服务</summary>
    /// <param name="client">命令客户端</param>
    /// <param name="model"></param>
    private static async Task<Object?> OnService(IServiceHandler client, ServiceModel model)
    {
        if (!client.Services.TryGetValue(model.Name, out var d))
        {
            // 通用方法
            if (!client.Services.TryGetValue("*", out d))
                throw new ApiException(400, $"找不到服务[{model.Name}]");
        }

        if (d is Func<String?, String?> func) return func(model.InputData);
        if (d is Func<ServiceModel, ServiceReplyModel> func2) return func2(model);

        if (d is Func<String?, Task<String?>> func3) return await func3(model.InputData).ConfigureAwait(false);
        if (d is Func<ServiceModel, Task<ServiceReplyModel>> func4) return await func4(model).ConfigureAwait(false);

        return null;
    }
}