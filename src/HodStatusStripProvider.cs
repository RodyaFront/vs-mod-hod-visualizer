using System.Collections.Generic;
using HodVisualizer.Adapters;
using HodVisualizer.Domain;
using HodVisualizer.Domain.Evaluators;
using HodVisualizer.Presentation;
using PlayerStatusStrip;
using Vintagestory.API.Client;

namespace HodVisualizer;

internal sealed class HodStatusStripProvider : IStatusStripProvider
{
    private readonly IHodStateReader _stateReader;
    private readonly IReadOnlyList<IStatusEvaluator> _evaluators;
    private readonly IStatusDescriptorFactory _descriptorFactory;

    internal HodStatusStripProvider()
        : this(
            new HodStateReader(),
            [
                new ThirstTierEvaluator(),
                new HeatColdEvaluator(),
                new EncumbranceEvaluator(),
                new NutritionDeficitEvaluator()
            ],
            new StatusDescriptorFactory())
    {
    }

    internal HodStatusStripProvider(
        IHodStateReader stateReader,
        IReadOnlyList<IStatusEvaluator> evaluators,
        IStatusDescriptorFactory descriptorFactory)
    {
        _stateReader = stateReader;
        _evaluators = evaluators;
        _descriptorFactory = descriptorFactory;
    }

    public void Collect(ICoreClientAPI capi, float deltaTime, List<StatusDescriptor> dest)
    {
        var snapshot = _stateReader.Read(capi);
        if (!snapshot.PlayerReady)
        {
            return;
        }

        foreach (IStatusEvaluator evaluator in _evaluators)
        {
            if (!evaluator.TryEvaluate(snapshot, out StatusSignal signal))
            {
                continue;
            }

            dest.Add(_descriptorFactory.Create(capi, signal));
        }
    }
}
