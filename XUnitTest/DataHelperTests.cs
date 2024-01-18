using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLife;
using NewLife.IoT.ThingModels;
using Xunit;
using NewLife.IoT;

namespace XUnitTest;

public class DataHelperTests
{
    [Theory]
    [InlineData(0xABCD, EndianType.BigEndian, "ABCD")]
    [InlineData(0xABCD, EndianType.LittleEndian, "CDAB")]
    [InlineData(0xABCD, EndianType.BigSwap, "ABCD")]
    [InlineData(0xABCD, EndianType.LittleSwap, "CDAB")]
    public void GetBytesUInt16(UInt16 value, EndianType endian, String hex)
    {
        var buf = value.GetBytes(endian);
        Assert.Equal(hex, buf.ToHex());
    }

    [Theory]
    [InlineData(0xABCD, ByteOrder.ABCD, "ABCD")]
    [InlineData(0xABCD, ByteOrder.DCBA, "CDAB")]
    [InlineData(0xABCD, ByteOrder.BADC, "ABCD")]
    [InlineData(0xABCD, ByteOrder.CDAB, "CDAB")]
    public void GetBytesUInt16_Order(UInt16 value, ByteOrder order, String hex)
    {
        var buf = value.GetBytes(order);
        Assert.Equal(hex, buf.ToHex());
    }

    [Theory]
    [InlineData(0x12345678, EndianType.BigEndian, "12345678")]
    [InlineData(0x12345678, EndianType.LittleEndian, "78563412")]
    [InlineData(0x12345678, EndianType.BigSwap, "34127856")]
    [InlineData(0x12345678, EndianType.LittleSwap, "56781234")]
    public void GetBytesUInt32(UInt32 value, EndianType endian, String hex)
    {
        var buf = value.GetBytes(endian);
        Assert.Equal(hex, buf.ToHex());
    }

    [Theory]
    [InlineData(0x12345678, ByteOrder.ABCD, "12345678")]
    [InlineData(0x12345678, ByteOrder.DCBA, "78563412")]
    [InlineData(0x12345678, ByteOrder.BADC, "34127856")]
    [InlineData(0x12345678, ByteOrder.CDAB, "56781234")]
    public void GetBytesUInt32_Order(UInt32 value, ByteOrder order, String hex)
    {
        var buf = value.GetBytes(order);
        Assert.Equal(hex, buf.ToHex());
    }
}
