namespace CampusSafetyHub;

// Bridge chuyen tiep canh bao sang cho Mediator
public sealed class EmergencyBridgeObserver : IAlertObserver
{
    private readonly IEmergencyMediator mediator;

    public EmergencyBridgeObserver(IEmergencyMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public void Update(AlertEvent alertEvent)
    {
        // Bug 2: Kich hoat khan cap ca khi chi la WARNING!
        // Le ra chi duoc kich hoat khi severity == "CRITICAL"
        // TODO: can kiem tra lai xem WARNING co can bat coi bao chay khong?
        if (alertEvent.Severity != null)
        {
            mediator.Notify("FIRE_DETECTED");
        }
    }
}
