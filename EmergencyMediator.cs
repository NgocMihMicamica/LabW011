namespace CampusSafetyHub;

// Mediator dong vai tro trung tam dieu phoi cac thiet bi ung pho khan cap
public sealed class EmergencyMediator : IEmergencyMediator
{
    private readonly Alarm alarm;
    private readonly DoorController door;
    private readonly VentilationController ventilation;

    public EmergencyMediator(Alarm alarm, DoorController door, VentilationController ventilation)
    {
        this.alarm = alarm ?? throw new ArgumentNullException(nameof(alarm));
        this.door = door ?? throw new ArgumentNullException(nameof(door));
        this.ventilation = ventilation ?? throw new ArgumentNullException(nameof(ventilation));
    }

    public void Notify(string eventType)
    {
        if (eventType == "FIRE_DETECTED")
        {
            Console.WriteLine("[MEDIATOR] === DIEU PHOI KHAN CAP: PHAT HIEN HOA HOAN ===");
            alarm.SoundAlarm();
            door.UnlockAllDoors();
            ventilation.StartExhaust();
        }
    }
}
