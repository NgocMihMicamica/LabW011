namespace CampusSafetyHub;

// Doi tuong du lieu bat bien mo ta canh bao
public sealed record AlertEvent(
    string SensorName,
    double Value,
    string Unit,
    string Severity,
    DateTime OccurredAt);
