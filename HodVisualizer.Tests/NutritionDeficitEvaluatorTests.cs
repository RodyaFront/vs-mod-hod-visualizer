using HodVisualizer.Contracts;
using HodVisualizer.Domain;
using HodVisualizer.Domain.Evaluators;
using Xunit;

namespace HodVisualizer.Tests;

public sealed class NutritionDeficitEvaluatorTests
{
    private readonly NutritionDeficitEvaluator _sut = new();

    [Fact]
    public void ReturnsNoSignal_WhenDeficitIsZero()
    {
        var snapshot = new HodPlayerSnapshot(
            true,
            new ThirstSnapshot(true, 500f, 1000f, 10f, 10f, 0f, 0),
            default,
            default);

        bool hasSignal = _sut.TryEvaluate(snapshot, out _);

        Assert.False(hasSignal);
    }

    [Fact]
    public void ReturnsSignal_WhenDeficitPositive()
    {
        var snapshot = new HodPlayerSnapshot(
            true,
            new ThirstSnapshot(true, 500f, 1000f, 10f, 10f, 120f, 0),
            default,
            default);

        bool hasSignal = _sut.TryEvaluate(snapshot, out StatusSignal signal);

        Assert.True(hasSignal);
        Assert.Equal(HodStatusCode.NutritionDeficit, signal.Code);
        Assert.Equal(120f, signal.Value, 3);
    }
}
