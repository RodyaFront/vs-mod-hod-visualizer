using HodVisualizer.Contracts;
using HodVisualizer.Domain;
using HodVisualizer.Domain.Evaluators;
using Xunit;

namespace HodVisualizer.Tests;

public sealed class ThirstTierEvaluatorTests
{
    private readonly ThirstTierEvaluator _sut = new();

    [Fact]
    public void ReturnsNoSignal_WhenHydrationAbove600()
    {
        var snapshot = BuildSnapshot(current: 900f, max: 1500f);

        bool hasSignal = _sut.TryEvaluate(snapshot, out _);

        Assert.False(hasSignal);
    }

    [Fact]
    public void ReturnsMediumThirst_WhenHydrationBetween0And600()
    {
        var snapshot = BuildSnapshot(current: 450f, max: 1500f);

        bool hasSignal = _sut.TryEvaluate(snapshot, out StatusSignal signal);

        Assert.True(hasSignal);
        Assert.Equal(HodStatusCode.SevereThirst, signal.Code);
        Assert.Equal(HodStatusSeverity.Medium, signal.Severity);
        Assert.True(signal.AuxiliaryValue > 0f);
    }

    [Fact]
    public void ReturnsNoSignal_WhenHydrationJustAboveThreshold()
    {
        var snapshot = BuildSnapshot(current: 601f, max: 1500f);

        bool hasSignal = _sut.TryEvaluate(snapshot, out _);

        Assert.False(hasSignal);
    }

    [Fact]
    public void ReturnsMediumThirst_WhenHydrationAtThreshold()
    {
        var snapshot = BuildSnapshot(current: 600f, max: 1500f);

        bool hasSignal = _sut.TryEvaluate(snapshot, out StatusSignal signal);

        Assert.True(hasSignal);
        Assert.Equal(HodStatusCode.SevereThirst, signal.Code);
        Assert.Equal(HodStatusSeverity.Medium, signal.Severity);
        Assert.Equal(0f, signal.AuxiliaryValue, 3);
    }

    [Fact]
    public void ReturnsDehydrating_WhenCurrentIsZero()
    {
        var snapshot = BuildSnapshot(current: 0f, max: 1500f);

        bool hasSignal = _sut.TryEvaluate(snapshot, out StatusSignal signal);

        Assert.True(hasSignal);
        Assert.Equal(HodStatusCode.Dehydrating, signal.Code);
        Assert.Equal(HodStatusSeverity.Critical, signal.Severity);
        Assert.Equal(30f, signal.AuxiliaryValue, 3);
    }

    private static HodPlayerSnapshot BuildSnapshot(float current, float max)
    {
        return new HodPlayerSnapshot(
            true,
            new ThirstSnapshot(true, current, max, 10f, 10f, 0f, 0),
            default,
            default);
    }
}
