namespace NewLife.IoT.Controllers;

/// <summary>继电器控制器</summary>
public interface IRelayController
{
    /// <summary>Modbus对象</summary>
    IModbus Modbus { get; set; }

    /// <summary>主机地址</summary>
    Byte Host { get; set; }

    /// <summary>控制指定点位</summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    void Write(Int32 index, Boolean value);

    /// <summary>翻转指定点位</summary>
    /// <param name="index"></param>
    void Invert(Int32 index);

    /// <summary>控制指定点位</summary>
    /// <param name="value"></param>
    void WriteAll(Boolean value);

    /// <summary>翻转指定点位</summary>
    void InvertAll();

    /// <summary>读取指定点位</summary>
    /// <param name="index"></param>
    /// <returns></returns>
    Boolean Read(Int32 index);

    /// <summary>读取所有点位</summary>
    /// <returns></returns>
    Boolean[] ReadAll();
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
    /// <param name="value"></param>
    public virtual void Write(Int32 index, Boolean value) => Modbus.WriteCoil(Host, (UInt16)(StartAddress + index), (UInt16)(value ? 0xFF00 : 0x0000));

    /// <summary>翻转指定点位</summary>
    /// <param name="index"></param>
    public virtual void Invert(Int32 index) => Modbus.WriteCoil(Host, (UInt16)(StartAddress + index), 0x5500);

    /// <summary>控制指定点位</summary>
    /// <param name="value"></param>
    public virtual void WriteAll(Boolean value) => Modbus.WriteCoil(Host, 0x00FF, (UInt16)(value ? 0xFFFF : 0x0000));

    /// <summary>翻转指定点位</summary>
    public virtual void InvertAll() => Modbus.WriteCoil(Host, 0x00FF, 0x5a00);

    /// <summary>读取指定点位</summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual Boolean Read(Int32 index) => Modbus.ReadCoil(Host, (UInt16)(StartAddress + index), 1)[0];

    /// <summary>读取所有点位</summary>
    /// <returns></returns>
    public virtual Boolean[] ReadAll() => Modbus.ReadCoil(Host, StartAddress, (UInt16)Count);

    /// <summary>读取从机地址</summary>
    /// <returns></returns>
    public UInt16 ReadAddress() => Modbus.ReadRegister(0x00, 0, 1)[0];

    ///// <summary>读取从机波特率</summary>
    ///// <returns></returns>
    //public UInt16 ReadBaudrate() => Modbus.ReadRegister(0xFF, 0x03E8, 1)[0];
}