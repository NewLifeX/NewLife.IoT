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
    [InlineData(0xABCD, EndianType.BigSwap, "CDAB")]
    [InlineData(0xABCD, EndianType.LittleSwap, "ABCD")]
    public void GetBytesUInt16(UInt16 value, EndianType endian, String hex)
    {
        var buf = value.GetBytes(endian);
        Assert.Equal(hex, buf.ToHex());

        var rs = hex.ToHex().ToUInt16(endian);
        Assert.Equal(value, rs);
    }

    [Theory]
    [InlineData(0xABCD, ByteOrder.ABCD, "ABCD")]
    [InlineData(0xABCD, ByteOrder.DCBA, "CDAB")]
    [InlineData(0xABCD, ByteOrder.BADC, "CDAB")]
    [InlineData(0xABCD, ByteOrder.CDAB, "ABCD")]
    public void GetBytesUInt16_Order(UInt16 value, ByteOrder order, String hex)
    {
        var buf = value.GetBytes(order);
        Assert.Equal(hex, buf.ToHex());

        var rs = hex.ToHex().ToUInt16(order);
        Assert.Equal(value, rs);
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

        var rs = hex.ToHex().ToUInt32(endian);
        Assert.Equal(value, rs);
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

        var rs = hex.ToHex().ToUInt32(order);
        Assert.Equal(value, rs);
    }

    [Theory]
    [InlineData("12345678", ByteOrder.ABCD, "12345678")]
    [InlineData("12345678", ByteOrder.DCBA, "78563412")]
    [InlineData("12345678", ByteOrder.BADC, "34127856")]
    [InlineData("12345678", ByteOrder.CDAB, "56781234")]
    [InlineData("12345678", (ByteOrder)0, "12345678")]
    [InlineData("12", ByteOrder.DCBA, "12")]
    [InlineData("1234", ByteOrder.DCBA, "3412")]
    [InlineData("123456", ByteOrder.DCBA, "563412")]
    [InlineData("12", ByteOrder.BADC, "12")]
    [InlineData("1234", ByteOrder.BADC, "3412")]
    [InlineData("123456", ByteOrder.BADC, "341256")]
    public void Swap(String hex, ByteOrder order, String hex2)
    {
        var buf = hex.ToHex();
        var rs = buf.Swap(order);
        Assert.Equal(hex2, rs.ToHex());
    }

    [Fact]
    public void Swap2()
    {
        var f = 12.34f;
        var buf = BitConverter.GetBytes(f);
        Assert.Equal("A4704541", buf.ToHex());

        f = 0.1234f;
        buf = BitConverter.GetBytes(f);
        Assert.Equal("24B9FC3D", buf.ToHex());
        buf = buf.Swap(ByteOrder.BADC);
        Assert.Equal("B9243DFC", buf.ToHex());

        var d = 1234.5678d;
        buf = BitConverter.GetBytes(d);
        Assert.Equal("ADFA5C6D454A9340", buf.ToHex());
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

        var v = spec.DecodeByThingModel(hex.ToHex(), point);
        Assert.Equal(data, v);
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
    [InlineData((Int16)0x1234, EndianType.BigSwap, "3412")]
    [InlineData((UInt16)0x1234, EndianType.BigSwap, "3412")]
    [InlineData((Int32)0x12345678, EndianType.BigSwap, "34127856")]
    [InlineData((UInt32)0x12345678, EndianType.BigSwap, "34127856")]
    [InlineData((Byte)0x12, EndianType.LittleSwap, "12")]
    [InlineData((Int16)0x1234, EndianType.LittleSwap, "1234")]
    [InlineData((UInt16)0x1234, EndianType.LittleSwap, "1234")]
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
    [InlineData((Byte)0x12, ByteOrder.ABCD, "12")]
    [InlineData((Int16)0x1234, ByteOrder.ABCD, "1234")]
    [InlineData((UInt16)0x1234, ByteOrder.ABCD, "1234")]
    [InlineData((Int32)0x12345678, ByteOrder.ABCD, "12345678")]
    [InlineData((UInt32)0x12345678, ByteOrder.ABCD, "12345678")]
    [InlineData((Byte)0x12, ByteOrder.DCBA, "12")]
    [InlineData((Int16)0x1234, ByteOrder.DCBA, "3412")]
    [InlineData((UInt16)0x1234, ByteOrder.DCBA, "3412")]
    [InlineData((Int32)0x12345678, ByteOrder.DCBA, "78563412")]
    [InlineData((UInt32)0x12345678, ByteOrder.DCBA, "78563412")]
    [InlineData((Byte)0x12, ByteOrder.BADC, "12")]
    [InlineData((Int16)0x1234, ByteOrder.BADC, "3412")]
    [InlineData((UInt16)0x1234, ByteOrder.BADC, "3412")]
    [InlineData((Int32)0x12345678, ByteOrder.BADC, "34127856")]
    [InlineData((UInt32)0x12345678, ByteOrder.BADC, "34127856")]
    [InlineData((Byte)0x12, ByteOrder.CDAB, "12")]
    [InlineData((Int16)0x1234, ByteOrder.CDAB, "1234")]
    [InlineData((UInt16)0x1234, ByteOrder.CDAB, "1234")]
    [InlineData((Int32)0x12345678, ByteOrder.CDAB, "56781234")]
    [InlineData((UInt32)0x12345678, ByteOrder.CDAB, "56781234")]
    public void EncodeByThingModel_Order(Object data, ByteOrder order, String hex)
    {
        _output.WriteLine($"data={data} type={data.GetType().Name} hex={hex} order={order}");

        var property = new PropertyExtend { Id = "test", Order = order };
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
    [InlineData((Single)12.34, 1, 0, "A4704541")]
    [InlineData((Double)1234.5678, 1, 0, "ADFA5C6D454A9340")]
    [InlineData((Int16)0x1234, 0.1, 40, "B478")]
    [InlineData((UInt16)0x1234, 0.1, 40, "B478")]
    [InlineData((Int32)0x12345678, 0.1, 40, "B60B5F20")]
    [InlineData((UInt32)0x12345678, 0.1, 40, "B60B5F20")]
    [InlineData((Single)12.34, 0.1, 40, "CD4C8AC3")]
    [InlineData((Double)1234.5678, 0.1, 40, "A803DFC2D654C740")]
    [InlineData((Int16)0x1234, 0.3, 57, "3BEF")]
    [InlineData((UInt16)0x1234, 0.3, 57, "3BEF")]
    [InlineData((Int32)0x12345678, 0.3, 57, "3CAE74BA")]
    [InlineData((UInt32)0x12345678, 0.3, 57, "3CAE74BA")]
    [InlineData((Single)12.34, 0.3, 57, "DDDD14C3")]
    [InlineData((Double)1234.5678, 0.3, 57, "5FFFD3A173AAAE40")]
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
