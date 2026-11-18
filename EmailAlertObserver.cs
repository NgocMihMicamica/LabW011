namespace CampusSafetyHub;

public sealed class EmailAlertObserver : IAlertObserver
{
    private readonly string recipient;

    public EmailAlertObserver(string recipient)
    {
        this.recipient = recipient;
    }

    public void Update(AlertEvent alertEvent)
    {
        Console.WriteLine(
            $"[EMAIL -> {recipient}] CANH BAO {alertEvent.Severity}: " +
            $"{alertEvent.SensorName} dat {alertEvent.Value:F1}{alertEvent.Unit} " +
            $"luc {alertEvent.OccurredAt:HH:mm:ss}");
    }
}
