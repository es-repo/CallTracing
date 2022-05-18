using System.Collections;
using Xunit;

namespace CallTracing.Tests.DelegateCallTests
{
    public static class ToStringTest
    {
        public delegate int SomeDelegate(object? o, int i, string s);

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return DelegateCall_StringExpected();
            }

            static object[] DelegateCall_StringExpected()
            {
                var stateActual = new DelegateCall(typeof(SomeDelegate), new object?[] { null, 1, "abc" });

                var expected = "DelegateCall { Type = CallTracing.Tests.DelegateCallTests.ToStringTest+SomeDelegate, Args = [null, 1, abc] }";

                return new object[] { stateActual, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            DelegateCall stateActual,
            string expected)
        {
            var actual = stateActual.ToString();

            Assert.Equal(expected, actual);
        }
    }
}
