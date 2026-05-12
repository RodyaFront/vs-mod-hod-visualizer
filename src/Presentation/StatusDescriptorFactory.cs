using HodVisualizer.Domain;
using PlayerStatusStrip;
using Vintagestory.API.Client;

namespace HodVisualizer.Presentation;

internal sealed class StatusDescriptorFactory : IStatusDescriptorFactory
{
    public StatusDescriptor Create(ICoreClientAPI capi, in StatusSignal signal)
    {
        return new StatusDescriptor(
            StableIdFor(signal.Code),
            HodStatusIconCatalog.Resolve(signal.Code),
            SortOrderFor(signal.Code),
            HodTooltipBuilder.Build(signal),
            signal.PulseMetric,
            AffectKindFor(signal.Code));
    }

    private static string StableIdFor(HodStatusCode code)
    {
        return code switch
        {
            HodStatusCode.Thirst => "hodvisualizer:thirst",
            HodStatusCode.SevereThirst => "hodvisualizer:severe-thirst",
            HodStatusCode.Dehydrating => "hodvisualizer:dehydrating",
            HodStatusCode.Hot => "hodvisualizer:hot",
            HodStatusCode.Cold => "hodvisualizer:cold",
            HodStatusCode.LiquidEncumbrance => "hodvisualizer:liquid-encumbrance",
            HodStatusCode.NutritionDeficit => "hodvisualizer:nutrition-deficit",
            _ => "hodvisualizer:unknown"
        };
    }

    private static int SortOrderFor(HodStatusCode code)
    {
        return code switch
        {
            HodStatusCode.Dehydrating => 0,
            HodStatusCode.LiquidEncumbrance => 10,
            HodStatusCode.SevereThirst => 20,
            HodStatusCode.Hot => 30,
            HodStatusCode.Thirst => 40,
            HodStatusCode.Cold => 50,
            HodStatusCode.NutritionDeficit => 60,
            _ => 90
        };
    }

    private static StatusAffectKind AffectKindFor(HodStatusCode code)
    {
        return code switch
        {
            HodStatusCode.Cold => StatusAffectKind.Positive,
            HodStatusCode.Thirst or
            HodStatusCode.SevereThirst or
            HodStatusCode.Dehydrating or
            HodStatusCode.Hot or
            HodStatusCode.LiquidEncumbrance or
            HodStatusCode.NutritionDeficit => StatusAffectKind.Negative,
            _ => StatusAffectKind.Neutral
        };
    }
}
