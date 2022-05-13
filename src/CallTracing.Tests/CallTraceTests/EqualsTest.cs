using System.Collections;
using Xunit;

namespace CallTracing.Tests.CallTraceTests
{
    public static class EqualsTest
    {
        public interface ISomeInterface
        {
            int Func(string s);

            int Property { get; set; }
        }

        public delegate int SomeDelegate(string s);

        public sealed record Args
        {
            public CallTrace? Other { get; init; }
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                // 1.
                yield return CallTrace_OtherIsNull_FalseExpected();

                // 2.
                yield return EmptyCallTrace_OtherIsEmptyToo_TrueExpected();

                // 3.
                yield return CallTrace_OtherHasSameCalls_TrueExpected();

                // 4.
                yield return CallTrace_OtherHasOtherCalls_FalseExpected();
            }

            static object[] CallTrace_OtherIsNull_FalseExpected()
            {
                var stateActual = new CallTrace();

                var args = new Args
                {
                    Other = null
                };

                var expected = false;

                return new object[] { stateActual, args, expected };
            }

            static object[] EmptyCallTrace_OtherIsEmptyToo_TrueExpected()
            {
                var stateActual = new CallTrace();

                var args = new Args
                {
                    Other = new CallTrace()
                };

                var expected = true;

                return new object[] { stateActual, args, expected };
            }

            static object[] CallTrace_OtherHasSameCalls_TrueExpected()
            {
                var stateActual = new CallTrace(
                    new Call[]
                    {
                        new DelegateCall(typeof(SomeDelegate), new object?[] { "a" }),
                        new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Func))!, new object?[] { "a" }),
                        new PropertyCall(typeof(SomeDelegate), typeof(ISomeInterface).GetProperty(nameof(ISomeInterface.Property))!),
                    });

                var args = new Args
                {
                    Other = new CallTrace(new Call[]
                    {
                        new DelegateCall(typeof(SomeDelegate), new object?[] { "a" }),
                        new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Func))!, new object?[] { "a" }),
                        new PropertyCall(typeof(SomeDelegate), typeof(ISomeInterface).GetProperty(nameof(ISomeInterface.Property))!),
                    })
                };

                var expected = true;

                return new object[] { stateActual, args, expected };
            }

            static object[] CallTrace_OtherHasOtherCalls_FalseExpected()
            {
                var stateActual = new CallTrace(
                    new Call[]
                    {
                        new DelegateCall(typeof(SomeDelegate), new object?[] { "a" }),
                        new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Func))!, new object?[] { "a", "b" }),
                        new PropertyCall(typeof(SomeDelegate), typeof(ISomeInterface).GetProperty(nameof(ISomeInterface.Property))!),
                    });

                var args = new Args
                {
                    Other = new CallTrace(new Call[]
                    {
                        new DelegateCall(typeof(SomeDelegate), new object?[] { "a" }),
                        new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Func))!, new object?[] { "a" }),
                        new PropertyCall(typeof(SomeDelegate), typeof(ISomeInterface).GetProperty(nameof(ISomeInterface.Property))!),
                    })
                };

                var expected = false;

                return new object[] { stateActual, args, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            CallTrace stateActual,
            Args args,
            bool expected)
        {
            var actual = stateActual.Equals(args.Other);

            Assert.Equal(expected, actual);
        }
    }
}
