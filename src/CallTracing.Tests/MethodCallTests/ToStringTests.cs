using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace CallTracing.Tests.MethodCallTests
{
    public static class ToStringTests
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
                var actualState = new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Action))!, new object?[] { "abc" });

                var expected = "MethodCall { Type = CallTracing.Tests.MethodCallTests.ToStringTests+ISomeInterface, Method = Void Action(System.String), Args = [abc] }";

                return new object[] { actualState, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(MethodCall actualState, string expected)
        {
            var actual = actualState.ToString();

            Assert.Equal(expected, actual);
        }
    }
}
