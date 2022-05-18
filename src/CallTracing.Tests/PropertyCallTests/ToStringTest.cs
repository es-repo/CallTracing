using System.Collections;
using Xunit;

namespace CallTracing.Tests.PropertyCallTests
{
    public static class ToStringTest
    {
        public interface ISomeInterface
        {
            int Property1 { get; set; }
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return PropertyCall_StringExpected();
            }

            static object[] PropertyCall_StringExpected()
            {
                var stateActual = new PropertyCall(typeof(ISomeInterface), typeof(ISomeInterface).GetProperty(nameof(ISomeInterface.Property1))!);

                var expected = "PropertyCall { Type = CallTracing.Tests.PropertyCallTests.ToStringTest+ISomeInterface, Property = Int32 Property1 }";

                return new object[] { stateActual, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            PropertyCall stateActual,
            string expected)
        {
            var actual = stateActual.ToString();

            Assert.Equal(expected, actual);
        }
    }
}
