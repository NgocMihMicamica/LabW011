namespace CampusSafetyHub;

// Adapter giup bien doi cam bien cu sang chuan ISensor
public sealed class SmokeSensorAdapter : ISensor
{
    private readonly LegacySmokeDetector legacy;

    public SmokeSensorAdapter(LegacySmokeDetector legacy)
    {
        this.legacy = legacy ?? throw new ArgumentNullException(nameof(legacy));
    }

    public double ReadValue()
    {
        // Bug 1: Nham thang do chia cho 100 thay vi chia 10.0
        // Gia tri 735 bi bien thanh 7.35 thay vi 73.5%
        // TODO: can kiem tra lai xem chia 10 hay chia 100
        return legacy.GetSmokeLevel() / 100.0;
    }

    public string GetUnit() => "% smoke";
}
