using UnityEngine.UI;

public interface IHasStatus
{
    Status Status{get;}
    int TakeDamage(Status attackerStatus);
    void SetHPSlider(Slider slider);
    void InitStatus();
}