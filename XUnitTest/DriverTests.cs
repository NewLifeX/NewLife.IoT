using NewLife.PC.Drivers;
using Xunit;

namespace XUnitTest;

public class DriverTests
{
    [Fact]
    public void Test1()
    {
        var driver = new PCDriver();
        var spec = driver.GetSpecification();
        Assert.NotNull(spec);
        Assert.NotNull(spec.Profile);
        Assert.NotNull(spec.Properties);
        Assert.Null(spec.Events);
        Assert.NotNull(spec.Services);
        Assert.Null(spec.ExtendedProperties);

        Assert.Equal("PC", spec.Profile.ProductKey);
        Assert.Equal(7, spec.Properties.Length);
        Assert.Equal(2, spec.Services.Length);

        var pi = spec.Properties[0];
        Assert.NotEmpty(pi.Id);
        Assert.NotEmpty(pi.Name);

        var tsl = spec.ToJson();
        Assert.NotEmpty(tsl);

        var node = driver.Open(null, null);
        Assert.NotNull(node);

        var spec2 = driver.GetSpecification();
        Assert.NotNull(spec2);

        var tsl2 = spec2.ToJson();
        Assert.NotEmpty(tsl2);

        Assert.Equal(tsl, tsl2);
    }
}