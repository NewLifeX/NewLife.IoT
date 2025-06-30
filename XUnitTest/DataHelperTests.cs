using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using NewLife;
using NewLife.IoT;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTest;

public class DataHelperTests(ITestOutputHelper output)
{
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
    [InlineData(1234.5678, EndianType.BigEndian, "449A522B")]
    [InlineData(1234.5678, EndianType.LittleEndian, "2B529A44")]
    [InlineData(1234.5678, EndianType.BigSwap, "9A442B52")]
    [InlineData(1234.5678, EndianType.LittleSwap, "522B449A")]
    [InlineData(1.2, EndianType.BigEndian, "3F99999A")]
    [InlineData(-1.5, EndianType.BigEndian, "BFC00000")]
    public void GetBytesSingle(Single value, EndianType endian, String hex)
    {
        var buf = value.GetBytes(endian);
        Assert.Equal(hex, buf.ToHex());

        var rs = hex.ToHex().ToSingle(endian);
        Assert.Equal(value, rs);
    }

    [Theory]
    [InlineData(1234.5678, ByteOrder.ABCD, "449A522B")]
    [InlineData(1234.5678, ByteOrder.DCBA, "2B529A44")]
    [InlineData(1234.5678, ByteOrder.BADC, "9A442B52")]
    [InlineData(1234.5678, ByteOrder.CDAB, "522B449A")]
    [InlineData(1.2, ByteOrder.ABCD, "3F99999A")]
    [InlineData(-1.5, ByteOrder.ABCD, "BFC00000")]
    public void GetBytesSingle_Order(Single value, ByteOrder order, String hex)
    {
        var buf = value.GetBytes(order);
        Assert.Equal(hex, buf.ToHex());

        var rs = hex.ToHex().ToSingle(order);
        Assert.Equal(value, rs);
    }

    [Theory]
    [InlineData(-12.345678, EndianType.BigEndian, "C028B0FCB4F1E4B4")]
    [InlineData(-12.345678, EndianType.LittleEndian, "B4E4F1B4FCB028C0")]
    [InlineData(-12.345678, EndianType.BigSwap, "28C0FCB0F1B4B4E4")]
    [InlineData(-12.345678, EndianType.LittleSwap, "E4B4B4F1B0FCC028")]
    public void GetBytesDouble(Double value, EndianType endian, String hex)
    {
        var buf = value.GetBytes(endian);
        Assert.Equal(hex, buf.ToHex());

        var rs = hex.ToHex().ToDouble(endian);
        Assert.Equal(value, rs);
    }

    [Theory]
    [InlineData(-12.345678, ByteOrder.ABCD, "C028B0FCB4F1E4B4")]
    [InlineData(-12.345678, ByteOrder.DCBA, "B4E4F1B4FCB028C0")]
    [InlineData(-12.345678, ByteOrder.BADC, "28C0FCB0F1B4B4E4")]
    [InlineData(-12.345678, ByteOrder.CDAB, "E4B4B4F1B0FCC028")]
    public void GetBytesDouble_Order(Double value, ByteOrder order, String hex)
    {
        var buf = value.GetBytes(order);
        Assert.Equal(hex, buf.ToHex());

        var rs = hex.ToHex().ToDouble(order);
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
        output.WriteLine($"data={data} type={data.GetType().Name} hex={hex}");

        var spec = new ThingSpec
        {
            Properties = [PropertySpec.Create("test", data.GetType().Name, ByteOrder.ABCD)]
        };

        var rs = spec.Encode(data, "test");
        var buf = rs as Byte[];
        Assert.NotNull(buf);
        Assert.Equal(hex, buf.ToHex());

        var v = spec.Decode(hex.ToHex(), "test");
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
        output.WriteLine($"data={data} type={data.GetType().Name} hex={hex} endian={endian}");

        var spec = new ThingSpec
        {
            Properties = [PropertySpec.Create("test", null, (ByteOrder)endian)]
        };

        var point = new PointModel { Name = "test", Type = data.GetType().Name };

        var rs = spec.Encode(data, point);
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
        output.WriteLine($"data={data} type={data.GetType().Name} hex={hex} order={order}");

        var spec = new ThingSpec
        {
            Properties = [PropertySpec.Create("test", null, order)]
        };

        var point = new PointModel { Name = "test", Type = data.GetType().Name };

        var rs = spec.Encode(data, point);
        var buf = rs as Byte[];
        Assert.NotNull(buf);
        Assert.Equal(hex, buf.ToHex());
    }

    [Theory]
    [InlineData((Int16)0x1234, 1, 0, "1234")]
    [InlineData((UInt16)0x1234, 1, 0, "1234")]
    [InlineData((Int32)0x12345678, 1, 0, "12345678")]
    [InlineData((UInt32)0x12345678, 1, 0, "12345678")]
    [InlineData((Single)1.2, 1, 0, "3F99999A")]
    [InlineData((Double)(-12.345678), 1, 0, "C028B0FCB4F1E4B4")]
    [InlineData((Int16)0x1234, 0.1, 40, "B478")]
    [InlineData((UInt16)0x1234, 0.1, 40, "B478")]
    [InlineData((Int32)0x12345678, 0.1, 40, "B60B5F20")]
    [InlineData((UInt32)0x12345678, 0.1, 40, "B60B5F20")]
    [InlineData((Single)1.2, 0.1, 40, "C3C20000")]
    [InlineData((Double)(-12.345678), 0.1, 40, "C0805BA7782EE1DE")]
    [InlineData((Int16)0x1234, 0.3, 57, "3BEF")]
    [InlineData((UInt16)0x1234, 0.3, 57, "3BEF")]
    [InlineData((Int32)0x12345678, 0.3, 57, "3CAE74BA")]
    [InlineData((UInt32)0x12345678, 0.3, 57, "3CAE74BA")]
    [InlineData((Single)1.2, 0.3, 57, "C339FFFF")]
    [InlineData((Double)(-12.345678), 0.3, 57, "C06CE4DF3D19D027")]
    public void EncodeByThingModel_Scaling(Object data, Single scaling, Single constant, String hex)
    {
        output.WriteLine($"type={data.GetType().Name} scaling={scaling} constant={constant} hex={hex}");

        var order = ByteOrder.ABCD;
        if (data is Int16 n16)
            output.WriteLine(n16.GetBytes(order).ToHex());
        else if (data is UInt16 u16)
            output.WriteLine(u16.GetBytes(order).ToHex());
        else if (data is Int32 n32)
            output.WriteLine(n32.GetBytes(order).ToHex());
        else if (data is UInt32 u32)
            output.WriteLine(u32.GetBytes(order).ToHex());
        else if (data is Single f)
            output.WriteLine(f.GetBytes(order).ToHex());
        else if (data is Double d)
            output.WriteLine(d.GetBytes(order).ToHex());

        var spec = new ThingSpec
        {
            Properties = [PropertySpec.Create("test", null, ByteOrder.ABCD)]
        };
        var specs = spec.GetProperty("test");
        specs.Scaling = scaling;
        specs.Constant = constant;

        var point = new PointModel { Name = "test", Type = data.GetType().Name };

        var rs = spec.Encode(data, point);
        var buf = rs as Byte[];
        Assert.NotNull(buf);
        Assert.Equal(hex, buf.ToHex());
    }

    [Fact]
    public void DecodeByThingModel()
    {
        var hex = "02-84-02-97-02-84";
        var buf = hex.ToHex();

        var humi = buf.ToUInt16(0, false) / 1000.0;
        var temp = buf.ToUInt16(2, false) / 10.0 - 40;

        var spec = new ThingSpec();
        var pi = spec.AddProperty("temp", "温度", "float");
        pi.Scaling = 0.1f;
        pi.Constant = -40;
        pi = spec.AddProperty("humi", "湿度", "float");
        pi.Scaling = 0.001f;
        pi.Constant = 0;

        var point = new PointModel { Name = "humi", Type = "float" };
        var h = spec.Decode(buf, point);
        Assert.Equal(Math.Round(humi, 4), Math.Round((Single)h, 4));

        var t = spec.Decode(buf.Skip(2).ToArray(), "temp");
        Assert.Equal(Math.Round(temp, 4), Math.Round((Single)t, 4));
    }
}
