﻿using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace CallTracing.Tests.DelegateCallTests
{
    public static class EqualsTests
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
                // 1.
                yield return DelegateCall_OtherIsNull_FalseExpected();

                // 2.
                yield return DelegateCall_OtherIsEquivalent_TrueExpected();

                // 3.
                yield return DelegateCall_OtherHasDifferentType_FalseExpected();

                // 4.
                yield return DelegateCall_OtherHasMoreArgs_FalseExpected();
            }

            static object[] DelegateCall_OtherIsNull_FalseExpected()
            {
                var actualState = new DelegateCall(typeof(SomeDelegate1), new object?[] {});

                var args = new Args
                {
                    Other = null
                };

                var expected = false;

                return new object[] { actualState, args, expected };
            }

            static object[] DelegateCall_OtherIsEquivalent_TrueExpected()
            {
                var actualState = new DelegateCall(typeof(SomeDelegate1), new object?[] { null, 1, "abc" });

                var args = new Args
                {
                    Other = new DelegateCall(typeof(SomeDelegate1), new object?[] { null, 1, "abc" })
                };

                var expected = true;

                return new object[] { actualState, args, expected };
            }

            static object[] DelegateCall_OtherHasDifferentType_FalseExpected()
            {
                var actualState = new DelegateCall(typeof(SomeDelegate1), new object?[] { null, 1, "abc" });

                var args = new Args
                {
                    Other = new DelegateCall(typeof(SomeDelegate2), new object?[] { null, 1, "abc" })
                };

                var expected = false;

                return new object[] { actualState, args, expected };
            }

            static object[] DelegateCall_OtherHasMoreArgs_FalseExpected()
            {
                var actualState = new DelegateCall(typeof(SomeDelegate1), new object?[] { null, 1, "abc" });

                var args = new Args
                {
                    Other = new DelegateCall(typeof(SomeDelegate1), new object?[] { null, 1, "abc", null })
                };

                var expected = false;

                return new object[] { actualState, args, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(DelegateCall actualState, Args args, bool expected)
        {
            var actual = actualState.Equals(args.Other);

            Assert.Equal(expected, actual);
        }
    }
}
