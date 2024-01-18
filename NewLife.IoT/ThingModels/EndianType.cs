namespace NewLife.IoT.ThingModels;

/// <summary>字节序类型</summary>
/// <remarks>
/// 大小端字节序，又称端序或尾序，是指在多字节的数据类型中，数值的字节的排列顺序。
/// 工控领域（PLC）常用ABCD/DCBA表示，网络领域常用1234/4321表示。
/// </remarks>
public enum EndianType : Byte
{
    /// <summary>大端字节序。ABCD/1234，高序字节存储在起始地址，也叫网络字节序。支持2/4/8字节</summary>
    BigEndian = 1,

    /// <summary>小端字节序。DCBA/4321，低序字节存储在起始地址，也叫主机字节序。支持2/4/8字节</summary>
    LittleEndian = 2,

    ///// <summary>中端字节序。低序字节存储在起始地址，但字节序不同的数据在内部字节的排序是不同的</summary>
    //MiddleEndian = 3,

    /// <summary>大端字节交换。BADC/2143，高序字存储在起始地址，每个字内部字节交换。仅支持4/8字节</summary>
    /// <remarks>Modbus发送Float常用</remarks>
    BigSwap = 3,

    /// <summary>小端字节交换。CDAB/3412，低序字存储在起始地址，每个字内部没有字节交换（相对于全倒序是交换了两次）。仅支持4/8字节</summary>
    LittleSwap = 4,
}

/// <summary>字节序类型</summary>
/// <remarks>
/// 大小端字节序，又称端序或尾序，是指在多字节的数据类型中，数值的字节的排列顺序。
/// 工控领域（PLC）常用ABCD/DCBA表示，网络领域常用1234/4321表示。
/// </remarks>
public enum ByteOrder : Byte
{
    /// <summary>大端字节序。ABCD/1234，高序字节存储在起始地址，也叫网络字节序。支持2/4/8字节</summary>
    ABCD = 1,

    /// <summary>小端字节序。DCBA/4321，低序字节存储在起始地址，也叫主机字节序。支持2/4/8字节</summary>
    DCBA = 2,

    /// <summary>大端字节交换。BADC/2143，高序字存储在起始地址，每个字内部字节交换。仅支持4/8字节</summary>
    /// <remarks>Modbus发送Float常用</remarks>
    BADC = 3,

    /// <summary>小端字节交换。CDAB/3412，低序字存储在起始地址，每个字内部没有字节交换（相对于全倒序是交换了两次）。仅支持4/8字节</summary>
    CDAB = 4,
}