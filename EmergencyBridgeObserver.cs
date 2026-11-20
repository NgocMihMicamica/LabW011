namespace CampusSafetyHub;

// Da sua o phien ban v2:
// - Chi kich hoat Mediator xu ly khan cap khi muc do dat nguong "CRITICAL" (>= 70%)
// - Muc WARNING (50..69.9%) chi dung o muc phat thong bao, khong kich hoat chuong bao chay
public sealed class EmergencyBridgeObserver : IAlertObserver
{
    private readonly IEmergencyMediator mediator;

    public EmergencyBridgeObserver(IEmergencyMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public void Update(AlertEvent alertEvent)
    {
        // Da sua: chi khi su kien la CRITICAL moi kich hoat ung pho khan cap
        if (alertEvent.Severity == "CRITICAL")
        {
            mediator.Notify("FIRE_DETECTED");
        }
    }
}
