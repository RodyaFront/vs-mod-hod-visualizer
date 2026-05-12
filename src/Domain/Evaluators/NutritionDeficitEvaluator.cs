using HodVisualizer.Contracts;

namespace HodVisualizer.Domain.Evaluators;

internal sealed class NutritionDeficitEvaluator : IStatusEvaluator
{
    public bool TryEvaluate(in HodPlayerSnapshot snapshot, out StatusSignal signal)
    {
        signal = default;
        if (!snapshot.PlayerReady || !snapshot.Thirst.Available || snapshot.Thirst.NutritionDeficitAmount <= 0f)
        {
            return false;
        }

        signal = new StatusSignal(
            HodStatusCode.NutritionDeficit,
            snapshot.Thirst.NutritionDeficitAmount >= 200f ? HodStatusSeverity.Medium : HodStatusSeverity.Low,
            snapshot.Thirst.NutritionDeficitAmount,
            PulseMetric: snapshot.Thirst.NutritionDeficitAmount);
        return true;
    }
}
