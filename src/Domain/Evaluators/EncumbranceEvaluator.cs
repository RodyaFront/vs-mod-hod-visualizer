using HodVisualizer.Contracts;

namespace HodVisualizer.Domain.Evaluators;

internal sealed class EncumbranceEvaluator : IStatusEvaluator
{
    private const float Epsilon = 0.0001f;

    public bool TryEvaluate(in HodPlayerSnapshot snapshot, out StatusSignal signal)
    {
        signal = default;
        if (!snapshot.PlayerReady || !snapshot.Encumbrance.Available || snapshot.Encumbrance.WalkSpeedPenalty <= Epsilon)
        {
            return false;
        }

        float penaltyPercent = snapshot.Encumbrance.WalkSpeedPenalty * 100f;
        signal = new StatusSignal(
            HodStatusCode.LiquidEncumbrance,
            penaltyPercent >= 20f ? HodStatusSeverity.High : HodStatusSeverity.Medium,
            penaltyPercent,
            PulseMetric: penaltyPercent);
        return true;
    }
}
