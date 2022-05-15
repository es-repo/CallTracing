using System.Collections;
using System.Linq.Expressions;
using Xunit;

namespace CallTracing.Tests.CallTraceTests
{
    public static class CtorWithLambdaExpressionsTest
    {
        interface ISomeInterface
        {
            void Action(string value);

            int Func(string value);

            int Property { get; }
        }

        delegate int SomeDelegate(string value);

        public sealed record Args
        {
            public LambdaExpression[] LambdaExpressions { get; init; } = new LambdaExpression[0];
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<object[]> GetEnumerator()
            {
                // 1.
                yield return TestCase1();
            }

            private object[] TestCase1()
            {
                var args = new Args
                {
                    LambdaExpressions = new LambdaExpression[]
                    {
                        (ISomeInterface someInterface) => someInterface.Action("a"),
                        (ISomeInterface someInterface) => someInterface.Func("b"),
                        (SomeDelegate someDelegate) => someDelegate("c"),
                        (ISomeInterface someInterface) => someInterface.Property,
                    }
                };

                var expected = new CallTrace(
                    new Call[]
                    {
                        new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Action))!, new object?[] { "a" }),
                        new MethodCall(typeof(ISomeInterface), typeof(ISomeInterface).GetMethod(nameof(ISomeInterface.Func))!, new object?[] { "b" }),
                        new DelegateCall(typeof(SomeDelegate), new object?[] { "c" }),
                        new PropertyCall(typeof(ISomeInterface), typeof(ISomeInterface).GetProperty(nameof(ISomeInterface.Property))!)
                    });

                return new object[] { args, expected };
            }
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            Args args,
            CallTrace expected)
        {
            var actual = new CallTrace(args.LambdaExpressions);

            Assert.Equal(expected, actual);
        }
    }

    static class Within
    {

        interface IBox
        {
            int Count { get; }
            void Open();
            void PutInto(object thing);
            void Close();
        }

        delegate void WriteLog(string message);

        static void FillBox(IBox box, IEnumerable<object> things, WriteLog writeLog)
        {
            box.Open();
            writeLog("The box is opened.");

            foreach (var thing in things)
            {
                box.PutInto(thing);
            }

            box.Close();
            writeLog("The box is closed.");
        }

        class BoxMock : IBox
        {
            private readonly CallTrace callTrace;

            public BoxMock(CallTrace callTrace)
            {
                this.callTrace = callTrace;
            }

            public int Count
            {
                get
                {
                    Expression<Func<IBox, int>> e = (IBox box) => box.Count;

                    callTrace.Add<IBox, int>(box => box.Count);
                    return 3;
                }
            }

            public void Open()
            {
                callTrace.Add<IBox>(box => box.Open());
            }

            public void PutInto(object thing)
            {
                callTrace.Add<IBox>(box => box.PutInto(thing));
            }

            public void Close()
            {
                callTrace.Add<IBox>(box => box.Close());
            }
        }
    }
}
