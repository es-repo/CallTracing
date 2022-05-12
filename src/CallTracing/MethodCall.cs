using CallTracing.Utilities;
using System.Reflection;

namespace CallTracing
{
    public sealed record MethodCall : Call
    {
        private readonly List<object?> args;

        public IReadOnlyList<object?> Args { get; private set; }

        public MethodInfo Method { get; private set; }

        public MethodCall(Type type, MethodInfo method, IEnumerable<object?> args)
            : base(type)
        {
            Method = method;
            this.args = args.ToList();
            Args = this.args.AsReadOnly();
        }

        public bool Equals(MethodCall? other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(Type, other.Type) &&
                Equals(Method, other.Method) &&
                Args.SequenceEqual(other.Args);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;

                hash = (hash * 7) + Type.GetHashCode();
                hash = (hash * 7) + Method.GetHashCode();
                hash = Args.Where(o => o != null).Aggregate(hash, (acc, o) => (acc * 7) + o!.GetHashCode());

                return hash;
            }
        }

        public override string ToString()
        {
            var argsString = ObjectUtilities.ObjectToString(Args);

            return $"{nameof(MethodCall)} {{ {nameof(Type)} = {Type}, {nameof(Method)} = {Method}, {nameof(Args)} = {argsString} }}";
        }
    }
}
