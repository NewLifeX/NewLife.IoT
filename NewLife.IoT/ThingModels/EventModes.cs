namespace NewLife.IoT.ThingModels;

/// <summary>事件模式。在客户端或服务端生成属性变更事件</summary>
public enum EventModes
{
    /// <summary>不触发</summary>
    None = 0,

    /// <summary>客户端触发</summary>
    Client = 1,

    /// <summary>服务端触发</summary>
    Server = 2,
}