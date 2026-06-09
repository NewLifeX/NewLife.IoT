namespace NewLife.IoT.Controllers;

/// <summary>Modbus操作接口</summary>
public interface IModbus : IDisposable
{
    #region 读取
    /// <summary>异步读取线圈，0x01</summary>
    /// <param name="host">主机。一般是1</param>
    /// <param name="address">地址。例如0x0002</param>
    /// <param name="count">线圈数量。一般要求8的倍数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>线圈状态字节数组</returns>
    Task<Boolean[]> ReadCoilAsync(Byte host, UInt16 address, UInt16 count, CancellationToken cancellationToken = default);

    /// <summary>异步读离散量输入，0x02</summary>
    /// <param name="host">主机。一般是1</param>
    /// <param name="address">地址。例如0x0002</param>
    /// <param name="count">输入数量。一般要求8的倍数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>输入状态字节数组</returns>
    Task<Boolean[]> ReadDiscreteAsync(Byte host, UInt16 address, UInt16 count, CancellationToken cancellationToken = default);

    /// <summary>异步读取保持寄存器，0x03</summary>
    /// <param name="host">主机。一般是1</param>
    /// <param name="address">地址。例如0x0002</param>
    /// <param name="count">寄存器数量。每个寄存器2个字节</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>寄存器值数组</returns>
    Task<UInt16[]> ReadRegisterAsync(Byte host, UInt16 address, UInt16 count, CancellationToken cancellationToken = default);

    /// <summary>异步读取输入寄存器，0x04</summary>
    /// <param name="host">主机。一般是1</param>
    /// <param name="address">地址。例如0x0002</param>
    /// <param name="count">输入寄存器数量。每个寄存器2个字节</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>输入寄存器值数组</returns>
    Task<UInt16[]> ReadInputAsync(Byte host, UInt16 address, UInt16 count, CancellationToken cancellationToken = default);
    #endregion

    #region 写入
    /// <summary>异步写入单线圈，0x05</summary>
    /// <param name="host">主机。一般是1</param>
    /// <param name="address">地址。例如0x0002</param>
    /// <param name="value">输出值。一般是 0xFF00/0x0000</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>输出值</returns>
    Task<Int32> WriteCoilAsync(Byte host, UInt16 address, UInt16 value, CancellationToken cancellationToken = default);

    /// <summary>异步写入保持寄存器，0x06</summary>
    /// <param name="host">主机。一般是1</param>
    /// <param name="address">地址。例如0x0002</param>
    /// <param name="value">数值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>寄存器值</returns>
    Task<Int32> WriteRegisterAsync(Byte host, UInt16 address, UInt16 value, CancellationToken cancellationToken = default);

    /// <summary>异步写多个线圈，0x0F</summary>
    /// <param name="host">主机。一般是1</param>
    /// <param name="address">地址。例如0x0002</param>
    /// <param name="values">值。一般是 0xFF00/0x0000</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>数量</returns>
    Task<Int32> WriteCoilsAsync(Byte host, UInt16 address, UInt16[] values, CancellationToken cancellationToken = default);

    /// <summary>异步写多个保持寄存器，0x10</summary>
    /// <param name="host">主机。一般是1</param>
    /// <param name="address">地址。例如0x0002</param>
    /// <param name="values">数值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>寄存器数量</returns>
    Task<Int32> WriteRegistersAsync(Byte host, UInt16 address, UInt16[] values, CancellationToken cancellationToken = default);
    #endregion
}
