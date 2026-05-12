namespace HodVisualizer.Domain;

public readonly record struct StatusSignal(
    HodStatusCode Code,
    HodStatusSeverity Severity,
    float Value,
    float AuxiliaryValue = 0f,
    float? PulseMetric = null);
