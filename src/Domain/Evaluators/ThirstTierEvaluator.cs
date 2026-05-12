using HodVisualizer.Contracts;
using Vintagestory.API.MathTools;

namespace HodVisualizer.Domain.Evaluators;

internal sealed class ThirstTierEvaluator : IStatusEvaluator
{
    private const float MediumThresholdUnits = 600f;
    private const float MaxMovementPenaltyPercent = 30f;

    public bool TryEvaluate(in HodPlayerSnapshot snapshot, out StatusSignal signal)
    {
        signal = default;
        if (!snapshot.PlayerReady || !snapshot.Thirst.Available || snapshot.Thirst.MaxThirst <= 0f)
        {
            return false;
        }

        if (snapshot.Thirst.CurrentThirst <= 0f)
        {
            signal = new StatusSignal(
                HodStatusCode.Dehydrating,
                HodStatusSeverity.Critical,
                0f,
                AuxiliaryValue: MaxMovementPenaltyPercent,
                PulseMetric: 1f);
            return true;
        }

        if (snapshot.Thirst.CurrentThirst <= MediumThresholdUnits)
        {
            float movePenaltyPercent = (1f - (snapshot.Thirst.CurrentThirst / MediumThresholdUnits)) * MaxMovementPenaltyPercent;
            movePenaltyPercent = GameMath.Clamp(movePenaltyPercent, 0f, MaxMovementPenaltyPercent);

            signal = new StatusSignal(
                HodStatusCode.SevereThirst,
                HodStatusSeverity.Medium,
                snapshot.Thirst.CurrentThirst,
                AuxiliaryValue: movePenaltyPercent,
                PulseMetric: MediumThresholdUnits - snapshot.Thirst.CurrentThirst);
            return true;
        }

        return false;
    }
}
