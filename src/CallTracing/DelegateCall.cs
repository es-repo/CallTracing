using CallTracing.Utilities;

namespace CallTracing
{
    public sealed record DelegateCall : Call
    {
        private readonly List<object?> args;

        public IReadOnlyList<object?> Args { get; private set; }

        public DelegateCall(Type type, IEnumerable<object?> args)
            : base(type)
        {
            this.args = args.ToList();
            Args = this.args.AsReadOnly();
        }

        public bool Equals(DelegateCall? other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(Type, other.Type) &&
                Args.SequenceEqual(other.Args);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + Type.GetHashCode();

                hash = Args.Where(o => o != null).Aggregate(hash, (acc, o) => (acc * 7) + o!.GetHashCode());

                return hash;
            }
        }

        public override string ToString()
        {
            var argsString = ObjectUtilities.ObjectToString(Args);

            return $"{nameof(DelegateCall)} {{ {nameof(Type)} = {Type}, {nameof(Args)} = {argsString} }}";
        }
    }
}
