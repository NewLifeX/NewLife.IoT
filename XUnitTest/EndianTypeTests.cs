using System;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using Xunit;

namespace XUnitTest;

public class EndianTypeTests
{
    [Fact]
    public void Test1()
    {
        var type = EndianType.BigEndian;
        Assert.Equal(1, (Byte)type);
        Assert.Equal("BigEndian", type + "");

        var type2 = (ByteOrder)type;
        Assert.Equal(1, (Byte)type2);
        Assert.Equal("ABCD", type2 + "");
    }

    [Fact]
    public void Test2()
    {
        var type = EndianType.LittleEndian;
        Assert.Equal(2, (Byte)type);
        Assert.Equal("LittleEndian", type + "");

        var type2 = (ByteOrder)type;
        Assert.Equal(2, (Byte)type2);
        Assert.Equal("DCBA", type2 + "");
    }

    [Fact]
    public void Test3()
    {
        var type = EndianType.BigSwap;
        Assert.Equal(3, (Byte)type);
        Assert.Equal("BigSwap", type + "");

        var type2 = (ByteOrder)type;
        Assert.Equal(3, (Byte)type2);
        Assert.Equal("BADC", type2 + "");
    }

    [Fact]
    public void Test4()
    {
        var type = EndianType.LittleSwap;
        Assert.Equal(4, (Byte)type);
        Assert.Equal("LittleSwap", type + "");

        var type2 = (ByteOrder)type;
        Assert.Equal(4, (Byte)type2);
        Assert.Equal("CDAB", type2 + "");
    }

    //[Theory]
    //[InlineData(EndianType.BigEndian, false, false)]
    //[InlineData(EndianType.LittleEndian, true, true)]
    //[InlineData(EndianType.BigSwap, true, false)]
    //[InlineData(EndianType.LittleSwap, false, true)]
    //public void PropertyExtendTest(EndianType endian, Boolean swap16, Boolean swap32)
    //{
    //    var property = new PropertyExtend { Endian = endian };
    //    Assert.Equal(swap16, property.Swap16);
    //    Assert.Equal(swap32, property.Swap32);
    //}

    //[Theory]
    //[InlineData(ByteOrder.ABCD, false, false)]
    //[InlineData(ByteOrder.DCBA, true, true)]
    //[InlineData(ByteOrder.BADC, true, false)]
    //[InlineData(ByteOrder.CDAB, false, true)]
    //public void PropertyExtendTest2(ByteOrder order, Boolean swap16, Boolean swap32)
    //{
    //    var property = new PropertyExtend { Order = order };
    //    Assert.Equal(swap16, property.Swap16);
    //    Assert.Equal(swap32, property.Swap32);
    //}
}
