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
                yield return CallTrace_OtherIsNull_FalseExpected_1();

                yield return EmptyCallTrace_OtherIsEmptyToo_TrueExpected_2();

                yield return CallTrace_OtherHasSameCalls_TrueExpected_3();

                yield return CallTrace_OtherHasOtherCalls_FalseExpected_4();
            }

            static object[] CallTrace_OtherIsNull_FalseExpected_1()
            {
                var stateActual = new CallTrace();

                var args = new Args
                {
                    Other = null
                };

                var expected = false;

                return new object[] { stateActual, args, expected };
            }

            static object[] EmptyCallTrace_OtherIsEmptyToo_TrueExpected_2()
            {
                var stateActual = new CallTrace();

                var args = new Args
                {
                    Other = new CallTrace()
                };

                var expected = true;

                return new object[] { stateActual, args, expected };
            }

            static object[] CallTrace_OtherHasSameCalls_TrueExpected_3()
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

            static object[] CallTrace_OtherHasOtherCalls_FalseExpected_4()
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
