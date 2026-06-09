using System.Threading;

namespace NewLife.IoT.Controllers;

/// <summary>继电器控制器</summary>
public interface IRelayController
{
    /// <summary>Modbus对象</summary>
    IModbus Modbus { get; set; }

    /// <summary>主机地址</summary>
    Byte Host { get; set; }

    /// <summary>异步控制指定点位</summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken">取消令牌</param>
    Task WriteAsync(Int32 index, Boolean value, CancellationToken cancellationToken = default);

    /// <summary>异步翻转指定点位</summary>
    /// <param name="index"></param>
    /// <param name="cancellationToken">取消令牌</param>
    Task InvertAsync(Int32 index, CancellationToken cancellationToken = default);

    /// <summary>异步控制所有点位</summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken">取消令牌</param>
    Task WriteAllAsync(Boolean value, CancellationToken cancellationToken = default);

    /// <summary>异步翻转所有点位</summary>
    /// <param name="cancellationToken">取消令牌</param>
    Task InvertAllAsync(CancellationToken cancellationToken = default);

    /// <summary>异步读取指定点位</summary>
    /// <param name="index"></param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<Boolean> ReadAsync(Int32 index, CancellationToken cancellationToken = default);

    /// <summary>异步读取所有点位</summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<Boolean[]> ReadAllAsync(CancellationToken cancellationToken = default);
}

/// <summary>继电器控制板</summary>
public class RelayController : IRelayController
{
    #region 属性
    /// <summary>Modbus对象</summary>
    public IModbus Modbus { get; set; } = null!;

    /// <summary>主机地址</summary>
    public Byte Host { get; set; } = 1;

    /// <summary>点位起始地址</summary>
    public UInt16 StartAddress { get; set; } = 0x0000;

    /// <summary>点位数量</summary>
    public Int32 Count { get; set; } = 8;
    #endregion

    /// <summary>控制指定点位</summary>
    /// <param name="index">索引。相对于起始地址的偏移量</param>
    /// <param name="value">输出值，true为0xFF00，false为0x0000</param>
    /// <param name="cancellationToken">取消令牌</param>
    public virtual async Task WriteAsync(Int32 index, Boolean value, CancellationToken cancellationToken = default) => await Modbus.WriteCoilAsync(Host, (UInt16)(StartAddress + index), (UInt16)(value ? 0xFF00 : 0x0000), cancellationToken).ConfigureAwait(false);

    /// <summary>翻转指定点位</summary>
    /// <param name="index"></param>
    /// <param name="cancellationToken">取消令牌</param>
    public virtual async Task InvertAsync(Int32 index, CancellationToken cancellationToken = default) => await Modbus.WriteCoilAsync(Host, (UInt16)(StartAddress + index), 0x5500, cancellationToken).ConfigureAwait(false);

    /// <summary>控制所有点位</summary>
    /// <param name="value">输出值，true为0xFFFF，false为0x0000</param>
    /// <param name="cancellationToken">取消令牌</param>
    public virtual async Task WriteAllAsync(Boolean value, CancellationToken cancellationToken = default) => await Modbus.WriteCoilAsync(Host, 0x00FF, (UInt16)(value ? 0xFFFF : 0x0000), cancellationToken).ConfigureAwait(false);

    /// <summary>翻转所有点位</summary>
    /// <param name="cancellationToken">取消令牌</param>
    public virtual async Task InvertAllAsync(CancellationToken cancellationToken = default) => await Modbus.WriteCoilAsync(Host, 0x00FF, 0x5a00, cancellationToken).ConfigureAwait(false);

    /// <summary>读取指定点位</summary>
    /// <param name="index"></param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public virtual async Task<Boolean> ReadAsync(Int32 index, CancellationToken cancellationToken = default)
    {
        var result = await Modbus.ReadCoilAsync(Host, (UInt16)(StartAddress + index), 1, cancellationToken).ConfigureAwait(false);
        return result[0];
    }

    /// <summary>读取所有点位</summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public virtual async Task<Boolean[]> ReadAllAsync(CancellationToken cancellationToken = default) => await Modbus.ReadCoilAsync(Host, StartAddress, (UInt16)Count, cancellationToken).ConfigureAwait(false);

    /// <summary>读取从机地址</summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public async Task<UInt16> ReadAddressAsync(CancellationToken cancellationToken = default)
    {
        var result = await Modbus.ReadRegisterAsync(0x00, 0, 1, cancellationToken).ConfigureAwait(false);
        return result[0];
    }

    ///// <summary>读取从机波特率</summary>
    ///// <returns></returns>
    //public UInt16 ReadBaudrate() => Modbus.ReadRegister(0xFF, 0x03E8, 1)[0];
}