using HodVisualizer.Contracts;
using HodVisualizer.Domain;
using HodVisualizer.Domain.Evaluators;
using Xunit;

namespace HodVisualizer.Tests;

public sealed class HeatColdEvaluatorTests
{
    private readonly HeatColdEvaluator _sut = new();

    [Fact]
    public void ReturnsHot_WhenCoolingIsNegative()
    {
        var snapshot = new HodPlayerSnapshot(
            true,
            new ThirstSnapshot(true, 500f, 1000f, 15f, 10f, 0f, 0),
            new CoolingSnapshot(true, -2f, 0f, false, false, false, false),
            default);

        bool hasSignal = _sut.TryEvaluate(snapshot, out StatusSignal signal);

        Assert.True(hasSignal);
        Assert.Equal(HodStatusCode.Hot, signal.Code);
        Assert.True(signal.Value > 0f);
    }

    [Fact]
    public void ReturnsCold_WhenCoolingIsPositive()
    {
        var snapshot = new HodPlayerSnapshot(
            true,
            new ThirstSnapshot(true, 500f, 1000f, 7.5f, 10f, 0f, 0),
            new CoolingSnapshot(true, 3f, 0f, false, false, false, false),
            default);

        bool hasSignal = _sut.TryEvaluate(snapshot, out StatusSignal signal);

        Assert.True(hasSignal);
        Assert.Equal(HodStatusCode.Cold, signal.Code);
        Assert.True(signal.Value > 0f);
    }

    [Fact]
    public void ReturnsNoSignal_WhenCoolingNearZero()
    {
        var snapshot = new HodPlayerSnapshot(
            true,
            new ThirstSnapshot(true, 500f, 1000f, 10f, 10f, 0f, 0),
            new CoolingSnapshot(true, 0f, 0f, false, false, false, false),
            default);

        bool hasSignal = _sut.TryEvaluate(snapshot, out _);

        Assert.False(hasSignal);
    }

    [Fact]
    public void ReturnsNoSignal_WhenCoolingPositiveButNoRealThirstReduction()
    {
        var snapshot = new HodPlayerSnapshot(
            true,
            new ThirstSnapshot(true, 500f, 1000f, 10f, 10f, 0f, 0),
            new CoolingSnapshot(true, 2f, 0f, false, false, false, false),
            default);

        bool hasSignal = _sut.TryEvaluate(snapshot, out _);

        Assert.False(hasSignal);
    }
}
