using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewLife.IoT.Clients;
using NewLife.IoT.ThingModels;
using Xunit;

namespace XUnitTest;

public class ServiceHandlerTests
{
    [Fact]
    public void RegisterService_FuncStringString_NoServiceName_UsesMethodName()
    {
        var handler = new TestServiceHandler();
        var method = new Func<String, String>(s => "reply:" + s);

        handler.RegisterService(null, method);

        Assert.True(handler.Services.Count > 0);
    }

    [Fact]
    public void RegisterService_FuncStringString_WithServiceName()
    {
        var handler = new TestServiceHandler();

        handler.RegisterService("Echo", new Func<String, String>(s => s));

        Assert.True(handler.Services.ContainsKey("Echo"));
    }

    [Fact]
    public void RegisterService_FuncStringTaskString()
    {
        var handler = new TestServiceHandler();

        handler.RegisterService("AsyncEcho", new Func<String, Task<String>>(s => Task.FromResult(s)));

        Assert.True(handler.Services.ContainsKey("AsyncEcho"));
    }

    [Fact]
    public void RegisterService_FuncServiceModelServiceReplyModel()
    {
        var handler = new TestServiceHandler();

        handler.RegisterService("SvcModel", new Func<ServiceModel, ServiceReplyModel>(m => new ServiceReplyModel { Id = m.Id }));

        Assert.True(handler.Services.ContainsKey("SvcModel"));
    }

    private class TestServiceHandler : IServiceHandler
    {
        public IDictionary<String, Delegate> Services { get; } = new Dictionary<String, Delegate>();
    }
}
