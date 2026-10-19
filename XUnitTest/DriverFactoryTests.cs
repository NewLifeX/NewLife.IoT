using System;
using NewLife.IoT;
using NewLife.IoT.Drivers;
using NewLife.IoT.ThingSpecification;
using Xunit;

namespace XUnitTest;

public class DriverFactoryTests
{
    [Driver("TestDriver")]
    private class TestDriver : DriverBase<TestNode, TestParameter>
    {
        protected override Boolean OnGetSpecification(ThingSpec thingSpec)
        {
            thingSpec.Properties = [PropertySpec.Create("Value", "test", "int")];
            return true;
        }
    }

    private class TestNode : Node
    {
        public Int32 StationId { get; set; }
    }

    private class TestParameter : IDriverParameter
    {
        public String? Address { get; set; }
        public Int32 Timeout { get; set; } = 3000;
    }

    [Fact]
    public void Register_ByNameAndType()
    {
        var name = "MyTestDriver";
        DriverFactory.Register(name, typeof(TestDriver));
        Assert.True(DriverFactory.Map.ContainsKey(name));
        Assert.Equal(typeof(TestDriver), DriverFactory.Map[name]);
    }

    [Fact]
    public void Register_Generic()
    {
        var name = "GenericTestDriver";
        DriverFactory.Register<TestDriver>(name);
        Assert.True(DriverFactory.Map.ContainsKey(name));
    }

    [Fact]
    public void Register_NullName_UsesTypeName()
    {
        DriverFactory.Register(null, typeof(TestDriver));
        Assert.True(DriverFactory.Map.ContainsKey("TestDriver"));
    }

    [Fact]
    public void Create_TypeRegistration_UsesFactory()
    {
        var name = "TypeRegistrationDriver";
        DriverFactory.Register(name, typeof(TestDriver));

        var driver = DriverFactory.Create(name, "");
        Assert.IsType<TestDriver>(driver);
    }

    [Fact]
    public void Register_NullType_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => DriverFactory.Register("test", null!));
    }

    [Fact]
    public void Register_WithFactory_UsesFactory()
    {
        var name = "FactoryTestDriver";
        var count = 0;
        DriverFactory.Register(name, typeof(TestDriver), () =>
        {
            count++;
            return new TestDriver();
        });

        var d1 = DriverFactory.Create(name, "");
        var d2 = DriverFactory.Create(name, "");
        Assert.IsType<TestDriver>(d1);
        Assert.IsType<TestDriver>(d2);
        Assert.NotSame(d1, d2);
        Assert.Equal(2, count);

        var d3 = DriverFactory.Create(name, "FACTORY-ID-1");
        var d4 = DriverFactory.Create(name, "FACTORY-ID-1");
        Assert.Same(d3, d4);
        Assert.Equal(3, count);
    }

    [Fact]
    public void Register_WithFactory_NullArguments_Throw()
    {
        Assert.Throws<ArgumentNullException>(() => DriverFactory.Register("FactoryNullType", null!, () => new TestDriver()));
        Assert.Throws<ArgumentNullException>(() => DriverFactory.Register("FactoryNullFunc", typeof(TestDriver), null!));
    }

    [Fact]
    public void Create_NoIdentifier_ReturnsNewInstance()
    {
        var name = "NoIdTestDriver";
        DriverFactory.Register<TestDriver>(name);
        var d1 = DriverFactory.Create(name, "");
        var d2 = DriverFactory.Create(name, "");
        Assert.NotNull(d1);
        Assert.NotNull(d2);
        Assert.NotSame(d1, d2);
    }

    [Fact]
    public void Create_SameIdentifier_ReturnsSingleton()
    {
        var name = "SingletonTestDriver";
        DriverFactory.Register<TestDriver>(name);
        var d1 = DriverFactory.Create(name, "COM1");
        var d2 = DriverFactory.Create(name, "COM1");
        Assert.NotNull(d1);
        Assert.NotNull(d2);
        Assert.Same(d1, d2);
    }

    [Fact]
    public void Create_DifferentIdentifier_ReturnsDifferentInstances()
    {
        var name = "MultiTestDriver";
        DriverFactory.Register<TestDriver>(name);
        var d1 = DriverFactory.Create(name, "COM1");
        var d2 = DriverFactory.Create(name, "COM2");
        Assert.NotNull(d1);
        Assert.NotNull(d2);
        Assert.NotSame(d1, d2);
    }

    [Fact]
    public void Create_UnregisteredName_ReturnsNull()
    {
        var driver = DriverFactory.Create("NonExistentDriver", "test");
        Assert.Null(driver);
    }

    [Fact]
    public void CreateParameter_CreatesAndParses()
    {
        var name = "ParamTestDriver";
        DriverFactory.Register<TestDriver>(name);
        var p = DriverFactory.CreateParameter(name, "");
        Assert.NotNull(p);
        Assert.IsType<TestParameter>(p);
    }

    [Fact]
    public void CreateParameter_Unregistered_ReturnsNull()
    {
        var p = DriverFactory.CreateParameter("NonExistent", "");
        Assert.Null(p);
    }
}
