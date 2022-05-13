//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using Xunit;

//namespace CallTracing.Tests.CallTraceTests
//{
//    public static class AddLambdaFuncBodyTests
//    {
//        public delegate double SomeDelegate(double value1, double value2);

//        public record Args
//        {
//            public Expression<Func<SomeDelegate, double>> LambdaExpression { get; init; } = null!;
//        }

//        class TestCases : IEnumerable<object[]>
//        {
//            public IEnumerator<object[]> GetEnumerator()
//            {
//                // 1. 
//                yield return EmptyCallTrace_LambdaWithFuncBody_CallAddedToCallTrace();
//            }

//            static object[] EmptyCallTrace_LambdaWithFuncBody_CallAddedToCallTrace()
//            {
//                var stateActual = new CallTrace();

//                Expression<Func<SomeDelegate, double>> lambdaExpression = (SomeDelegate someDelegate) => someDelegate(1.34, 2.32);

//                var args = new Args
//                {
//                    LambdaExpression = lambdaExpression
//                };

//                var expectedState = new CallTrace(new MockCallTrace[]
//                {
//                    new MockCallTrace
//                    {
//                        Type = typeof(SomeDelegate),
//                        Args = new List<object?> { 1.34, 2.32 },
//                        Method = null
//                    }
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
