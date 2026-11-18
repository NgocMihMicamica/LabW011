namespace CampusSafetyHub;

// Giao dien chuan ma module giam sat su dung
public interface ISensor
{
    double ReadValue();
    string GetUnit();
}
