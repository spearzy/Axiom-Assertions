namespace Axiom.Analyzers.Tests.Helpers;

internal static class NunitAssertStubs
{
    public const string Source =
        """
        using System;
        using System.Collections.Generic;
        using System.Threading.Tasks;

        namespace NUnit.Framework
        {
            using Constraints;

            public delegate Task AsyncTestDelegate();

            public static class Assert
            {
                public static void That<TActual>(TActual actual, IResolveConstraint expression) { }
                public static void That<TActual>(TActual actual, IResolveConstraint expression, string message) { }
                public static TActual ThrowsAsync<TActual>(AsyncTestDelegate code) where TActual : Exception => default!;
                public static TActual ThrowsAsync<TActual>(AsyncTestDelegate code, string message, params object[] args) where TActual : Exception => default!;
                public static Exception ThrowsAsync(Type expectedExceptionType, AsyncTestDelegate code) => default!;
                public static Exception ThrowsAsync(Type expectedExceptionType, AsyncTestDelegate code, string message, params object[] args) => default!;
                public static TActual CatchAsync<TActual>(AsyncTestDelegate code) where TActual : Exception => default!;
                public static TActual CatchAsync<TActual>(AsyncTestDelegate code, string message, params object[] args) where TActual : Exception => default!;
                public static Exception CatchAsync(Type expectedExceptionType, AsyncTestDelegate code) => default!;
                public static Exception CatchAsync(Type expectedExceptionType, AsyncTestDelegate code, string message, params object[] args) => default!;
            }

            public static class Is
            {
                public static ConstraintExpression Not => default!;
                public static NullConstraint Null => default!;
                public static TrueConstraint True => default!;
                public static FalseConstraint False => default!;
                public static EmptyConstraint Empty => default!;
                public static UniqueConstraint Unique => default!;
                public static EqualConstraint EqualTo(object? expected) => default!;
                public static EqualConstraint EqualTo(string expected) => default!;
                public static EqualConstraint EqualTo<T>(IEnumerable<T> expected) => default!;
                public static SameAsConstraint SameAs(object? expected) => default!;
                public static ComparableConstraint GreaterThan(object? expected) => default!;
                public static ComparableConstraint GreaterThanOrEqualTo(object? expected) => default!;
                public static ComparableConstraint LessThan(object? expected) => default!;
                public static ComparableConstraint LessThanOrEqualTo(object? expected) => default!;
                public static RangeConstraint InRange(object? minimum, object? maximum) => default!;
                public static TypeOfConstraint TypeOf<TExpected>() => default!;
                public static TypeOfConstraint TypeOf(Type expectedType) => default!;
                public static InstanceOfConstraint InstanceOf<TExpected>() => default!;
                public static InstanceOfConstraint InstanceOf(Type expectedType) => default!;
                public static AssignableToConstraint AssignableTo<TExpected>() => default!;
                public static AssignableToConstraint AssignableTo(Type expectedType) => default!;
            }

            public static class Does
            {
                public static ConstraintExpression Not => default!;
                public static ContainsConstraint Contain(string expected) => default!;
                public static StartsWithConstraint StartWith(string expected) => default!;
                public static EndsWithConstraint EndWith(string expected) => default!;
            }

            public static class Has
            {
                public static ConstraintExpression No => default!;
                public static CountConstraintExpression Count => default!;
                public static MemberConstraint Member(object? expected) => default!;
            }
        }

        namespace NUnit.Framework.Constraints
        {
            public interface IResolveConstraint { }

            public class ConstraintExpression : IResolveConstraint
            {
                public ConstraintExpression Not => default!;
                public NullConstraint Null => default!;
                public TrueConstraint True => default!;
                public FalseConstraint False => default!;
                public EmptyConstraint Empty => default!;
                public UniqueConstraint Unique => default!;
                public EqualConstraint EqualTo(object? expected) => default!;
                public EqualConstraint EqualTo(string expected) => default!;
                public SameAsConstraint SameAs(object? expected) => default!;
                public ComparableConstraint GreaterThan(object? expected) => default!;
                public ComparableConstraint GreaterThanOrEqualTo(object? expected) => default!;
                public ComparableConstraint LessThan(object? expected) => default!;
                public ComparableConstraint LessThanOrEqualTo(object? expected) => default!;
                public RangeConstraint InRange(object? minimum, object? maximum) => default!;
                public TypeOfConstraint TypeOf<TExpected>() => default!;
                public TypeOfConstraint TypeOf(Type expectedType) => default!;
                public InstanceOfConstraint InstanceOf<TExpected>() => default!;
                public InstanceOfConstraint InstanceOf(Type expectedType) => default!;
                public AssignableToConstraint AssignableTo<TExpected>() => default!;
                public AssignableToConstraint AssignableTo(Type expectedType) => default!;
                public ContainsConstraint Contain(string expected) => default!;
                public MemberConstraint Member(object? expected) => default!;
                public StartsWithConstraint StartWith(string expected) => default!;
                public EndsWithConstraint EndWith(string expected) => default!;
            }

            public class EqualConstraint : ConstraintExpression
            {
                public EqualConstraint Using(object comparer) => this;
                public EqualConstraint Within(double tolerance) => this;
            }

            public sealed class NullConstraint : ConstraintExpression { }
            public sealed class TrueConstraint : ConstraintExpression { }
            public sealed class FalseConstraint : ConstraintExpression { }
            public sealed class EmptyConstraint : ConstraintExpression { }
            public sealed class UniqueConstraint : ConstraintExpression { }
            public sealed class SameAsConstraint : ConstraintExpression { }
            public sealed class ComparableConstraint : ConstraintExpression
            {
                public ComparableConstraint Using(object comparer) => this;
                public ComparableConstraint Within(double tolerance) => this;
            }
            public sealed class RangeConstraint : ConstraintExpression
            {
                public RangeConstraint Using(object comparer) => this;
            }
            public sealed class TypeOfConstraint : ConstraintExpression { }
            public sealed class InstanceOfConstraint : ConstraintExpression { }
            public sealed class AssignableToConstraint : ConstraintExpression { }
            public sealed class ContainsConstraint : ConstraintExpression
            {
                public ContainsConstraint IgnoreCase => this;
            }
            public sealed class MemberConstraint : ConstraintExpression
            {
                public MemberConstraint Using(object comparer) => this;
            }
            public sealed class StartsWithConstraint : ConstraintExpression { }
            public sealed class EndsWithConstraint : ConstraintExpression { }
            public sealed class CountConstraintExpression : ConstraintExpression
            {
                public EqualConstraint EqualTo(int expected) => default!;
            }
        }
        """;
}
