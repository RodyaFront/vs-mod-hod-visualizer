using System.Globalization;
using HodVisualizer.Domain;
using Vintagestory.API.Config;

namespace HodVisualizer.Presentation;

internal static class HodTooltipBuilder
{
    internal static string Build(in StatusSignal signal)
    {
        return signal.Code switch
        {
            HodStatusCode.Thirst => BuildThirst(signal.Value),
            HodStatusCode.SevereThirst => BuildSevereThirst(signal.Value, signal.AuxiliaryValue),
            HodStatusCode.Dehydrating => BuildDehydrating(signal.AuxiliaryValue),
            HodStatusCode.Hot => BuildHot(signal.Value),
            HodStatusCode.Cold => BuildCold(signal.Value),
            HodStatusCode.LiquidEncumbrance => BuildEncumbrance(signal.Value),
            HodStatusCode.NutritionDeficit => BuildNutritionDeficit(signal.Value),
            _ => string.Empty
        };
    }

    private static string BuildThirst(float percent)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "<font color=\"#e0cd8b\"><b>{0}</b></font>{1}",
            Lang.Get("hodvisualizer:status-thirst-title"),
            Lang.Get("hodvisualizer:status-thirst-body", percent));
    }

    private static string BuildSevereThirst(float _, float movePenaltyPercent)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "<font color=\"#d88952\"><b>{0}</b></font>{1}",
            Lang.Get("hodvisualizer:status-severe-thirst-title"),
            Lang.Get("hodvisualizer:status-severe-thirst-body", movePenaltyPercent));
    }

    private static string BuildDehydrating(float movePenaltyPercent)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "<font color=\"#d14a4a\"><b>{0}</b></font>{1}",
            Lang.Get("hodvisualizer:status-dehydrating-title"),
            Lang.Get("hodvisualizer:status-dehydrating-body", movePenaltyPercent));
    }

    private static string BuildHot(float increasedLossPercent)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "<font color=\"#d88952\"><b>{0}</b></font>{1}",
            Lang.Get("hodvisualizer:status-hot-title"),
            Lang.Get("hodvisualizer:status-hot-body", increasedLossPercent));
    }

    private static string BuildCold(float reducedLossPercent)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "<font color=\"#6fbadf\"><b>{0}</b></font>{1}",
            Lang.Get("hodvisualizer:status-cold-title"),
            Lang.Get("hodvisualizer:status-cold-body", reducedLossPercent));
    }

    private static string BuildEncumbrance(float penaltyPercent)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "<font color=\"#d88952\"><b>{0}</b></font>{1}",
            Lang.Get("hodvisualizer:status-encumbrance-title"),
            Lang.Get("hodvisualizer:status-encumbrance-body", penaltyPercent));
    }

    private static string BuildNutritionDeficit(float value)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "<font color=\"#d8bf73\"><b>{0}</b></font>{1}",
            Lang.Get("hodvisualizer:status-nutrition-deficit-title"),
            Lang.Get("hodvisualizer:status-nutrition-deficit-body", value));
    }
}
