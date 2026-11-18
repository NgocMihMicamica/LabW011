namespace CampusSafetyHub;

// Subject quan ly observer, doc cam bien va phat canh bao
public sealed class SensorMonitor
{
    private readonly List<IAlertObserver> observers = new();

    public void Attach(IAlertObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        // Bug 3: Khong kiem tra trung lap! Neu dang ky 2 lan se bi them 2 lan
        // TODO: can chan khong cho add trung
        observers.Add(observer);
    }

    public void Detach(IAlertObserver observer)
    {
        observers.Remove(observer);
    }

    public void CheckSensor(string sensorName, ISensor sensor)
    {
        double value = sensor.ReadValue();

        string? severity = value switch
        {
            >= 70 => "CRITICAL",
            >= 50 => "WARNING",
            _ => null
        };

        if (severity is null)
        {
            return; // Duoi 50% la binh thuong, khong phat canh bao
        }

        var alertEvent = new AlertEvent(
            sensorName,
            value,
            sensor.GetUnit(),
            severity,
            DateTime.Now);

        Notify(alertEvent);
    }

    private void Notify(AlertEvent alertEvent)
    {
        // Bug 4: Duyet truc tiep tren observers ma khong dung ToArray()
        // Neu co observer nao goi Detach trong khi duyet thi se bi Exception (Collection was modified)
        // Dong thoi chua bat try-catch: neu 1 observer loi se lam chet cac observer sau
        foreach (IAlertObserver observer in observers)
        {
            observer.Update(alertEvent);
        }
    }
}
