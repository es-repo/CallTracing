using System.Collections;
using Xunit;

namespace CallTracing.Tests.DelegateCallTests
{
    public static class EqualsTest
    {
        public sealed record Args
        {
            public DelegateCall? Other { get; init; }
        }

        public delegate int SomeDelegate1(object? o, int i, string s);
        public delegate int SomeDelegate2(object? o, int i, string s);

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return DelegateCall_OtherIsNull_FalseExpected_1();

                yield return DelegateCall_OtherIsEquivalent_TrueExpected_2();

                yield return DelegateCall_OtherHasDifferentType_FalseExpected_3();

                yield return DelegateCall_OtherHasMoreArgs_FalseExpected_4();
            }

            static object[] DelegateCall_OtherIsNull_FalseExpected_1()
            {
                var stateActual = new DelegateCall(typeof(SomeDelegate1), new object?[] { });

                var args = new Args
                {
                    Other = null
                };

                var expected = false;

                return new object[] { stateActual, args, expected };
            }

            static object[] DelegateCall_OtherIsEquivalent_TrueExpected_2()
            {
                var stateActual = new DelegateCall(typeof(SomeDelegate1), new object?[] { null, 1, "abc" });

                var args = new Args
                {
                    Other = new DelegateCall(typeof(SomeDelegate1), new object?[] { null, 1, "abc" })
                };

                var expected = true;

                return new object[] { stateActual, args, expected };
            }

            static object[] DelegateCall_OtherHasDifferentType_FalseExpected_3()
            {
                var stateActual = new DelegateCall(typeof(SomeDelegate1), new object?[] { null, 1, "abc" });

                var args = new Args
                {
                    Other = new DelegateCall(typeof(SomeDelegate2), new object?[] { null, 1, "abc" })
                };

                var expected = false;

                return new object[] { stateActual, args, expected };
            }

            static object[] DelegateCall_OtherHasMoreArgs_FalseExpected_4()
            {
                var stateActual = new DelegateCall(typeof(SomeDelegate1), new object?[] { null, 1, "abc" });

                var args = new Args
                {
                    Other = new DelegateCall(typeof(SomeDelegate1), new object?[] { null, 1, "abc", null })
                };

                var expected = false;

                return new object[] { stateActual, args, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            DelegateCall stateActual,
            Args args,
            bool expected)
        {
            var actual = stateActual.Equals(args.Other);

            Assert.Equal(expected, actual);
        }
    }
}
