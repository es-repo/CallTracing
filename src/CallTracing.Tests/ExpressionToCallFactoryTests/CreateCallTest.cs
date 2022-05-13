using System.Collections;
using System.Linq.Expressions;
using Xunit;

namespace CallTracing.Tests.ExpressionToCallFactoryTests
{
    public static class CreateCallTest
    {
        public delegate int SomeDelegate(string s, double d);

        public interface ISomeInterface
        {
            int Func(string s, double d);

            int Property { get; set; }
        }

        public sealed record Args
        {
            public LambdaExpression LambdaExpression { get; init; } = null!;
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return DelegateCallExpression_DelegateCallExpected();
            }

            static object[] DelegateCallExpression_DelegateCallExpected()
            {
                Expression<Func<SomeDelegate, int>> lambdaExpression = someDelegate => someDelegate("a", 0.21);

                var args = new Args
                {
                    LambdaExpression = lambdaExpression
                };

                var expected = new DelegateCall(typeof(SomeDelegate), new object?[] { "a", 0.21 });

                return new object[] { args, expected };
            }

            static object[] MethodCallExpression_MethodCallExpressionExpected()
            {
                Expression<Func<ISomeInterface, int>> lambdaExpression = someInterface => someInterface.Func("a", 0.21);

                var args = new Args
                {
                    LambdaExpression = lambdaExpression
                };

                var expected = new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Func))!, new object?[] { "a", 0.21 });

                return new object[] { args, expected };
            }

            static object[] MemberExpression_PropertyCallExpected()
            {
                Expression<Func<ISomeInterface, int>> lambdaExpression = someInterface => someInterface.Property;

                var args = new Args
                {
                    LambdaExpression = lambdaExpression
                };

                var expected = new PropertyCall(typeof(ISomeInterface), typeof(ISomeInterface).GetProperty(nameof(ISomeInterface.Property))!);

                return new object[] { args, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            Args args,
            Call expected)
        {
            var actual = ExpressionToCallFactory.CreateCall(args.LambdaExpression);

            Assert.Equal(actual, expected);
        }
    }
}
