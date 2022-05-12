﻿using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace CallTracing.Tests.PropertyCallTests
{
    public static class EqualsTests
    {
        public sealed record Args
        {
            public PropertyCall? Other { get; init; }
        }

        public interface ISomeInterface1
        {
            int Property1 { get; set; }

            int Property2 { get; set; }
        }

        public interface ISomeInterface2
        {
            int Property1 { get; set; }
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                // 1.
                yield return PropertyCall_OtherIsNull_FalseExpected();

                // 2.
                yield return PropertyCall_OtherIsEquivalent_TrueExpected();

                // 3.
                yield return PropertyCall_OtherHasDifferentType_FalseExpected();

                // 4.
                yield return PropertyCall_OtherHasDifferentProperty_FalseExpected();
            }

            static object[] PropertyCall_OtherIsNull_FalseExpected()
            {
                var actualState = new PropertyCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetProperty(nameof(ISomeInterface1.Property1))!);

                var args = new Args
                {
                    Other = null
                };

                var expected = false;

                return new object[] { actualState, args, expected };
            }

            static object[] PropertyCall_OtherIsEquivalent_TrueExpected()
            {
                var actualState = new PropertyCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetProperty(nameof(ISomeInterface1.Property1))!);

                var args = new Args
                {
                    Other = new PropertyCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetProperty(nameof(ISomeInterface1.Property1))!)
                };

                var expected = true;

                return new object[] { actualState, args, expected };
            }

            static object[] PropertyCall_OtherHasDifferentType_FalseExpected()
            {
                var actualState = new PropertyCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetProperty(nameof(ISomeInterface1.Property1))!);

                var args = new Args
                {
                    Other = new PropertyCall(typeof(ISomeInterface1), typeof(ISomeInterface2).GetProperty(nameof(ISomeInterface2.Property1))!)
                };

                var expected = false;

                return new object[] { actualState, args, expected };
            }

            static object[] PropertyCall_OtherHasDifferentProperty_FalseExpected()
            {
                var actualState = new PropertyCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetProperty(nameof(ISomeInterface1.Property1))!);

                var args = new Args
                {
                    Other = new PropertyCall(typeof(ISomeInterface1), typeof(ISomeInterface1).GetProperty(nameof(ISomeInterface1.Property2))!)
                };

                var expected = false;

                return new object[] { actualState, args, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(PropertyCall actualState, Args args, bool expected)
        {
            var actual = actualState.Equals(args.Other);

            Assert.Equal(expected, actual);
        }
    }
}
