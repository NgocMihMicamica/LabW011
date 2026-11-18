namespace CampusSafetyHub;

// Hop dong nhan canh bao cho cac ben quan tam
public interface IAlertObserver
{
    void Update(AlertEvent alertEvent);
}
