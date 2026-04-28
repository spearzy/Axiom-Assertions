using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class NunitAssertMigrationAsyncThrowsTests
{
    [Fact]
    public async Task AssertThrowsAsync_Lambda_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        {|AXM1066:Assert.ThrowsAsync<InvalidOperationException>(() => ThrowNowAsync())|};
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await new Func<Task>(() => ThrowNowAsync()).Should().ThrowExactlyAsync<InvalidOperationException>();
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsAsync_MethodGroup_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        Assert.ThrowsAsync<InvalidOperationException>(ThrowNowAsync);
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await new Func<Task>(ThrowNowAsync).Should().ThrowExactlyAsync<InvalidOperationException>();
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsAsync_UsedResult_AppendsThrown()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public async Task<InvalidOperationException> Check()
                    {
                        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => ThrowNowAsync());
                        return ex;
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public async Task<InvalidOperationException> Check()
                    {
                        var ex = (await new Func<Task>(() => ThrowNowAsync()).Should().ThrowExactlyAsync<InvalidOperationException>()).Thrown;
                        return ex;
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsAsync_UsedAsArgument_AppendsThrown()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        Use(Assert.ThrowsAsync<ArgumentException>(() => ThrowNowAsync()));
                    }

                    private static void Use(ArgumentException exception) { }
                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        Use((await new Func<Task>(() => ThrowNowAsync()).Should().ThrowExactlyAsync<ArgumentException>()).Thrown);
                    }

                    private static void Use(ArgumentException exception) { }
                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertCatchAsync_Lambda_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        {|AXM1067:Assert.CatchAsync<ArgumentException>(() => ThrowNowAsync())|};
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await new Func<Task>(() => ThrowNowAsync()).Should().ThrowAsync<ArgumentException>();
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertCatchAsync_UsedResult_AppendsThrown()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public async Task<ArgumentException> Check()
                    {
                        return Assert.CatchAsync<ArgumentException>(() => ThrowNowAsync());
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public async Task<ArgumentException> Check()
                    {
                        return (await new Func<Task>(() => ThrowNowAsync()).Should().ThrowAsync<ArgumentException>()).Thrown;
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsAsync_InNonAsyncMethod_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check()
                    {
                        Assert.ThrowsAsync<InvalidOperationException>(() => ThrowNowAsync());
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrowsAsync_MessageOverload_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        Assert.ThrowsAsync<InvalidOperationException>(() => ThrowNowAsync(), "custom message");
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertCatchAsync_MessageOverload_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        Assert.CatchAsync<ArgumentException>(() => ThrowNowAsync(), "custom message");
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrowsAsync_TypeOverload_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        Assert.ThrowsAsync(typeof(InvalidOperationException), () => ThrowNowAsync());
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrowsAsync_AsyncTestDelegateVariable_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public async Task Check(AsyncTestDelegate work)
                    {
                        Assert.ThrowsAsync<InvalidOperationException>(work);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }
}
