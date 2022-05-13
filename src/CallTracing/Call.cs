namespace CallTracing
{
    public abstract record Call
    {
        /// <summary>
        /// Delegate's type or method's or property's declaring type.
        /// </summary>
        public Type Type { get; protected set; }

        protected Call(
            Type type)
        {
            Type = type;
        }
    }
}
