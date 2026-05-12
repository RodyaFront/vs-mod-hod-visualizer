using HodVisualizer.Contracts;
using HodVisualizer.Domain;
using HodVisualizer.Domain.Evaluators;
using Xunit;

namespace HodVisualizer.Tests;

public sealed class EncumbranceEvaluatorTests
{
    private readonly EncumbranceEvaluator _sut = new();

    [Fact]
    public void ReturnsNoSignal_WhenPenaltyMissing()
    {
        var snapshot = new HodPlayerSnapshot(
            true,
            default,
            default,
            new EncumbranceSnapshot(false, 0f));

        bool hasSignal = _sut.TryEvaluate(snapshot, out _);

        Assert.False(hasSignal);
    }

    [Fact]
    public void ReturnsSignal_WhenPenaltyIsActive()
    {
        var snapshot = new HodPlayerSnapshot(
            true,
            default,
            default,
            new EncumbranceSnapshot(true, 0.3f));

        bool hasSignal = _sut.TryEvaluate(snapshot, out StatusSignal signal);

        Assert.True(hasSignal);
        Assert.Equal(HodStatusCode.LiquidEncumbrance, signal.Code);
        Assert.Equal(30f, signal.Value, 3);
    }
}
