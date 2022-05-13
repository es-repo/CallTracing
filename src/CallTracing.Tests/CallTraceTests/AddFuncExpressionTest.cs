using System.Collections;
using System.Linq.Expressions;
using Xunit;

namespace CallTracing.Tests.CallTraceTests
{
    public static class AddFuncExpressionTest
    {
        public interface ISomeInterface
        {
            int Func(string value);

            int Property { get; set; }
        }

        public record Args
        {
            public Expression<Func<ISomeInterface, int>> LambdaExpression { get; init; } = null!;
        }

        class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                // 1. 
                yield return EmptyCallTrace_FuncExpressionUseMethod_CallAddedToCallTrace();

                // 2. 
                yield return NonEmptyCallTrace_FuncExpressionUseProperty_CallAddedToCallTrace();
            }

            static object[] EmptyCallTrace_FuncExpressionUseMethod_CallAddedToCallTrace()
            {
                var stateActual = new CallTrace();

                Expression<Func<ISomeInterface, int>> lambdaExpression = (ISomeInterface someInterface) => someInterface.Func("abc");

                var args = new Args
                {
                    LambdaExpression = lambdaExpression
                };

                var stateExpected = new CallTrace(new Call[]
                {
                    new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Func))!, new object?[] { "abc" })
                });

                return new object[] { stateActual, args, stateExpected };
            }

            static object[] NonEmptyCallTrace_FuncExpressionUseProperty_CallAddedToCallTrace()
            {
                var stateActual = new CallTrace(new Call[]
                {
                    new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Func))!, new object?[] { "abc" })
                });

                Expression<Func<ISomeInterface, int>> lambdaExpression = (ISomeInterface someInterface) => someInterface.Property;

                var args = new Args
                {
                    LambdaExpression = lambdaExpression
                };

                var stateExpected = new CallTrace(new Call[]
                {
                    new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Func))!, new object?[] { "abc" }),
                    new PropertyCall(typeof(ISomeInterface), typeof(ISomeInterface).GetProperty(nameof(ISomeInterface.Property))!)
                });

                return new object[] { stateActual, args, stateExpected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            CallTrace stateActual,
            Args args,
            CallTrace stateExpected)
        {
            stateActual.Add(args.LambdaExpression);

            Assert.Equal(stateExpected, stateActual);
        }
    }
}
