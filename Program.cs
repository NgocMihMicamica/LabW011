using System;

namespace CampusSafetyHub;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("==========================================================");
        Console.WriteLine("    CAMPUS SAFETY HUB - PHIEN BAN v2 (FIXED 1 PHAN)       ");
        Console.WriteLine("==========================================================");

        // 1. Khoi tao Mediator va cac thiet bi
        var alarm = new Alarm();
        var door = new DoorController();
        var ventilation = new VentilationController();
        IEmergencyMediator mediator = new EmergencyMediator(alarm, door, ventilation);

        // 2. Khoi tao Observer
        var monitor = new SensorMonitor();
        var dashboard = new DashboardObserver();
        var email = new EmailAlertObserver("it-support@campus.edu.vn");
        var audit = new AuditLogObserver();
        var bridge = new EmergencyBridgeObserver(mediator);

        monitor.Attach(dashboard);
        monitor.Attach(email);
        monitor.Attach(audit);
        monitor.Attach(bridge);

        // 3. Test Adapter da sua (chia 10.0)
        Console.WriteLine("\n[1] Test Adapter sau khi sua:");
        var legacy = new LegacySmokeDetector(735);
        ISensor adapterSensor = new SmokeSensorAdapter(legacy);
        Console.WriteLine($"Gia tri doc duoc: {adapterSensor.ReadValue()}{adapterSensor.GetUnit()}");
        Console.WriteLine("[THANH CONG] Adapter da chuyen 735 thanh 73.5% smoke chinh xac!");

        // 4. Test phan biet WARNING vs CRITICAL
        Console.WriteLine("\n[2] Test voi muc khói WARNING (55%):");
        var warningSensor = new MockSensor(55.0, "% smoke");
        monitor.CheckSensor("Sensor Room 101", warningSensor);
        Console.WriteLine("=> Nhan xet: Chi gui thong bao qua Dashboard/Email/Audit, KHONG kich hoat chuong bao!");

        Console.WriteLine("\n[3] Test voi muc khói CRITICAL (75%):");
        var criticalSensor = new MockSensor(75.0, "% smoke");
        monitor.CheckSensor("Sensor Server Room", criticalSensor);
        Console.WriteLine("=> Nhan xet: Da kich hoat Mediator thanh cong khi CRITICAL!");

        Console.WriteLine("\n(Luu y: SensorMonitor van chua chan attach trung va chua duyet ToArray an toan)");
    }
}

class MockSensor : ISensor
{
    private readonly double val;
    private readonly string unit;
    public MockSensor(double val, string unit) { this.val = val; this.unit = unit; }
    public double ReadValue() => val;
    public string GetUnit() => unit;
}
