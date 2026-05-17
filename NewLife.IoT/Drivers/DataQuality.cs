namespace NewLife.IoT.Drivers;

/// <summary>数据质量码。描述采集数据的可信度</summary>
/// <remarks>
/// 参考 OPC UA 质量码体系，分三级：Good（可用）、Uncertain（存疑）、Bad（无效）。
/// Bad 子码进一步区分离线、超时等常见运行时失败，驱动应在失败时尽量填充具体子码。
/// </remarks>
public enum DataQuality : Byte
{
    /// <summary>良好。数据有效可信，可直接使用</summary>
    Good = 0,

    /// <summary>不确定。数据可能有效，但来源存疑，例如断线恢复后的最后已知值</summary>
    Uncertain = 1,

    /// <summary>不良。采集失败，数据无效</summary>
    Bad = 2,

    /// <summary>不良：未连接。设备离线或连接尚未建立</summary>
    BadNotConnected = 3,

    /// <summary>不良：超时。通信超时导致采集失败</summary>
    BadTimeout = 4,

    /// <summary>不良：校验错误。响应数据校验失败，可能受到干扰</summary>
    BadChecksumError = 5,

    /// <summary>不良：协议错误。响应数据格式不符合协议规范</summary>
    BadProtocolError = 6,
}
