using System.Collections;
using Xunit;

namespace CallTracing.Tests.MethodCallTests
{
    public static class EqualsTest
    {
        public sealed record Args
        {
            public MethodCall? Other { get; init; }
        }

        public interface ISomeInterface1
        {
            void Action(string s);
            int Func(string s);
        }

        public interface ISomeInterface2
        {
            void Action(string s);
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                // 1.
                yield return MethodCall_OtherIsNull_FalseExpected();

                // 2.
                yield return MethodCall_OtherIsEquivalent_TrueExpected();

                // 3.
                yield return MethodCall_OtherHasDifferentType_FalseExpected();

                // 4.
                yield return MethodCall_OtherHasDifferentMethod_FalseExpected();

                // 5.
                yield return MethodCall_OtherHasMoreArgs_FalseExpected();
            }

            static object[] MethodCall_OtherIsNull_FalseExpected()
            {
                var stateActual = new MethodCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetMethod(nameof(ISomeInterface1.Action))!, new object?[] { "abc" });

                var args = new Args
                {
                    Other = null
                };

                var expected = false;

                return new object[] { stateActual, args, expected };
            }

            static object[] MethodCall_OtherIsEquivalent_TrueExpected()
            {
                var stateActual = new MethodCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetMethod(nameof(ISomeInterface1.Action))!, new object?[] { "abc" });

                var args = new Args
                {
                    Other = new MethodCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetMethod(nameof(ISomeInterface1.Action))!, new object?[] { "abc" })
                };

                var expected = true;

                return new object[] { stateActual, args, expected };
            }

            static object[] MethodCall_OtherHasDifferentType_FalseExpected()
            {
                var stateActual = new MethodCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetMethod(nameof(ISomeInterface1.Action))!, new object?[] { "abc" });

                var args = new Args
                {
                    Other = new MethodCall(typeof(ISomeInterface2), typeof(ISomeInterface2).GetMethod(nameof(ISomeInterface2.Action))!, new object?[] { "abc" })
                };

                var expected = false;

                return new object[] { stateActual, args, expected };
            }

            static object[] MethodCall_OtherHasDifferentMethod_FalseExpected()
            {
                var stateActual = new MethodCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetMethod(nameof(ISomeInterface1.Action))!, new object?[] { "abc" });

                var args = new Args
                {
                    Other = new MethodCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetMethod(nameof(ISomeInterface1.Func))!, new object?[] { "abc" })
                };

                var expected = false;

                return new object[] { stateActual, args, expected };
            }

            static object[] MethodCall_OtherHasMoreArgs_FalseExpected()
            {
                var stateActual = new MethodCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetMethod(nameof(ISomeInterface1.Action))!, new object?[] { "abc" });

                var args = new Args
                {
                    Other = new MethodCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetMethod(nameof(ISomeInterface1.Func))!, new object?[] { "abc", null })
                };

                var expected = false;

                return new object[] { stateActual, args, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            MethodCall stateActual,
            Args args,
            bool expected)
        {
            var actual = stateActual.Equals(args.Other);

            Assert.Equal(expected, actual);
        }
    }
}
