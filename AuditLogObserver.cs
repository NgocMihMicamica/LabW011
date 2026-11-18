namespace CampusSafetyHub;

public sealed class AuditLogObserver : IAlertObserver
{
    public void Update(AlertEvent alertEvent)
    {
        Console.WriteLine(
            $"[AUDIT LOG] Ghi nhan su kien {alertEvent.Severity} tu {alertEvent.SensorName} " +
            $"- Gia tri: {alertEvent.Value:F1}{alertEvent.Unit}");
    }
}
