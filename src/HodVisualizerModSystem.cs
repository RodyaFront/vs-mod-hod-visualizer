using System;
using System.Globalization;
using HodVisualizer.Adapters;
using HodVisualizer.Domain;
using HodVisualizer.Domain.Evaluators;
using PlayerStatusStrip;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace HodVisualizer;

public sealed class HodVisualizerModSystem : ModSystem
{
    private ICoreClientAPI? _clientApi;
    private IStatusStripHudApi? _statusApi;
    private HodStatusStripProvider? _statusProvider;
    private Action? _onLeftWorld;
    private Action? _onLevelFinalize;
    private readonly IHodStateReader _stateReader = new HodStateReader();
    private readonly HeatColdEvaluator _heatColdEvaluator = new();

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        _clientApi = api;

        _onLeftWorld = UnregisterStatusProvider;
        api.Event.LeftWorld += _onLeftWorld;

        _onLevelFinalize = () => TryRegisterStatusProvider(api);
        api.Event.LevelFinalize += _onLevelFinalize;

        TryRegisterStatusProvider(api);
        RegisterDebugCommand(api);
    }

    private bool TryRegisterStatusProvider(ICoreClientAPI api)
    {
        if (_statusProvider != null)
        {
            return true;
        }

        if (!api.ModLoader.IsModEnabled("hydrateordiedrate"))
        {
            api.Logger.Warning("[HOD Visualizer] 'hydrateordiedrate' is not enabled. Provider registration skipped.");
            return false;
        }

        if (!api.ModLoader.IsModEnabled("playerstatusstrip"))
        {
            api.Logger.Warning("[HOD Visualizer] 'playerstatusstrip' is not enabled. Provider registration skipped.");
            return false;
        }

        PlayerStatusStripModSystem? stripSystem = api.ModLoader.GetModSystem<PlayerStatusStripModSystem>();
        _statusApi = stripSystem?.StatusApi;
        if (_statusApi == null)
        {
            api.Logger.Warning("[HOD Visualizer] PlayerStatusStrip API is unavailable. Provider registration skipped.");
            return false;
        }

        _statusProvider = new HodStatusStripProvider();
        _statusApi.RegisterProvider(_statusProvider);
        api.Logger.Notification("[HOD Visualizer] Registered status provider in PlayerStatusStrip.");
        return true;
    }

    private void UnregisterStatusProvider()
    {
        if (_statusApi != null && _statusProvider != null)
        {
            _statusApi.UnregisterProvider(_statusProvider);
        }

        _statusProvider = null;
        _statusApi = null;
    }

    private void RegisterDebugCommand(ICoreClientAPI api)
    {
        api.ChatCommands
            .Create("hodvizdebug")
            .WithDescription("Print HOD Visualizer debug values for hot/cold logic.")
            .HandleWith(_ => BuildDebugSnapshotResult(api));
    }

    private TextCommandResult BuildDebugSnapshotResult(ICoreClientAPI api)
    {
        var snapshot = _stateReader.Read(api);
        if (!snapshot.PlayerReady)
        {
            return TextCommandResult.Error("[HOD Visualizer] Player is not ready yet.");
        }

        bool hasHeatSignal = _heatColdEvaluator.TryEvaluate(snapshot, out StatusSignal heatSignal);
        float baseRate = snapshot.Thirst.BaseThirstDecayRate ?? 0f;
        float rateDeltaPercent = baseRate > 0f
            ? ((snapshot.Thirst.ThirstRate / baseRate) - 1f) * 100f
            : float.NaN;

        string message = string.Format(
            CultureInfo.InvariantCulture,
            "[HOD Visualizer] hot/cold debug\n" +
            "cooling: total={0:0.###}, gear={1:0.###}, wet={2}, room={3}, lowSun={4}, shade={5}\n" +
            "thirst: current={6:0.###}, max={7:0.###}, rate={8:0.###}, base={9:0.###}, delta={10:0.###}%\n" +
            "result: hasSignal={11}, code={12}, value={13:0.###}, severity={14}",
            snapshot.Cooling.TotalCooling,
            snapshot.Cooling.GearCooling,
            snapshot.Cooling.WetBonus ? 1 : 0,
            snapshot.Cooling.RoomBonus ? 1 : 0,
            snapshot.Cooling.LowSunBonus ? 1 : 0,
            snapshot.Cooling.ShadeBonus ? 1 : 0,
            snapshot.Thirst.CurrentThirst,
            snapshot.Thirst.MaxThirst,
            snapshot.Thirst.ThirstRate,
            baseRate,
            rateDeltaPercent,
            hasHeatSignal ? 1 : 0,
            hasHeatSignal ? heatSignal.Code.ToString() : "none",
            hasHeatSignal ? heatSignal.Value : 0f,
            hasHeatSignal ? heatSignal.Severity.ToString() : "none");

        return TextCommandResult.Success(message);
    }

    public override void Dispose()
    {
        UnregisterStatusProvider();

        if (_clientApi != null)
        {
            if (_onLeftWorld != null)
            {
                _clientApi.Event.LeftWorld -= _onLeftWorld;
            }

            if (_onLevelFinalize != null)
            {
                _clientApi.Event.LevelFinalize -= _onLevelFinalize;
            }
        }

        _onLeftWorld = null;
        _onLevelFinalize = null;
        _clientApi = null;

        base.Dispose();
    }
}
