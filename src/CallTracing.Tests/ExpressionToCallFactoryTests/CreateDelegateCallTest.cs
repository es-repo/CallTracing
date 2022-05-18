using System.Collections;
using System.Linq.Expressions;
using Xunit;

namespace CallTracing.Tests.ExpressionToCallFactoryTests
{
    public static class CreateDelegateCallTest
    {
        public delegate int SomeDelegate(string s, double d);

        public sealed record Args
        {
            public InvocationExpression InvocationExpression { get; init; } = null!;
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return InvocationExpressionWithInlineParams_DelegateCallExpected_1();

                yield return InvocationExpressionWithCapturedValuesParams_DelegateCallExpected_2();
            }

            static object[] InvocationExpressionWithInlineParams_DelegateCallExpected_1()
            {
                Expression<Func<SomeDelegate, int>> lambdaExpression = someDelegate => someDelegate("a", 0.21);
                var invocationExpression = (InvocationExpression)lambdaExpression.Body;

                var args = new Args
                {
                    InvocationExpression = invocationExpression
                };

                var expected = new DelegateCall(typeof(SomeDelegate), new object?[] { "a", 0.21 });

                return new object[] { args, expected };
            }

            static object[] InvocationExpressionWithCapturedValuesParams_DelegateCallExpected_2()
            {
                var s = "a";
                var d = 0.21;

                Expression<Func<SomeDelegate, int>> lambdaExpression = someDelegate => someDelegate(s, d);
                var invocationExpression = (InvocationExpression)lambdaExpression.Body;

                var args = new Args
                {
                    InvocationExpression = invocationExpression
                };

                var expected = new DelegateCall(typeof(SomeDelegate), new object?[] { s, d });

                return new object[] { args, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            Args args,
            DelegateCall expected)
        {
            var actual = ExpressionToCallFactory.CreateDelegateCall(args.InvocationExpression);

            Assert.Equal(actual, expected);
        }
    }
}
