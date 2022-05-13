using System.Collections;
using System.Linq.Expressions;
using Xunit;

namespace CallTracing.Tests.CallTraceTests
{
    public static class AddActionExpressionTest
    {
        public interface ISomeInterface
        {
            void Action1(string value);

            void Action2(int value);
        }

        public record Args
        {
            public Expression<Action<ISomeInterface>> LambdaExpression { get; init; } = null!;
        }

        class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                // 1. 
                yield return EmptyCallTrace_ActionExpression_CallAddedToCallTrace();

                // 2. 
                yield return NonEmptyCallTrace_ActionExpression_CallAddedToCallTrace();
            }

            static object[] EmptyCallTrace_ActionExpression_CallAddedToCallTrace()
            {
                var stateActual = new CallTrace();

                Expression<Action<ISomeInterface>> lambdaExpression = (ISomeInterface someInterface) => someInterface.Action1("abc");

                var args = new Args
                {
                    LambdaExpression = lambdaExpression
                };

                var stateExpected = new CallTrace(new Call[]
                {
                    new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Action1))!, new object?[] { "abc" })
                });

                return new object[] { stateActual, args, stateExpected };
            }

            static object[] NonEmptyCallTrace_ActionExpression_CallAddedToCallTrace()
            {
                var stateActual = new CallTrace(new Call[]
                {
                    new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Action1))!, new object?[] { "abc" })
                });

                Expression<Action<ISomeInterface>> lambdaExpression = (ISomeInterface someInterface) => someInterface.Action2(1);

                var args = new Args
                {
                    LambdaExpression = lambdaExpression
                };

                var stateExpected = new CallTrace(new Call[]
                {
                    new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Action1))!, new object?[] { "abc" }),
                    new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Action2))!, new object?[] { 1 })
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
