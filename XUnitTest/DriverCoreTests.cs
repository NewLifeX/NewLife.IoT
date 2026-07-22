using System;
using System.Collections.Generic;
using System.Linq;
using NewLife.IoT.Drivers;
using NewLife.IoT.ThingModels;
using Xunit;

namespace XUnitTest;

/// <summary>核心驱动结果类型单元测试</summary>
public class DriverCoreTests
{
    #region ReadResult

    [Fact]
    public void ReadResult_Success_CreatesValidResult()
    {
        var points = new IPoint[2];
        var values = new Object?[] { 42, "hello" };

        var result = ReadResult.Success(points, values);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Points.Length);
        Assert.Equal(2, result.Values.Length);
        Assert.Equal(42, result.Values[0]);
        Assert.Equal("hello", result.Values[1]);
        Assert.Equal(DataQuality.Good, result.Quality);
        Assert.Equal(IoTErrorCode.None, result.Code);
        Assert.Null(result.Message);
    }

    [Fact]
    public void ReadResult_Success_WithQualityAndDiagnostics()
    {
        var points = new[] { new PointModel { Name = "test" } };
        var values = new Object?[] { 100 };
        var requestBytes = new Byte[] { 0x01, 0x03 };
        var responseBytes = new Byte[] { 0x01, 0x03, 0x04 };

        var result = ReadResult.Success(points, values, DataQuality.Uncertain, requestBytes, responseBytes);

        Assert.True(result.IsSuccess);
        Assert.Equal(DataQuality.Uncertain, result.Quality);
        Assert.Equal(requestBytes, result.RequestBytes);
        Assert.Equal(responseBytes, result.ResponseBytes);
    }

    [Fact]
    public void ReadResult_GetValue_ByName()
    {
        var points = new IPoint[]
        {
            new PointModel { Name = "Temperature", Address = "4x0001" },
            new PointModel { Name = "Humidity", Address = "4x0002" }
        };
        var values = new Object?[] { 25.5, 60.0 };

        var result = ReadResult.Success(points, values);

        Assert.Equal(25.5, result.GetValue("Temperature"));
        Assert.Equal(60.0, result.GetValue("Humidity"));
        Assert.Null(result.GetValue("NotFound"));
    }

    [Fact]
    public void ReadResult_GetValue_ByAddress()
    {
        var points = new IPoint[]
        {
            new PointModel { Name = "T", Address = "4x0001" }
        };
        var values = new Object?[] { 42 };

        var result = ReadResult.Success(points, values);

        Assert.Equal(42, result.GetValue("4x0001"));
    }

    [Fact]
    public void ReadResult_IExtend()
    {
        var result = ReadResult.Success([], []);

        result["custom"] = "value";

        Assert.Equal("value", result["custom"]);
        Assert.Single(result.Items);
    }

    #endregion

    #region WriteResult

    [Fact]
    public void WriteResult_Success_SetsAffectedCount()
    {
        var result = WriteResult.Success(echoValue: 99);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.AffectedCount);
        Assert.Equal(99, result.EchoValue);
        Assert.Equal(IoTErrorCode.None, result.Code);
    }

    [Fact]
    public void WriteResult_SuccessBatch_SetsCorrectCount()
    {
        var result = WriteResult.SuccessBatch(5);

        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.AffectedCount);
        Assert.Null(result.EchoValue);
    }

    [Fact]
    public void WriteResult_Fail()
    {
        var result = WriteResult.Fail(IoTErrorCode.WriteError, "写入超时");

        Assert.False(result.IsSuccess);
        Assert.Equal(IoTErrorCode.WriteError, result.Code);
        Assert.Equal("写入超时", result.Message);
        Assert.Equal(0, result.AffectedCount);
    }

    [Fact]
    public void WriteResult_ToString_Output()
    {
        Assert.Contains("Success", WriteResult.Success().ToString());
        Assert.Contains("Fail", WriteResult.Fail(IoTErrorCode.Timeout, "timeout").ToString());
    }

    #endregion

    #region ControlResult

    [Fact]
    public void ControlResult_Success_WithOutput()
    {
        var output = new Dictionary<String, Object?> { ["result"] = "ok" };
        var result = ControlResult.Success(output);

        Assert.True(result.IsSuccess);
        Assert.Equal("ok", result.OutputParameters["result"]);
    }

    [Fact]
    public void ControlResult_Success_NoOutput()
    {
        var result = ControlResult.Success();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.OutputParameters);
    }

    [Fact]
    public void ControlResult_Fail()
    {
        var result = ControlResult.Fail(IoTErrorCode.NotSupported, "不支持");

        Assert.False(result.IsSuccess);
        Assert.Equal(IoTErrorCode.NotSupported, result.Code);
        Assert.Equal("不支持", result.Message);
    }

    [Fact]
    public void ControlResult_IExtend()
    {
        var result = ControlResult.Success();

        result["elapsed"] = 150;

        Assert.Equal(150, result["elapsed"]);
    }

    [Fact]
    public void ControlResult_ToString()
    {
        Assert.Contains("Success", ControlResult.Success().ToString());
        Assert.Contains("Fail", ControlResult.Fail(IoTErrorCode.Timeout, "to").ToString());
    }

    #endregion

    #region IDriverResult 多态

    [Fact]
    public void IDriverResult_Polymorphism()
    {
        IDriverResult[] results =
        [
            ReadResult.Success([], []),
            WriteResult.Success(),
            ControlResult.Success(),
            WriteResult.Fail(IoTErrorCode.Timeout, "超时")
        ];

        Assert.Equal(3, results.Count(r => r.IsSuccess));
        Assert.Single(results.Where(r => !r.IsSuccess));
        Assert.Equal(IoTErrorCode.Timeout, results.Last().Code);
    }

    #endregion

    #region DriverDataEventArgs

    [Fact]
    public void DriverDataEventArgs_Properties()
    {
        var node = new Node { IsConnected = true };
        var points = new IPoint[] { new PointModel { Name = "p1" } };
        var result = ReadResult.Success(points, [1]);
        var args = new DriverDataEventArgs { Node = node, Points = points, Result = result };

        Assert.Equal(node, args.Node);
        Assert.Equal(points, args.Points);
        Assert.Equal(result, args.Result);
        Assert.Equal(result.Timestamp, args.Timestamp);
    }

    [Fact]
    public void DriverDataEventArgs_Timestamp_Fallback()
    {
        var args = new DriverDataEventArgs { Node = new Node(), Points = [] };

        var ts = args.Timestamp;
        Assert.InRange(ts, DateTime.UtcNow.AddSeconds(-2), DateTime.UtcNow.AddSeconds(2));
    }

    #endregion

    #region IoTException

    [Fact]
    public void IoTException_Creation()
    {
        var ex = new IoTException(IoTErrorCode.ConnectionFailed, "无法连接");

        Assert.Equal(IoTErrorCode.ConnectionFailed, ex.Code);
        Assert.Equal("无法连接", ex.Message);
    }

    [Fact]
    public void IoTException_WithInnerException()
    {
        var inner = new InvalidOperationException("内部错误");
        var ex = new IoTException(IoTErrorCode.ProtocolError, "协议错误", inner);

        Assert.Equal(IoTErrorCode.ProtocolError, ex.Code);
        Assert.Same(inner, ex.InnerException);
    }

    #endregion

    #region IoTErrorCode

    [Fact]
    public void IoTErrorCode_Segments()
    {
        Assert.Equal(0, (Int32)IoTErrorCode.None);

        Assert.Equal(1001, (Int32)IoTErrorCode.InvalidParameter);
        Assert.Equal(1002, (Int32)IoTErrorCode.DriverNotFound);
        Assert.Equal(1003, (Int32)IoTErrorCode.NotSupported);

        Assert.Equal(2001, (Int32)IoTErrorCode.ConnectionFailed);
        Assert.Equal(2002, (Int32)IoTErrorCode.Timeout);
        Assert.Equal(2006, (Int32)IoTErrorCode.ReadError);

        Assert.Equal(3001, (Int32)IoTErrorCode.DeviceOffline);
    }

    #endregion

    #region WriteRequest

    [Fact]
    public void WriteRequest_Constructors()
    {
        var point = new PointModel { Name = "p1", Address = "4x0001" };

        var r1 = new WriteRequest();
        Assert.Null(r1.Point);
        Assert.Null(r1.Value);

        var r2 = new WriteRequest(42);
        Assert.Null(r2.Point);
        Assert.Equal(42, r2.Value);

        var r3 = new WriteRequest(point, 99);
        Assert.Equal(point, r3.Point);
        Assert.Equal(99, r3.Value);
    }

    [Fact]
    public void WriteRequest_IExtend()
    {
        var req = new WriteRequest(new PointModel { Name = "p1" }, 100);

        req["QoS"] = 1;
        req["Priority"] = "high";

        Assert.Equal(1, req["QoS"]);
        Assert.Equal("high", req["Priority"]);
    }

    #endregion

    #region ControlRequest

    [Fact]
    public void ControlRequest_Create()
    {
        var parameters = new Dictionary<String, Object?> { ["Zero"] = 0.0, ["Span"] = 100.0 };
        var req = ControlRequest.Create("Calibrate", parameters);

        Assert.Equal("Calibrate", req.ServiceName);
        Assert.Equal(2, req.Parameters.Count);
        Assert.Equal(0.0, req.Parameters["Zero"]);
        Assert.Equal(100.0, req.Parameters["Span"]);
    }

    [Fact]
    public void ControlRequest_Create_NoParams()
    {
        var req = ControlRequest.Create("Reset");

        Assert.Equal("Reset", req.ServiceName);
        Assert.Empty(req.Parameters);
    }

    [Fact]
    public void ControlRequest_IExtend()
    {
        var req = ControlRequest.Create("Task");

        req["timeout"] = 5000;

        Assert.Equal(5000, req["timeout"]);
    }

    [Fact]
    public void ControlRequest_ToString()
    {
        var req = ControlRequest.Create("Calibrate", new Dictionary<String, Object?> { ["x"] = 1 });
        var s = req.ToString();

        Assert.Contains("Calibrate", s);
        Assert.Contains("1", s);
    }

    #endregion

    #region DriverAttribute

    [Fact]
    public void DriverAttribute_Creation()
    {
        var attr = new DriverAttribute("ModbusTcp");

        Assert.Equal("ModbusTcp", attr.Name);
    }

    #endregion

    #region INode / Node

    [Fact]
    public void Node_Default()
    {
        var node = new Node();

        Assert.True(node.IsConnected);
        Assert.Null(node.Device);
        Assert.Null(node.Parameter);
    }

    [Fact]
    public void Node_IsConnected_Changeable()
    {
        var node = new Node { IsConnected = false };

        Assert.False(node.IsConnected);
    }

    #endregion

    #region IDriverParameter

    [Fact]
    public void DriverParameter_GetKey()
    {
        var p = new DriverParameter();
        var key = p.GetKey();

        Assert.NotNull(key);
        Assert.Contains("DriverParameter", key);
    }

    #endregion

    #region DataQuality 枚举值

    [Fact]
    public void DataQuality_Values()
    {
        Assert.Equal(0, (Byte)DataQuality.Good);
        Assert.Equal(1, (Byte)DataQuality.Uncertain);
        Assert.Equal(2, (Byte)DataQuality.Bad);
        Assert.Equal(3, (Byte)DataQuality.BadNotConnected);
        Assert.Equal(4, (Byte)DataQuality.BadTimeout);
        Assert.Equal(5, (Byte)DataQuality.BadChecksumError);
        Assert.Equal(6, (Byte)DataQuality.BadProtocolError);
    }

    #endregion

    #region 边界条件

    [Fact]
    public void ReadResult_EmptyPoints()
    {
        var result = ReadResult.Success([], []);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Points);
        Assert.Empty(result.Values);
    }

    [Fact]
    public void WriteResult_ZeroBatch()
    {
        var result = WriteResult.SuccessBatch(0);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.AffectedCount);
    }

    [Fact]
    public void ControlResult_EmptyOutput()
    {
        var result = ControlResult.Success(null);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.OutputParameters);
        Assert.Empty(result.OutputParameters);
    }

    #endregion
}
