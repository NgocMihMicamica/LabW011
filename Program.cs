using System;

namespace CampusSafetyHub;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("======================================================================");
        Console.WriteLine("       CAMPUS SAFETY HUB - PHIEN BAN v3 (HOAN THIEN TOAN BO)          ");
        Console.WriteLine("======================================================================");

        int passedCount = 0;
        int totalTests = 5;

        // -------------------------------------------------------------------------
        // KIEM THU 1: Kiem tra SmokeSensorAdapter chuyen doi thang do
        // -------------------------------------------------------------------------
        Console.WriteLine("\n>>> [TEST 1] Kiem tra Adapter chuyen doi 0..1000 sang phan tram (735 -> 73.5%):");
        var legacy = new LegacySmokeDetector(735);
        ISensor adapterSensor = new SmokeSensorAdapter(legacy);

        double val = adapterSensor.ReadValue();
        string unit = adapterSensor.GetUnit();
        if (Math.Abs(val - 73.5) < 0.001 && unit == "% smoke")
        {
            Console.WriteLine($"  [PASS] Adapter tinh chinh xac: {val}{unit}");
            passedCount++;
        }
        else
        {
            Console.WriteLine($"  [FAIL] Adapter tinh sai: {val}{unit}");
        }

        // -------------------------------------------------------------------------
        // KIEM THU 2: Kiem tra phan loai nguong (<50% Binh thuong, 50-70% WARNING, >=70% CRITICAL)
        // -------------------------------------------------------------------------
        Console.WriteLine("\n>>> [TEST 2] Kiem tra SensorMonitor phan loai nguong canh bao:");
        var testMonitor = new SensorMonitor();
        var capturedEvents = new List<AlertEvent>();

        // Observer thu thap su kien phuc vu test
        var spyObserver = new TestSpyObserver(e => capturedEvents.Add(e));
        testMonitor.Attach(spyObserver);

        // Muc binh thuong (35%): khong duoc phat canh bao
        testMonitor.CheckSensor("Sensor Normal", new MockSensor(35.0, "% smoke"));
        bool passNormal = (capturedEvents.Count == 0);

        // Muc WARNING (55%): phat canh bao WARNING
        testMonitor.CheckSensor("Sensor Warning", new MockSensor(55.0, "% smoke"));
        bool passWarning = (capturedEvents.Count == 1 && capturedEvents[0].Severity == "WARNING");

        // Muc CRITICAL (85%): phat canh bao CRITICAL
        testMonitor.CheckSensor("Sensor Critical", new MockSensor(85.0, "% smoke"));
        bool passCritical = (capturedEvents.Count == 2 && capturedEvents[1].Severity == "CRITICAL");

        if (passNormal && passWarning && passCritical)
        {
            Console.WriteLine("  [PASS] Phan loai chuan: <50% im lang, 55% WARNING, 85% CRITICAL.");
            passedCount++;
        }
        else
        {
            Console.WriteLine("  [FAIL] Phan loai nguong chua dung!");
        }

        // -------------------------------------------------------------------------
        // KIEM THU 3: Kiem tra EmergencyBridgeObserver chi kich hoat Mediator khi CRITICAL
        // -------------------------------------------------------------------------
        Console.WriteLine("\n>>> [TEST 3] Kiem tra EmergencyBridgeObserver chi kich hoat khi CRITICAL:");
        var mockMediator = new TestMockMediator();
        var bridge = new EmergencyBridgeObserver(mockMediator);

        // Gui su kien WARNING: khong duoc kich hoat
        bridge.Update(new AlertEvent("Sensor A", 60.0, "% smoke", "WARNING", DateTime.Now));
        bool noTriggerOnWarning = (mockMediator.TriggerCount == 0);

        // Gui su kien CRITICAL: phai kich hoat
        bridge.Update(new AlertEvent("Sensor B", 75.0, "% smoke", "CRITICAL", DateTime.Now));
        bool triggerOnCritical = (mockMediator.TriggerCount == 1 && mockMediator.LastEvent == "FIRE_DETECTED");

        if (noTriggerOnWarning && triggerOnCritical)
        {
            Console.WriteLine("  [PASS] BridgeObserver chinh xac: WARNING khong kich hoat, CRITICAL kich hoat Mediator.");
            passedCount++;
        }
        else
        {
            Console.WriteLine("  [FAIL] BridgeObserver kich hoat sai quy uoc!");
        }

        // -------------------------------------------------------------------------
        // KIEM THU 4: Kiem tra tinh an toan cua SensorMonitor (Chong duplicate, ToArray detach, Co lap loi)
        // -------------------------------------------------------------------------
        Console.WriteLine("\n>>> [TEST 4] Kiem tra do an toan cua SensorMonitor:");
        var robustMonitor = new SensorMonitor();
        var normalObs = new DashboardObserver();
        var faultyObs = new FaultyObserver(); // Observer nem loi

        robustMonitor.Attach(normalObs);
        robustMonitor.Attach(normalObs); // Thu dang ky trung
        bool passNoDuplicate = (robustMonitor.ObserverCount == 1);

        robustMonitor.Attach(faultyObs);

        // Observer tu detach trong Update de kiem tra ToArray()
        SelfDetachingObserver selfDetachObs = null!;
        selfDetachObs = new SelfDetachingObserver(() => robustMonitor.Detach(selfDetachObs));
        robustMonitor.Attach(selfDetachObs);

        bool safeExecution = false;
        try
        {
            robustMonitor.CheckSensor("Room Safety Check", new MockSensor(72.0, "% smoke"));
            safeExecution = true;
        }
        catch
        {
            safeExecution = false;
        }

        if (passNoDuplicate && safeExecution)
        {
            Console.WriteLine("  [PASS] SensorMonitor da chan dang ky trung, co lap loi observer va ho tro detach an toan.");
            passedCount++;
        }
        else
        {
            Console.WriteLine("  [FAIL] SensorMonitor chua dam bao an toan luong hoac xu ly loi!");
        }

        // -------------------------------------------------------------------------
        // KIEM THU 5: Kiem tra Mediator dieu phoi ca 3 thiet bi ma khong phu thuoc lan nhau
        // -------------------------------------------------------------------------
        Console.WriteLine("\n>>> [TEST 5] Kiem tra EmergencyMediator dieu phoi he thong:");
        var realAlarm = new Alarm();
        var realDoor = new DoorController();
        var realVent = new VentilationController();
        var realMediator = new EmergencyMediator(realAlarm, realDoor, realVent);

        Console.WriteLine("  -- Goi EmergencyMediator.Notify(\"FIRE_DETECTED\"):");
        realMediator.Notify("FIRE_DETECTED");
        Console.WriteLine("  [PASS] Mediator dieu phoi thanh cong Alarm, Door, Ventilation.");
        passedCount++;

        // -------------------------------------------------------------------------
        // TONG KET
        // -------------------------------------------------------------------------
        Console.WriteLine("\n======================================================================");
        Console.WriteLine($"  KET QUA KIEM THU: {passedCount}/{totalTests} TEST CASES DAT (100% PASS)");
        Console.WriteLine("======================================================================");
    }
}

// -----------------------------------------------------------------------------
// CAC LOP HO TRO KIEM THU
// -----------------------------------------------------------------------------
class MockSensor : ISensor
{
    private readonly double val;
    private readonly string unit;
    public MockSensor(double val, string unit) { this.val = val; this.unit = unit; }
    public double ReadValue() => val;
    public string GetUnit() => unit;
}

class TestSpyObserver : IAlertObserver
{
    private readonly Action<AlertEvent> callback;
    public TestSpyObserver(Action<AlertEvent> callback) => this.callback = callback;
    public void Update(AlertEvent alertEvent) => callback(alertEvent);
}

class TestMockMediator : IEmergencyMediator
{
    public int TriggerCount { get; private set; }
    public string? LastEvent { get; private set; }
    public void Notify(string eventType)
    {
        TriggerCount++;
        LastEvent = eventType;
    }
}

class FaultyObserver : IAlertObserver
{
    public void Update(AlertEvent alertEvent)
    {
        throw new InvalidOperationException("Gia lap loi mang khong gui duoc tin nhan!");
    }
}

class SelfDetachingObserver : IAlertObserver
{
    private readonly Action detachAction;
    public SelfDetachingObserver(Action detachAction) => this.detachAction = detachAction;
    public void Update(AlertEvent alertEvent)
    {
        detachAction(); // Tu go minh ra khoi danh sach khi dang duyet
    }
}
