using System.Collections;
using Xunit;

namespace CallTracing.Tests.MethodCallTests
{
    public static class ToStringTest
    {
        public interface ISomeInterface
        {
            void Action(string s);
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                // 1.
                yield return TestCase1();
            }

            static object[] TestCase1()
            {
                var stateActual = new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Action))!, new object?[] { "abc" });

                var expected = "MethodCall { Type = CallTracing.Tests.MethodCallTests.ToStringTest+ISomeInterface, Method = Void Action(System.String), Args = [abc] }";

                return new object[] { stateActual, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            MethodCall stateActual,
            string expected)
        {
            var actual = stateActual.ToString();

            Assert.Equal(expected, actual);
        }
    }
}
