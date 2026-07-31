using System;
using System.Text;
using NewLife;
using NewLife.IoT;
using NewLife.IoT.ThingModels;
using NewLife.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTest;

public class PointHelperTests
{
    private readonly ITestOutputHelper _output;

    public PointHelperTests(ITestOutputHelper output) => _output = output;

    [Theory]
    [InlineData((Byte)0x12, ByteOrder.ABCD, "12")]
    [InlineData((Int16)0x1234, ByteOrder.ABCD, "1234")]
    [InlineData((UInt16)0x1234, ByteOrder.ABCD, "1234")]
    [InlineData((Int32)0x12345678, ByteOrder.ABCD, "12345678")]
    [InlineData((UInt32)0x12345678, ByteOrder.ABCD, "12345678")]
    [InlineData((Single)0.1234f, ByteOrder.ABCD, "24B9FC3D")]
    [InlineData((Single)12.34, ByteOrder.ABCD, "A4704541")]
    [InlineData((Double)1234.5678, ByteOrder.ABCD, "ADFA5C6D454A9340")]
    [InlineData((Byte)0x12, ByteOrder.DCBA, "12")]
    [InlineData((Int16)0x1234, ByteOrder.DCBA, "3412")]
    [InlineData((UInt16)0x1234, ByteOrder.DCBA, "3412")]
    [InlineData((Int32)0x12345678, ByteOrder.DCBA, "78563412")]
    [InlineData((UInt32)0x12345678, ByteOrder.DCBA, "78563412")]
    [InlineData((Single)12.34, ByteOrder.DCBA, "414570A4")]
    [InlineData((Double)1234.5678, ByteOrder.DCBA, "40934A456D5CFAAD")]
    [InlineData((Byte)0x12, ByteOrder.BADC, "12")]
    [InlineData((Int16)0x1234, ByteOrder.BADC, "3412")]
    [InlineData((UInt16)0x1234, ByteOrder.BADC, "3412")]
    [InlineData((Int32)0x12345678, ByteOrder.BADC, "34127856")]
    [InlineData((UInt32)0x12345678, ByteOrder.BADC, "34127856")]
    [InlineData((Single)12.34, ByteOrder.BADC, "70A44145")]
    [InlineData((Double)1234.5678, ByteOrder.BADC, "FAAD6D5C4A454093")]
    [InlineData((Byte)0x12, ByteOrder.CDAB, "12")]
    [InlineData((Int16)0x1234, ByteOrder.CDAB, "1234")]
    [InlineData((UInt16)0x1234, ByteOrder.CDAB, "1234")]
    [InlineData((Int32)0x12345678, ByteOrder.CDAB, "56781234")]
    [InlineData((UInt32)0x12345678, ByteOrder.CDAB, "56781234")]
    [InlineData((Single)12.34, ByteOrder.CDAB, "4541A470")]
    [InlineData((Double)1234.5678, ByteOrder.CDAB, "9340454A5C6DADFA")]
    public void GetBytes(Object data, ByteOrder order, String hex)
    {
        _output.WriteLine($"data={data} type={data.GetType().Name} hex={hex} order={order}");

        var point = new PointModel { Name = "test", Type = data.GetType().Name };

        // GetBytes返回小端，先倒序，再转为目标字节序
        var rs = point.GetBytes(data);
        if (data.GetType().IsInt()) rs = rs.Swap(ByteOrder.DCBA);
        rs = rs.Swap(order);
        var buf = rs as Byte[];
        Assert.NotNull(buf);
        Assert.Equal(hex, buf.ToHex());

        // 先倒序转为小端，再转为目标字节序
        buf = hex.ToHex();
        if (data.GetType().IsInt()) buf = buf.Swap(ByteOrder.DCBA);
        buf = buf.Swap(order);
        var v = point.Convert(buf);
        Assert.Equal(data, v);
    }

    // ===== ConvertToWord =====

    private static UInt16[] ParseWords(String hex)
    {
        var buf = hex.ToHex();
        var words = new UInt16[buf.Length / 2];
        for (var i = 0; i < words.Length; i++)
            words[i] = (UInt16)((buf[i * 2] << 8) | buf[i * 2 + 1]);
        return words;
    }

    [Theory]
    [InlineData(true, "0001")]
    [InlineData(false, "0000")]
    [InlineData((Byte)0x01, "0001")]
    [InlineData((Int16)0x1234, "1234")]
    [InlineData((UInt16)0x1234, "1234")]
    [InlineData((Int32)0x12345678, "12345678")]
    [InlineData((UInt32)0x12345678, "12345678")]
    [InlineData((Int32)(-0x12345678), "EDCBA988")]
    [InlineData((Int64)0x123456789ABCDEF0, "123456789ABCDEF0")]
    [InlineData((Int64)(-0x123456789ABCDEF0), "EDCBA98765432110")]
    public void ConvertToWord_IntegerTypes_BigEndianRegisters(Object data, String hex)
    {
        var point = new PointModel { Name = "test", Type = data.GetType().Name };
        var words = point.ConvertToWord(data);

        Assert.NotNull(words);
        Assert.Equal(ParseWords(hex), words);
    }

    [Fact]
    public void ConvertToWord_Single_OutputIeee754Bits()
    {
        var point = new PointModel { Name = "test", Type = "Single" };

        // 12.34f = 0x414570A4，高字在前
        Assert.Equal(ParseWords("414570A4"), point.ConvertToWord(12.34f));
        // 负数符号位翻转
        Assert.Equal(ParseWords("C14570A4"), point.ConvertToWord(-12.34f));
        // 零
        Assert.Equal(ParseWords("00000000"), point.ConvertToWord(0f));
    }

    [Fact]
    public void ConvertToWord_Double_OutputIeee754Bits()
    {
        var point = new PointModel { Name = "test", Type = "Double" };

        // 1234.5678 = 0x40934A456D5CFAAD，高字在前
        Assert.Equal(ParseWords("40934A456D5CFAAD"), point.ConvertToWord(1234.5678));
        // 负数符号位翻转
        Assert.Equal(ParseWords("C0934A456D5CFAAD"), point.ConvertToWord(-1234.5678));
        // 零
        Assert.Equal(ParseWords("0000000000000000"), point.ConvertToWord(0d));
    }

    [Fact]
    public void ConvertToWord_Decimal_OutputInt64Registers()
    {
        var point = new PointModel { Name = "test", Type = "Decimal" };

        // 12.34 截断为 12，补码输出 4 个寄存器
        Assert.Equal(ParseWords("000000000000000C"), point.ConvertToWord(12.34m));
        // -12.34 截断为 -12，64位补码 0xFFFFFFFFFFFFFFF4
        Assert.Equal(ParseWords("FFFFFFFFFFFFFFF4"), point.ConvertToWord(-12.34m));
    }
}
