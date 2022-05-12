using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace CallTracing.Tests.DelegateCallTests
{
    public static class ToStringTests
    {
        public delegate int SomeDelegate(object? o, int i, string s);

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                // 1.
                yield return TestCase1();
            }

            static object[] TestCase1()
            {
                var actualState = new DelegateCall(typeof(SomeDelegate), new object?[] { null, 1, "abc" });

                var expected = "DelegateCall { Type = CallTracing.Tests.DelegateCallTests.ToStringTests+SomeDelegate, Args = [null, 1, abc] }";

                return new object[] { actualState, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(DelegateCall actualState, string expected)
        {
            var actual = actualState.ToString();

            Assert.Equal(expected, actual);
        }
    }
}
