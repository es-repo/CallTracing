using System.Linq.Expressions;

namespace CallTracing
{
    public sealed record CallTrace
    {
        private readonly List<Call> calls;

        public IReadOnlyList<Call> Calls { get; private set; }

        internal CallTrace(
            IEnumerable<Call> calls)
        {
            this.calls = calls.ToList();
            Calls = this.calls.AsReadOnly();
        }

        public CallTrace() : this(new List<Call>())
        {
        }

        public void Add<TMock, TResult>(
            Expression<Func<TMock, TResult>> lambdaExpression)
        {
            AddUntyped(lambdaExpression);
        }

        public void Add<TMock>(
            Expression<Action<TMock>> lambdaExpression)
        {
            AddUntyped(lambdaExpression);
        }

        private void AddUntyped(
            LambdaExpression lambdaExpression)
        {
            var call = ExpressionToCallFactory.CreateCall(lambdaExpression);
            calls.Add(call);
        }

        public bool Equals(
            CallTrace? other)
        {
            if (other == null)
            {
                return false;
            }

            return calls.SequenceEqual(other.calls);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;

                hash = calls.Aggregate(hash, (acc, o) => (acc * 7) + o.GetHashCode());

                return hash;
            }
        }

        public override string ToString()
        {
            var callsString = Utilities.ObjectUtilities.ObjectToString(Calls);

            return $"{nameof(CallTrace)} {{ {nameof(Calls)} = {callsString} }}";
        }
    }
}
