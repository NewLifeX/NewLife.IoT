using System;
using System.Threading.Tasks;
using Moq;
using NewLife.IoT;
using NewLife.IoT.ThingModels;
using Xunit;

namespace XUnitTest;

public class ThingExtensionsTests
{
    private readonly Mock<IDevice> _mockDevice;

    public ThingExtensionsTests()
    {
        _mockDevice = new Mock<IDevice>();
    }

    [Fact]
    public void WriteInfoEvent_CallsWriteEventWithInfo()
    {
        String? capturedType = null, capturedName = null, capturedRemark = null;
        _mockDevice.Setup(d => d.WriteEvent(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()))
                   .Callback<String, String, String>((t, n, r) => { capturedType = t; capturedName = n; capturedRemark = r; })
                   .Returns(true);

        _mockDevice.Object.WriteInfoEvent("DeviceOnline", "connected");

        Assert.Equal("info", capturedType);
        Assert.Equal("DeviceOnline", capturedName);
        Assert.Equal("connected", capturedRemark);
    }

    [Fact]
    public void WriteAlertEvent_CallsWriteEventWithAlert()
    {
        String? capturedType = null;
        _mockDevice.Setup(d => d.WriteEvent(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()))
                   .Callback<String, String, String>((t, n, r) => { capturedType = t; })
                   .Returns(true);

        _mockDevice.Object.WriteAlertEvent("HighTemp", "over 80C");

        Assert.Equal("alert", capturedType);
    }

    [Fact]
    public void WriteErrorEvent_CallsWriteEventWithError()
    {
        String? capturedType = null, capturedName = null, capturedRemark = null;
        _mockDevice.Setup(d => d.WriteEvent(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()))
                   .Callback<String, String, String>((t, n, r) => { capturedType = t; capturedName = n; capturedRemark = r; })
                   .Returns(true);

        _mockDevice.Object.WriteErrorEvent("ConnectionLost", "timeout");

        Assert.Equal("error", capturedType);
        Assert.Equal("ConnectionLost", capturedName);
        Assert.Equal("timeout", capturedRemark);
    }
}
