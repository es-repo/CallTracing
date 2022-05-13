using System.Collections;
using System.Linq.Expressions;
using Xunit;

namespace CallTracing.Tests.ExpressionToCallFactoryTests
{
    public static class CreatePropertyCallTest
    {
        public interface ISomeInterface
        {
            int Property { get; set; }
        }

        public sealed record Args
        {
            public MemberExpression MemberExpression { get; init; } = null!;
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                // 1.
                yield return MemberExpression_PropertyCallExpected();
            }

            static object[] MemberExpression_PropertyCallExpected()
            {
                Expression<Func<ISomeInterface, int>> lambdaExpression = someInterface => someInterface.Property;
                var memberExpression = (MemberExpression)lambdaExpression.Body;

                var args = new Args
                {
                    MemberExpression = memberExpression
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
            PropertyCall expected)
        {
            var actual = ExpressionToCallFactory.CreatePropertyCall(args.MemberExpression);

            Assert.Equal(actual, expected);
        }
    }
}
