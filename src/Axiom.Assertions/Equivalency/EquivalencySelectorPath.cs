using System.Linq.Expressions;

namespace Axiom.Assertions.Equivalency;

internal static class EquivalencySelectorPath
{
    // Converts a selector like:
    //   x => x.Address.Postcode
    // into a stable path string:
    //   "Address.Postcode"
    // The equivalency engine already works with path strings which this keeps
    // expression-based APIs and path-based APIs using the same underlying model.
    public static string Create<TSubject>(
        Expression<Func<TSubject, object?>> selector,
        string paramName)
    {
        ArgumentNullException.ThrowIfNull(selector);

        var segments = new Stack<string>();
        // Walk the expression tree and collect member names in order.
        // If the selector is not a simple member chain, fail fast with a clear message.
        if (!TryCollectSegments(StripConversions(selector.Body), selector.Parameters[0], segments) || segments.Count == 0)
        {
            throw new ArgumentException(
                "Selector must be a simple member-access path such as x => x.Name or x => x.Address.Postcode.",
                paramName);
        }

        return string.Join(".", segments);
    }

    private static bool TryCollectSegments(
        Expression? expression,
        ParameterExpression rootParameter,
        Stack<string> segments)
    {
        switch (expression)
        {
            case MemberExpression memberExpression:
                // Expression trees are leaf-first (Postcode -> Address -> x),
                // so we push each member then recurse toward the root parameter.
                segments.Push(memberExpression.Member.Name);
                return TryCollectSegments(
                    StripConversions(memberExpression.Expression),
                    rootParameter,
                    segments);

            case ParameterExpression parameterExpression:
                // Valid selector paths must end at the original lambda parameter (x).
                return ReferenceEquals(parameterExpression, rootParameter);

            default:
                // Method calls, arithmetic, constants, etc. are rejected.
                return false;
        }
    }

    private static Expression? StripConversions(Expression? expression)
    {
        // Selectors returning object? often include compiler-added conversions
        // (for example boxing value types). Remove those wrappers so member
        // traversal sees the real expression chain.
        while (expression is UnaryExpression
            {
                NodeType: ExpressionType.Convert
                   or ExpressionType.ConvertChecked
                   or ExpressionType.TypeAs
            } unaryExpression)
        {
            expression = unaryExpression.Operand;
        }

        return expression;
    }
}
