using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace CallTracing.Tests.PropertyCallTests
{
    public static class ToStringTests
    {
        public interface ISomeInterface
        {
            int Property1 { get; set; }
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
                var actualState = new PropertyCall(typeof(ISomeInterface), typeof(ISomeInterface).GetProperty(nameof(ISomeInterface.Property1))!);

                var expected = "PropertyCall { Type = CallTracing.Tests.PropertyCallTests.ToStringTests+ISomeInterface, Property = Int32 Property1 }";

                return new object[] { actualState, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(PropertyCall actualState, string expected)
        {
            var actual = actualState.ToString();

            Assert.Equal(expected, actual);
        }
    }
}
