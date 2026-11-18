using System;

namespace CampusSafetyHub;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("==========================================================");
        Console.WriteLine("    CAMPUS SAFETY HUB - PHIEN BAN v1 (DRAFT / BUGGY)      ");
        Console.WriteLine("==========================================================");

        // 1. Khoi tao cac thanh phan Mediator
        var alarm = new Alarm();
        var door = new DoorController();
        var ventilation = new VentilationController();
        IEmergencyMediator mediator = new EmergencyMediator(alarm, door, ventilation);

        // 2. Khoi tao Observer
        var monitor = new SensorMonitor();
        var dashboard = new DashboardObserver();
        var email = new EmailAlertObserver("admin@campus.edu.vn");
        var audit = new AuditLogObserver();
        var bridge = new EmergencyBridgeObserver(mediator);

        // Thu attach (Bug: attach 2 lan khong bi chan)
        monitor.Attach(dashboard);
        monitor.Attach(dashboard); // Bug: bi trung
        monitor.Attach(email);
        monitor.Attach(audit);
        monitor.Attach(bridge);

        // 3. Test Adapter (Bug: dang chia cho 100 nen bi sai gia tri)
        Console.WriteLine("\n[1] Kiem tra Adapter voi gia tri 735:");
        var legacy = new LegacySmokeDetector(735);
        ISensor adapterSensor = new SmokeSensorAdapter(legacy);
        Console.WriteLine($"Gia tri doc duoc: {adapterSensor.ReadValue()}{adapterSensor.GetUnit()}");
        Console.WriteLine("(Bug: 735 dang bi tinh thanh 7.35% thay vi 73.5%!)");

        // 4. Test canh bao voi sensor gia lap
        Console.WriteLine("\n[2] Test voi muc khoi WARNING (55%):");
        var mockWarningSensor = new MockSensor(55.0, "% smoke");
        monitor.CheckSensor("Smoke Sensor Zone 1", mockWarningSensor);
        Console.WriteLine("(Bug: WARNING ma cung bi EmergencyBridgeObserver kich hoat chuong bao chay!)");
    }
}

// Sensor gia lap phuc vu test
class MockSensor : ISensor
{
    private readonly double val;
    private readonly string unit;
    public MockSensor(double val, string unit) { this.val = val; this.unit = unit; }
    public double ReadValue() => val;
    public string GetUnit() => unit;
}
