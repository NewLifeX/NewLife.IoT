namespace NewLife.IoT.ThingModels
{
    /// <summary>点位</summary>
    public interface IPoint
    {
        /// <summary>名称</summary>
        String Name { get; set; }

        /// <summary>地址。常规地址 0x06，大地址 40000，位域地址 0x46000:05，比特位置0~15</summary>
        String Address { get; set; }

        /// <summary>数据类型。来自物模型</summary>
        String Type { get; set; }

        /// <summary>大小。数据字节数，或字符串长度，Modbus寄存器一般占2个字节</summary>
        Int32 Length { get; set; }
    }
}