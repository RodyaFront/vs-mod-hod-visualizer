using System;
using HodVisualizer.Contracts;

namespace HodVisualizer.Domain.Evaluators;

internal sealed class HeatColdEvaluator : IStatusEvaluator
{
    private const float CoolingEpsilon = 0.01f;
    private const float EffectPercentEpsilon = 0.05f;

    public bool TryEvaluate(in HodPlayerSnapshot snapshot, out StatusSignal signal)
    {
        signal = default;
        if (!snapshot.PlayerReady || !snapshot.Cooling.Available)
        {
            return false;
        }

        float totalCooling = snapshot.Cooling.TotalCooling;
        if (MathF.Abs(totalCooling) <= CoolingEpsilon)
        {
            return false;
        }

        float rateDeltaPercent = ResolveThirstRateDeltaPercent(snapshot);
        if (totalCooling < 0f)
        {
            float increasedLoss = MathF.Max(0f, rateDeltaPercent);
            if (increasedLoss <= EffectPercentEpsilon)
            {
                return false;
            }

            signal = new StatusSignal(
                HodStatusCode.Hot,
                increasedLoss >= 50f ? HodStatusSeverity.High : HodStatusSeverity.Medium,
                increasedLoss,
                AuxiliaryValue: totalCooling,
                PulseMetric: MathF.Abs(totalCooling));
            return true;
        }

        float reducedLoss = MathF.Max(0f, -rateDeltaPercent);
        if (reducedLoss <= EffectPercentEpsilon)
        {
            return false;
        }

        signal = new StatusSignal(
            HodStatusCode.Cold,
            reducedLoss >= 50f ? HodStatusSeverity.High : HodStatusSeverity.Low,
            reducedLoss,
            AuxiliaryValue: totalCooling,
            PulseMetric: MathF.Abs(totalCooling));
        return true;
    }

    private static float ResolveThirstRateDeltaPercent(in HodPlayerSnapshot snapshot)
    {
        if (!snapshot.Thirst.Available || !snapshot.Thirst.BaseThirstDecayRate.HasValue)
        {
            return -snapshot.Cooling.TotalCooling * 10f;
        }

        float baseline = snapshot.Thirst.BaseThirstDecayRate.Value;
        if (baseline <= 0f || !float.IsFinite(baseline))
        {
            return -snapshot.Cooling.TotalCooling * 10f;
        }

        return ((snapshot.Thirst.ThirstRate / baseline) - 1f) * 100f;
    }
}
