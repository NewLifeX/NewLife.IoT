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
}
