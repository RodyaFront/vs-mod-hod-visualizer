using HodVisualizer.Domain;
using Vintagestory.API.Common;

namespace HodVisualizer.Presentation;

internal static class HodStatusIconCatalog
{
    internal static readonly AssetLocation Thirst = new("hodvisualizer", "textures/icons/thirst.png");
    internal static readonly AssetLocation SevereThirst = new("hodvisualizer", "textures/icons/severe_thirst.png");
    internal static readonly AssetLocation Dehydrating = new("hodvisualizer", "textures/icons/dehydrating.png");
    internal static readonly AssetLocation Hot = new("hodvisualizer", "textures/icons/hot.png");
    internal static readonly AssetLocation Cold = new("hodvisualizer", "textures/icons/cold.png");
    internal static readonly AssetLocation Encumbrance = new("hodvisualizer", "textures/icons/encumbrance.png");
    internal static readonly AssetLocation NutritionDeficit = new("hodvisualizer", "textures/icons/nutrition_deficit.png");

    internal static AssetLocation Resolve(HodStatusCode code)
    {
        return code switch
        {
            HodStatusCode.Thirst => Thirst,
            HodStatusCode.SevereThirst => SevereThirst,
            HodStatusCode.Dehydrating => Dehydrating,
            HodStatusCode.Hot => Hot,
            HodStatusCode.Cold => Cold,
            HodStatusCode.LiquidEncumbrance => Encumbrance,
            HodStatusCode.NutritionDeficit => NutritionDeficit,
            _ => Thirst
        };
    }
}
