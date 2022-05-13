//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using Xunit;

//namespace CallTracing.Tests.CallTraceTests
//{
//    public static class AddLambdaActionBodyTests
//    {
//        public interface ISomeInterface
//        {
//            void Action(string value);

//            void Action2(int value);

//            int Property { get; }
//        }

//        public record Args
//        {
//            public Expression<Action<ISomeInterface>> LambdaExpression { get; init; } = null!;
//        }

//        class TestCases : IEnumerable<object[]>
//        {
//            public IEnumerator<object[]> GetEnumerator()
//            {
//                // 1. 
//                yield return EmptyCallTrace_LambdaWithActionBody_CallAddedToCallTrace();

//                // 2. 
//                yield return NonEmptyCallTrace_LambdaWithActionBody_CallAddedToCallTrace();

//                // 3. 
//                //yield return NonEmptyCallTrace_LambdaWithPropertyBody_CallAddedToCallTrace();
//            }

//            static object[] EmptyCallTrace_LambdaWithActionBody_CallAddedToCallTrace()
//            {
//                var stateActual = new CallTrace();

//                Expression<Action<ISomeInterface>> lambdaExpression = (ISomeInterface someInterface) => someInterface.Action("abc");

//                var args = new Args
//                {
//                    LambdaExpression = lambdaExpression
//                };

//                var expectedState = new CallTrace(new MockCallTrace[]
//                {
//                    new MockCallTrace
//                    {
//                        Type = typeof(ISomeInterface),
//                        Args = new List<object?> { "abc" },
//                        Method = typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Action))
//                    }
//                });

//                return new object[] { stateActual, args, expectedState };
//            }

//            static object[] EmptyCallTrace_LambdaWithActionBody_CallAddedToCallTrace2()
//            {
//                var stateActual = new CallTrace();

//                Expression<Action<ISomeInterface>> lambdaExpression = (ISomeInterface someInterface) => someInterface.Action("abc");

//                var args = new Args
//                {
//                    LambdaExpression = lambdaExpression
//                };

//                var expectedState = new CallTrace(new MockCallTrace[]
//                {
//                    new MockCallTrace
//                    {
//                        Type = typeof(ISomeInterface),
//                        Args = new List<object?> { "abc" },
//                        Method = typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Action))
//                    }
//                });

//                return new object[] { stateActual, args, expectedState };
//            }

//            static object[] NonEmptyCallTrace_LambdaWithActionBody_CallAddedToCallTrace()
//            {
//                var stateActual = new CallTrace(new MockCallTrace[]
//                {
//                    new MockCallTrace
//                    {
//                        Type = typeof(ISomeInterface),
//                        Args = new List<object?> { "abc" },
//                        Method = typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Action))
//                    }
//                });

//                Expression<Action<ISomeInterface>> lambdaExpression = (ISomeInterface someInterface) => someInterface.Action2(123);

//                var args = new Args
//                {
//                    LambdaExpression = lambdaExpression
//                };

//                var expectedState = new CallTrace(new MockCallTrace[]
//                {
//                    new MockCallTrace
//                    {
//                        Type = typeof(ISomeInterface),
//                        Args = new List<object?> { "abc" },
//                        Method = typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Action))
//                    },

//                    new MockCallTrace
//                    {
//                        Type = typeof(ISomeInterface),
//                        Args = new List<object?> { 123 },
//                        Method = typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Action2))
//                    },
//                });

//                return new object[] { stateActual, args, expectedState };
//            }

//            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//        }

//        [Theory]
//        [ClassData(typeof(TestCases))]
//        public static void Test(CallTrace stateActual, Args args, CallTrace expectedState)
//        {
//            stateActual.Add(args.LambdaExpression);

//            Assert.Equal(expectedState, stateActual);
//        }
//    }
//}
