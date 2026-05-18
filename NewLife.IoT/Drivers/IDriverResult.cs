namespace NewLife.IoT.Drivers;

/// <summary>驱动操作结果公共接口</summary>
/// <remarks>ReadResult、WriteResult、ControlResult 均实现此接口，便于上层代码统一处理操作状态</remarks>
public interface IDriverResult
{
    /// <summary>操作是否成功</summary>
    Boolean IsSuccess { get; }

    /// <summary>错误码。仅 IsSuccess=false 时有意义</summary>
    IoTErrorCode Code { get; }

    /// <summary>错误消息。仅 IsSuccess=false 时有意义</summary>
    String? Message { get; }
}
