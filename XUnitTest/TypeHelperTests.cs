using System;
using NewLife.IoT;
using NewLife.IoT.ThingModels;
using Xunit;

namespace XUnitTest;

public class TypeHelperTests
{
    [Fact]
    public void GetLength()
    {
        Assert.Equal(1, TypeHelper.GetLength("bit"));
        Assert.Equal(1, TypeHelper.GetLength("bool"));

        Assert.Equal(2, TypeHelper.GetLength("short"));
        Assert.Equal(2, TypeHelper.GetLength("number"));

        Assert.Equal(4, TypeHelper.GetLength("uint32"));
        Assert.Equal(4, TypeHelper.GetLength("float"));

        Assert.Equal(8, TypeHelper.GetLength("ulong"));
        Assert.Equal(8, TypeHelper.GetLength("double"));
    }

    [Fact]
    public void GetLength2()
    {
        {
            var point = new PointModel { Type = "int", Length = 2 };
            Assert.Equal(2, point.GetLength());
        }
        {
            var point = new PointModel { Type = "int", Length = 0 };
            Assert.Equal(4, point.GetLength());
        }
    }

    [Fact]
    public void GetNetType()
    {
        Assert.Equal(typeof(Boolean), TypeHelper.GetNetType("bool"));
        Assert.Equal(typeof(UInt16), TypeHelper.GetNetType("ushort"));
        Assert.Equal(typeof(Int32), TypeHelper.GetNetType("int"));
        Assert.Equal(typeof(Single), TypeHelper.GetNetType("float"));
        Assert.Equal(typeof(Int16), TypeHelper.GetNetType("number"));
        Assert.Equal(typeof(String), TypeHelper.GetNetType("text"));
        Assert.Equal(typeof(DateTime), TypeHelper.GetNetType("time"));
        Assert.Equal(typeof(DateTime), TypeHelper.GetNetType("date"));
    }

    [Fact]
    public void GetNetType2()
    {
        {
            var point = new PointModel { Type = "int", Length = 2 };
            Assert.Equal(typeof(Int16), point.GetNetType());
        }
        {
            var point = new PointModel { Type = "int", Length = 0 };
            Assert.Equal(typeof(Int32), point.GetNetType());
        }
    }

    [Fact]
    public void SetNetType()
    {
        {
            var point = new PointModel();
            point.SetNetType(typeof(UInt16));

            Assert.Equal("int", point.Type);
            Assert.Equal(2, point.Length);
        }
        {
            var point = new PointModel();
            point.SetNetType(typeof(Single));

            Assert.Equal("float", point.Type);
            Assert.Equal(4, point.Length);
        }
        {
            var point = new PointModel();
            point.SetNetType(typeof(Double));

            Assert.Equal("float", point.Type);
            Assert.Equal(8, point.Length);
        }
    }

    [Fact]
    public void SetNetType2()
    {
        var ntype = TypeHelper.GetNetType("String");
        Assert.Equal(typeof(String), ntype);
    }

    [Fact]
    public void GetIoTType()
    {
        {
            Assert.Equal("int", TypeHelper.GetIoTType(typeof(UInt16), false));
            Assert.Equal("float", TypeHelper.GetIoTType(typeof(Double), false));
        }
        {
            Assert.Equal("short", TypeHelper.GetIoTType(typeof(UInt16), true));
            Assert.Equal("double", TypeHelper.GetIoTType(typeof(Double), true));
        }
    }

    [Fact]
    public void GetIoTType2()
    {
        {
            var point = new PointModel { Type = "int", Length = 2 };
            Assert.Equal("int", point.GetIoTType(false));
        }
        {
            var point = new PointModel { Type = "int", Length = 2 };
            Assert.Equal("short", point.GetIoTType(true));
        }
        {
            var point = new PointModel { Type = "double", Length = 0 };
            Assert.Equal("float", point.GetIoTType(false));
        }
        {
            var point = new PointModel { Type = "double", Length = 0 };
            Assert.Equal("double", point.GetIoTType(true));
        }
        {
            var point = new PointModel { Type = "double", Length = 2 };
            Assert.Equal("float", point.GetIoTType(true));
        }
    }

    [Fact]
    public void GetIoTTypes()
    {
        {
            var dic = TypeHelper.GetIoTTypes(false);
            Assert.False(dic.ContainsKey("double"));
        }
        {
            var dic = TypeHelper.GetIoTTypes(true);
            Assert.True(dic.ContainsKey("double"));
        }
    }
}