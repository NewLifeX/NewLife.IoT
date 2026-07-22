using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NewLife.IoT;
using NewLife.IoT.Drivers;
using NewLife.IoT.ThingModels;
using Xunit;

namespace XUnitTest;

public class DriverExtensionsTests
{
    private readonly Mock<IDriver> _mockDriver;
    private readonly Mock<IDevice> _mockDevice;
    private readonly Node _node;
    private readonly IPoint[] _points;

    public DriverExtensionsTests()
    {
        _mockDriver = new Mock<IDriver>();
        _mockDevice = new Mock<IDevice>();
        _node = new Node { Driver = _mockDriver.Object };
        _points = [new PointModel { Name = "Test", Type = "int" }];
    }

    [Fact]
    public void Read_Sync_CallsReadAsync()
    {
        var expected = ReadResult.Success(_points, [42]);
        _mockDriver.Setup(d => d.ReadAsync(It.IsAny<INode>(), It.IsAny<IPoint[]>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expected);

        var result = _mockDriver.Object.Read(_node, _points);

        Assert.Same(expected, result);
        _mockDriver.Verify(d => d.ReadAsync(_node, _points, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Write_Sync_CallsWriteAsync()
    {
        var expected = WriteResult.Success(echoValue: 99);
        _mockDriver.Setup(d => d.WriteAsync(It.IsAny<INode>(), It.IsAny<WriteRequest[]>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expected);

        var result = _mockDriver.Object.Write(_node, _points[0], 42);

        Assert.Equal(expected, result);
        _mockDriver.Verify(
            d => d.WriteAsync(_node, It.Is<WriteRequest[]>(r => r.Length == 1 && r[0].Value!.Equals(42)), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Close_Sync_CallsCloseAsync()
    {
        _mockDriver.Setup(d => d.CloseAsync(It.IsAny<INode>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);

        _mockDriver.Object.Close(_node);

        _mockDriver.Verify(d => d.CloseAsync(_node, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WriteAsync_SinglePoint_WrapsToArray()
    {
        var expected = WriteResult.Success();
        _mockDriver.Setup(d => d.WriteAsync(It.IsAny<INode>(), It.IsAny<WriteRequest[]>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expected);

        var request = new WriteRequest(_points[0], 100);
        var result = await _mockDriver.Object.WriteAsync(_node, request);

        Assert.Equal(expected, result);
        _mockDriver.Verify(
            d => d.WriteAsync(_node, It.Is<WriteRequest[]>(r => r.Length == 1 && r[0] == request), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
