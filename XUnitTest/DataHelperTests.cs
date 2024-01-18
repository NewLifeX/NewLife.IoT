using System;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using NewLife;
using NewLife.IoT;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTest;

public class DataHelperTests
{
    private readonly ITestOutputHelper _output;

    public DataHelperTests(ITestOutputHelper output) => _output = output;

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

    [Theory]
    [InlineData((Byte)0x12, "12")]
    [InlineData((Int16)0x1234, "1234")]
    [InlineData((UInt16)0x1234, "1234")]
    [InlineData((Int32)0x12345678, "12345678")]
    [InlineData((UInt32)0x12345678, "12345678")]
    public void EncodeByThingModel(Object data, String hex)
    {
        _output.WriteLine($"data={data} type={data.GetType().Name} hex={hex}");

        var property = new PropertyExtend { Id = "test" };
        var spec = new ThingSpec
        {
            ExtendedProperties = new[] { property }
        };

        var point = new PointModel { Name = "test", Type = data.GetType().Name };

        var rs = spec.EncodeByThingModel(data, point);
        var buf = rs as Byte[];
        Assert.NotNull(buf);
        Assert.Equal(hex, buf.ToHex());
    }

    [Theory]
    [InlineData((Byte)0x12, EndianType.BigEndian, "12")]
    [InlineData((Int16)0x1234, EndianType.BigEndian, "1234")]
    [InlineData((UInt16)0x1234, EndianType.BigEndian, "1234")]
    [InlineData((Int32)0x12345678, EndianType.BigEndian, "12345678")]
    [InlineData((UInt32)0x12345678, EndianType.BigEndian, "12345678")]
    [InlineData((Byte)0x12, EndianType.LittleEndian, "12")]
    [InlineData((Int16)0x1234, EndianType.LittleEndian, "3412")]
    [InlineData((UInt16)0x1234, EndianType.LittleEndian, "3412")]
    [InlineData((Int32)0x12345678, EndianType.LittleEndian, "78563412")]
    [InlineData((UInt32)0x12345678, EndianType.LittleEndian, "78563412")]
    [InlineData((Byte)0x12, EndianType.BigSwap, "12")]
    [InlineData((Int16)0x1234, EndianType.BigSwap, "1234")]
    [InlineData((UInt16)0x1234, EndianType.BigSwap, "1234")]
    [InlineData((Int32)0x12345678, EndianType.BigSwap, "34127856")]
    [InlineData((UInt32)0x12345678, EndianType.BigSwap, "34127856")]
    [InlineData((Byte)0x12, EndianType.LittleSwap, "12")]
    [InlineData((Int16)0x1234, EndianType.LittleSwap, "3412")]
    [InlineData((UInt16)0x1234, EndianType.LittleSwap, "3412")]
    [InlineData((Int32)0x12345678, EndianType.LittleSwap, "56781234")]
    [InlineData((UInt32)0x12345678, EndianType.LittleSwap, "56781234")]
    public void EncodeByThingModel_Endian(Object data, EndianType endian, String hex)
    {
        _output.WriteLine($"data={data} type={data.GetType().Name} hex={hex} endian={endian}");

        var property = new PropertyExtend { Id = "test", Endian = endian };
        var spec = new ThingSpec
        {
            ExtendedProperties = new[] { property }
        };

        var point = new PointModel { Name = "test", Type = data.GetType().Name };

        var rs = spec.EncodeByThingModel(data, point);
        var buf = rs as Byte[];
        Assert.NotNull(buf);
        Assert.Equal(hex, buf.ToHex());
    }

    [Theory]
    [InlineData((Int16)0x1234, 1, 0, "1234")]
    [InlineData((UInt16)0x1234, 1, 0, "1234")]
    [InlineData((Int32)0x12345678, 1, 0, "12345678")]
    [InlineData((UInt32)0x12345678, 1, 0, "12345678")]
    [InlineData((Int16)0x1234, 0.1, 40, "B478")]
    [InlineData((UInt16)0x1234, 0.1, 40, "B478")]
    [InlineData((Int32)0x12345678, 0.1, 40, "B60B5F20")]
    [InlineData((UInt32)0x12345678, 0.1, 40, "B60B5F20")]
    [InlineData((Int16)0x1234, 0.3, 57, "3BEF")]
    [InlineData((UInt16)0x1234, 0.3, 57, "3BEF")]
    [InlineData((Int32)0x12345678, 0.3, 57, "3CAE74BA")]
    [InlineData((UInt32)0x12345678, 0.3, 57, "3CAE74BA")]
    public void EncodeByThingModel_Scaling(Object data, Single scaling, Single constant, String hex)
    {
        _output.WriteLine($"type={data.GetType().Name} scaling={scaling} constant={constant} hex={hex}");

        var property = new PropertyExtend { Id = "test", Scaling = scaling, Constant = constant };
        var spec = new ThingSpec
        {
            ExtendedProperties = new[] { property }
        };

        var point = new PointModel { Name = "test", Type = data.GetType().Name };

        var rs = spec.EncodeByThingModel(data, point);
        var buf = rs as Byte[];
        Assert.NotNull(buf);
        Assert.Equal(hex, buf.ToHex());
    }
}
