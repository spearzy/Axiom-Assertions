using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class MstestAssertMigrationAsyncThrowsTests
{
    [Fact]
    public async Task AssertThrowsExceptionAsync_ActionVariable_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public async Task Check(Func<Task> work)
                    {
                        await {|AXM1068:Assert.ThrowsExceptionAsync<InvalidOperationException>(work)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public async Task Check(Func<Task> work)
                    {
                        await work.Should().ThrowExactlyAsync<InvalidOperationException>();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsExceptionAsync_Lambda_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await {|AXM1068:Assert.ThrowsExceptionAsync<InvalidOperationException>(() => ThrowNowAsync())|};
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsExceptionAsync_MethodGroup_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await Assert.ThrowsExceptionAsync<InvalidOperationException>(ThrowNowAsync);
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsExceptionAsync_UsedResult_AppendsThrown()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public async Task<InvalidOperationException> Check()
                    {
                        var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => ThrowNowAsync());
                        return ex;
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsExactlyAsync_Lambda_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await {|AXM1069:Assert.ThrowsExactlyAsync<InvalidOperationException>(() => ThrowNowAsync())|};
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsAsync_Lambda_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await {|AXM1070:Assert.ThrowsAsync<ArgumentException>(() => ThrowNowAsync())|};
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsAsync_UsedResult_AppendsThrown()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public async Task<ArgumentException> Check()
                    {
                        return await Assert.ThrowsAsync<ArgumentException>(() => ThrowNowAsync());
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsExceptionAsync_WithoutAwait_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public Task<InvalidOperationException> Check()
                    {
                        return Assert.ThrowsExceptionAsync<InvalidOperationException>(() => ThrowNowAsync());
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrowsAsync_WithoutAwait_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public Task<ArgumentException> Check()
                    {
                        return Assert.ThrowsAsync<ArgumentException>(() => ThrowNowAsync());
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrowsExceptionAsync_MessageOverload_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => ThrowNowAsync(), "custom message");
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrowsExactlyAsync_MessageOverload_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => ThrowNowAsync(), "custom message");
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new InvalidOperationException());
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrowsAsync_MessageOverload_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await Assert.ThrowsAsync<ArgumentException>(() => ThrowNowAsync(), "custom message");
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrowsAsync_MessageBuilderOverload_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await Assert.ThrowsAsync<ArgumentException>(
                            () => ThrowNowAsync(),
                            exception => exception?.Message ?? "missing");
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }
}
