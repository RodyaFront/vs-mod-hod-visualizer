namespace HodVisualizer.Contracts;

public readonly record struct ThirstSnapshot(
    bool Available,
    float CurrentThirst,
    float MaxThirst,
    float ThirstRate,
    float? BaseThirstDecayRate,
    float NutritionDeficitAmount,
    int HydrationLossDelaySeconds);

public readonly record struct CoolingSnapshot(
    bool Available,
    float TotalCooling,
    float GearCooling,
    bool WetBonus,
    bool RoomBonus,
    bool LowSunBonus,
    bool ShadeBonus);

public readonly record struct EncumbranceSnapshot(
    bool Available,
    float WalkSpeedPenalty);

public readonly record struct HodPlayerSnapshot(
    bool PlayerReady,
    ThirstSnapshot Thirst,
    CoolingSnapshot Cooling,
    EncumbranceSnapshot Encumbrance);
