using System.Collections;
using System.Linq.Expressions;
using Xunit;

namespace CallTracing.Tests.ExpressionToCallFactoryTests
{
    public static class CreateMethodCallTest
    {
        public interface ISomeInterface
        {
            int Func(string s, double d);
        }

        public sealed record Args
        {
            public MethodCallExpression MethodCallExpression { get; init; } = null!;
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return MethodCallExpression_MethodCallExpressionExpected_1();
            }

            static object[] MethodCallExpression_MethodCallExpressionExpected_1()
            {
                Expression<Func<ISomeInterface, int>> lambdaExpression = someInterface => someInterface.Func("a", 0.21);
                var methodCallExpression = (MethodCallExpression)lambdaExpression.Body;

                var args = new Args
                {
                    MethodCallExpression = methodCallExpression
                };

                var expected = new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Func))!, new object?[] { "a", 0.21 });

                return new object[] { args, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            Args args,
            MethodCall expected)
        {
            var actual = ExpressionToCallFactory.CreateMethodCall(args.MethodCallExpression);

            Assert.Equal(actual, expected);
        }
    }
}
