using System;
using System.Text;
using System.Text.Json;
using HodVisualizer.Contracts;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace HodVisualizer.Adapters;

internal sealed class HodStateReader : IHodStateReader
{
    private const string ConfigPath = "HydrateOrDiedrateConfig.json";
    private const long ConfigRefreshIntervalMs = 5000L;
    private const float DefaultBaseThirstRate = 10f;
    private const float DefaultMaxThirst = 1500f;

    private long _nextConfigRefreshMs;
    private float? _cachedBaseThirstRate;

    public HodPlayerSnapshot Read(ICoreClientAPI capi)
    {
        IClientPlayer? player = capi.World.Player;
        if (player?.Entity == null)
        {
            return new HodPlayerSnapshot(false, default, default, default);
        }

        float? baseThirstDecayRate = ResolveBaseThirstDecayRate(capi);
        ThirstSnapshot thirst = ReadThirst(player.Entity, baseThirstDecayRate);
        CoolingSnapshot cooling = ReadCooling(player.Entity);
        EncumbranceSnapshot encumbrance = ReadEncumbrance(player.Entity);

        return new HodPlayerSnapshot(true, thirst, cooling, encumbrance);
    }

    private float? ResolveBaseThirstDecayRate(ICoreClientAPI capi)
    {
        long now = capi.World.ElapsedMilliseconds;
        if (now < _nextConfigRefreshMs)
        {
            return _cachedBaseThirstRate;
        }

        _nextConfigRefreshMs = now + ConfigRefreshIntervalMs;

        try
        {
            string encoded = capi.World.Config.GetString(ConfigPath);
            if (string.IsNullOrWhiteSpace(encoded))
            {
                _cachedBaseThirstRate = null;
                return null;
            }

            string json = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            using JsonDocument doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("Thirst", out JsonElement thirstObj))
            {
                _cachedBaseThirstRate = null;
                return null;
            }

            if (!thirstObj.TryGetProperty("ThirstDecayRate", out JsonElement decayRateNode))
            {
                _cachedBaseThirstRate = null;
                return null;
            }

            if (decayRateNode.ValueKind != JsonValueKind.Number)
            {
                _cachedBaseThirstRate = null;
                return null;
            }

            float value = decayRateNode.GetSingle();
            _cachedBaseThirstRate = value > 0f && float.IsFinite(value) ? value : DefaultBaseThirstRate;
            return _cachedBaseThirstRate;
        }
        catch
        {
            _cachedBaseThirstRate = null;
            return null;
        }
    }

    private static ThirstSnapshot ReadThirst(Entity entity, float? baseThirstDecayRate)
    {
        ITreeAttribute? thirstTree = entity.WatchedAttributes.GetTreeAttribute("thirst");
        if (thirstTree == null)
        {
            return new ThirstSnapshot(false, 0f, 0f, 0f, baseThirstDecayRate, 0f, 0);
        }

        float maxThirst = thirstTree.GetFloat("maxThirst", DefaultMaxThirst);
        if (!float.IsFinite(maxThirst) || maxThirst <= 0f)
        {
            maxThirst = DefaultMaxThirst;
        }

        float currentThirst = GameMath.Clamp(thirstTree.GetFloat("currentThirst", maxThirst), 0f, maxThirst);
        float thirstRate = thirstTree.GetFloat("thirstRate", baseThirstDecayRate ?? DefaultBaseThirstRate);
        float nutritionDeficit = Math.Max(0f, thirstTree.GetFloat("nutritionDeficitAmount", 0f));
        int hydrationLossDelay = Math.Max(0, thirstTree.GetInt("hydrationLossDelay", 0));

        return new ThirstSnapshot(
            true,
            currentThirst,
            maxThirst,
            thirstRate,
            baseThirstDecayRate,
            nutritionDeficit,
            hydrationLossDelay);
    }

    private static CoolingSnapshot ReadCooling(Entity entity)
    {
        ITreeAttribute? coolingTree = entity.WatchedAttributes.GetTreeAttribute("hodCooling");
        if (coolingTree == null)
        {
            return new CoolingSnapshot(false, 0f, 0f, false, false, false, false);
        }

        return new CoolingSnapshot(
            true,
            coolingTree.GetFloat("totalCooling", 0f),
            coolingTree.GetFloat("gearCooling", 0f),
            coolingTree.GetInt("wetBonus", 0) != 0,
            coolingTree.GetInt("roomBonus", 0) != 0,
            coolingTree.GetInt("lowSunBonus", 0) != 0,
            coolingTree.GetInt("shadeBonus", 0) != 0);
    }

    private static EncumbranceSnapshot ReadEncumbrance(Entity entity)
    {
        try
        {
            if (entity.Stats["walkspeed"].ValuesByKey.TryGetValue("liquidEncumbrancePenalty", out EntityStat<float> mod))
            {
                return new EncumbranceSnapshot(true, GameMath.Max(0f, -mod.Value));
            }
        }
        catch
        {
            // Keep snapshot resilient when stats are not initialized yet.
        }

        return new EncumbranceSnapshot(false, 0f);
    }
}
