using HodVisualizer.Contracts;

namespace HodVisualizer.Domain;

internal interface IStatusEvaluator
{
    bool TryEvaluate(in HodPlayerSnapshot snapshot, out StatusSignal signal);
}
