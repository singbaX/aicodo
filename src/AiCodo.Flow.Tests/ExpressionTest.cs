using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCodo.Flow.Tests
{ 
    public class ExpressionTest
    {
        [Test]
        public void TestObjectExpression()
        {
            var script = @"s.ToDecimal()";
            var value = script.Eval("s", "12.456");
            Assert.AreEqual(value, 12.456);

            script = @"s.IsNullOrEmpty()";
            value = script.Eval("s", "12");
            Assert.IsFalse(value.ToBoolean());
        }
    }
}
