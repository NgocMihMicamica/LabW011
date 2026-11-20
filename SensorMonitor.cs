namespace CampusSafetyHub;

// Da hoan thien o phien ban v3:
// 1. Chan dang ky trung lap (Khong cho cung mot observer dang ky hai lan).
// 2. Ho tro Detach de huy dang ky observer.
// 3. Duyet tren ban sao observers.ToArray() de tranh loi 'Collection was modified' khi observer detach giua chung.
// 4. Co lap loi: boc try-catch de mot observer loi khong lam anh huong den cac observer khac.
public sealed class SensorMonitor
{
    private readonly List<IAlertObserver> observers = new();

    public void Attach(IAlertObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        // Da sua: Chan khong cho cung 1 observer dang ky 2 lan
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void Detach(IAlertObserver observer)
    {
        if (observer != null)
        {
            observers.Remove(observer);
        }
    }

    public void CheckSensor(string sensorName, ISensor sensor)
    {
        ArgumentNullException.ThrowIfNull(sensor);

        double value = sensor.ReadValue();

        // Xac dinh muc do dua tren nguong quy uoc
        string? severity = value switch
        {
            >= 70 => "CRITICAL",
            >= 50 => "WARNING",
            _ => null // Duoi 50% la binh thuong, khong phat canh bao
        };

        if (severity is null)
        {
            return;
        }

        // AlertEvent la record bat bien, dam bao khong observer nao sua duoc du lieu goc
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
        // Da sua: Duyet tren ban sao ToArray() de mot observer co the tu huy dang ky (detach)
        // trong khi dang nhan thong bao ma khong lam hong vong lap foreach.
        foreach (IAlertObserver observer in observers.ToArray())
        {
            try
            {
                observer.Update(alertEvent);
            }
            catch (Exception ex)
            {
                // Da sua: Mot observer bi loi khong duoc lam hong cac observer khac
                Console.WriteLine($"[MONITOR WARNING] Observer '{observer.GetType().Name}' gap loi khi nhan canh bao: {ex.Message}");
            }
        }
    }

    // Phuong thuc ho tro kiem thu so luong observer hien tai
    public int ObserverCount => observers.Count;
}
