using System.Linq.Expressions;
using System.Reflection;

namespace CallTracing
{
    internal static class ExpressionToCallFactory
    {
        internal static Call CreateCall(
            LambdaExpression lambdaExpression)
        {
            if (lambdaExpression.Body is MethodCallExpression methodCallExpression)
            {
                return CreateMethodCall(methodCallExpression);
            }
            else if (lambdaExpression.Body is InvocationExpression invocationExpression)
            {
                return CreateDelegateCall(invocationExpression);
            }
            else if (lambdaExpression.Body is MemberExpression memberExpression)
            {
                return CreatePropertyCall(memberExpression);
            }
            else
            {
                throw new NotSupportedException($"{nameof(LambdaExpression)}'s {nameof(LambdaExpression.Body)} should be of {nameof(MethodCallExpression)}, {nameof(InvocationExpression)} or {nameof(MemberExpression)} type.");
            }
        }

        internal static DelegateCall CreateDelegateCall(
            InvocationExpression invocationExpression)
        {
            var args = invocationExpression.Arguments.ToObjects();

            return new DelegateCall(invocationExpression.Expression.Type, args);
        }

        internal static MethodCall CreateMethodCall(
            MethodCallExpression methodCallExpression)
        {
            var type = methodCallExpression.Object?.Type ?? throw new NotSupportedException("Static method calls are not supported.");

            var args = methodCallExpression.Arguments.ToObjects();

            return new MethodCall(type, methodCallExpression.Method, args);
        }

        internal static PropertyCall CreatePropertyCall(
            MemberExpression memberExpression)
        {
            if (memberExpression.Member.DeclaringType == null)
            {
                throw new ArgumentException($"{nameof(memberExpression)}.{nameof(MemberExpression.Member)}.{nameof(MemberExpression.Member.DeclaringType)} is null.");
            }

            if (memberExpression.Member is PropertyInfo propertyInfo)
            {
                return new PropertyCall(memberExpression.Member.DeclaringType, propertyInfo);
            }
            else
            {
                throw new ArgumentException($"{nameof(memberExpression)}.{nameof(MemberExpression.Member)} is not {nameof(PropertyInfo)}.");
            }
        }

        internal static IEnumerable<object?> ToObjects(
            this IEnumerable<Expression> argumentExpressions)
        {
            return argumentExpressions.Select(ArgumentExpressionToObject);
        }

        internal static object? ArgumentExpressionToObject(
            Expression argumentExpression)
        {
            // TODO: Not all argument expressions require compilation.
            // Those which were captured as variable values can be extracted as is.
            //
            // For example:
            //
            // Expression<Action<ISomeInterface>> e1 = (ISomeInterface someInterface) => someInterface.Action(new Value()); // <== "new Value()" here is an expression and compilation is required.
            //
            // var value = new Value();
            // Expression<Action<ISomeInterface>> e2 = (ISomeInterface someInterface) => someInterface.Action(value); // <== "value" here is captured and compilation is not required.
            //
            // Hence performance optimization is available.
            return Expression.Lambda(argumentExpression).Compile().DynamicInvoke();
        }
    }
}
