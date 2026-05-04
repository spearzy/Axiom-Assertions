using Axiom.Assertions;
using Axiom.Vectors;

namespace Axiom.Vectors.PackageSmoke;

public sealed class VectorsPackageSmokeTests
{
    [Fact]
    public void VectorAssertions_WorkThroughThePublishedPackage()
    {
        var embedding = new float[] { 1f, 0f, 0f };
        var expected = new float[] { 1f, 0f, 0f };
        var unrelated = new float[] { 0f, 1f, 0f };
        ReadOnlyMemory<float> embeddingMemory = embedding;
        ReadOnlyMemory<float> expectedMemory = expected;
        ReadOnlyMemory<float> unrelatedMemory = unrelated;
        ReadOnlyMemory<float> zero = new float[] { 0f, 0f, 0f };

        embedding.Should().HaveDimension(3);
        embedding.Should().NotContainNaNOrInfinity();
        embedding.Should().BeApproximatelyEqualTo(expected, tolerance: 0.001f);
        embedding.Should().HaveDotProductWith(expected, expectedDotProduct: 1f, tolerance: 0.001f);
        embedding.Should().HaveEuclideanDistanceTo(unrelated, expectedDistance: 1.4142135f, tolerance: 0.001f);
        embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.999f);
        embedding.Should().BeNormalized(tolerance: 0.001f);
        embedding.Should().NotBeZeroVector();

        embeddingMemory.Should().HaveDotProductWith(expectedMemory, expectedDotProduct: 1f, tolerance: 0.001f);
        embeddingMemory.Should().HaveEuclideanDistanceTo(unrelatedMemory, expectedDistance: 1.4142135f, tolerance: 0.001f);
        zero.Should().BeZeroVector();
    }

    [Fact]
    public void RankingAssertions_WorkThroughThePublishedPackage()
    {
        var results = new[] { "doc-2", "doc-7", "doc-5", "doc-9" };
        var relevantItems = new[] { "doc-2", "doc-5" };
        var queries = new[]
        {
            new RankingQuery<string>(["doc-2", "doc-7", "doc-5"], ["doc-2"]),
            new RankingQuery<string>(["doc-8", "doc-5", "doc-3"], ["doc-5"]),
        };

        results.Should().ContainInTopK("doc-7", 2);
        results.Should().HaveRank("doc-7", 2);
        results.Should().HaveRecallAt(2, relevantItems, expectedRecall: 0.5d, tolerance: 0.001d);
        results.Should().HavePrecisionAt(2, relevantItems, expectedPrecision: 0.5d, tolerance: 0.001d);
        results.Should().HaveReciprocalRank("doc-7", expectedReciprocalRank: 0.5d, tolerance: 0.001d);
        queries.Should().HaveMeanReciprocalRank(expectedMeanReciprocalRank: 0.75d, tolerance: 0.001d);
        queries.Should().HaveHitRateAt(k: 1, expectedHitRate: 0.5d, tolerance: 0.001d);
    }
}
