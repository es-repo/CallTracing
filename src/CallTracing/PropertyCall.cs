using System.Reflection;

namespace CallTracing
{
    public sealed record PropertyCall : Call
    {
        public PropertyInfo Property { get; private set; }

        public PropertyCall(Type type, PropertyInfo property)
            : base(type)
        {
            Property = property;
        }

        public bool Equals(PropertyCall? other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(Type, other.Type) &&
                Equals(Property, other.Property);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + Type.GetHashCode();
                hash = (hash * 7) + Property.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"{nameof(PropertyCall)} {{ {nameof(Type)} = {Type}, {nameof(Property)} = {Property} }}";
        }
    }
}
