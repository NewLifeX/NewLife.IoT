using System;
using System.Collections.Generic;
using NewLife.IoT.Features;
using Xunit;

namespace XUnitTest;

public class ExpressionEngineTests
{
    [Fact]
    public void ExpressionEngine_ParseAndInvoke()
    {
        var engine = new TestExpressionEngine();
        
        engine.Parse("x + 1", new Dictionary<String, Type> { ["x"] = typeof(Int32) });
        Assert.True(engine.Parsed);

        var result = engine.Invoke(new Dictionary<String, Object> { ["x"] = 21 });
        Assert.Equal(42, result);
    }

    [Fact]
    public void ExpressionEngine_UpdateTime()
    {
        var engine = new TestExpressionEngine();
        var now = DateTime.UtcNow;
        engine.UpdateTime = now;
        Assert.Equal(now, engine.UpdateTime);
    }

    private class TestExpressionEngine : IExpressionEngine
    {
        public DateTime UpdateTime { get; set; }
        public Boolean Parsed { get; private set; }

        public Object Parse(String expression, IDictionary<String, Type> parameters)
        {
            Parsed = true;
            return new Object();
        }

        public Object Invoke(IDictionary<String, Object> arguments)
        {
            return 42;
        }
    }
}
