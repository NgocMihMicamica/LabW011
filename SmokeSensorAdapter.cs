namespace CampusSafetyHub;

// Da sua o phien ban v2:
// - Chuyen doi dung thang do 0..1000 thanh 0..100.0 bang cach chia cho 10.0
// - Tranh loi chia so nguyen va dam bao do chinh xac so thuc
public sealed class SmokeSensorAdapter : ISensor
{
    private readonly LegacySmokeDetector legacy;

    public SmokeSensorAdapter(LegacySmokeDetector legacy)
    {
        this.legacy = legacy ?? throw new ArgumentNullException(nameof(legacy));
    }

    public double ReadValue()
    {
        // Da sua: chia cho 10.0 thay vi chia 100.0 (vi du: 735 / 10.0 = 73.5%)
        return legacy.GetSmokeLevel() / 10.0;
    }

    public string GetUnit() => "% smoke";
}
