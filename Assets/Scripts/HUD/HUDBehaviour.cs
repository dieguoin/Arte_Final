using Movement.Components;
using TMPro;
using UnityEngine;

public enum HUDProperties
{
    Limits,
    Health,
    Stamina,
    AdvancedAttack,
    AdvancedParry,
    AdvancedDodge,
    HealingCharges,
}

public class HUDBehaviour : MonoBehaviour, IObserver
{
    [SerializeField] private FighterMovement _player;

    private uint MaxHealth { get; set; }
    private uint MaxStamina { get; set; }
    private uint MaxAdvancedAttackCharges { get; set; }
    private float MaxAdvancedAttackPer100 { get; set; }
    private uint MaxAdvancedParryCharges { get; set; }
    private float MaxAdvancedParryPer100 { get; set; }
    private uint MaxAdvancedDodgeCharges { get; set; }
    private float MaxAdvancedDodgePer100 { get; set; }
    private uint MaxHealingCharges { get; set; }

    [SerializeField] private BarComponent HealthBar;
    [SerializeField] private BarComponent StaminaBar;
    [SerializeField] private ChargingComponent AdvancedAttack;
    [SerializeField] private ChargingComponent AdvancedParry;
    [SerializeField] private ChargingComponent AdvancedDodge;
    [SerializeField] private TMP_Text HealingCharges;

    // Start is called before the first frame update
    void Start()
    {
        MaxHealth = _player.MaxHealth;
        MaxStamina = _player.MaxStamina;
        MaxAdvancedAttackPer100 = 100;
        MaxAdvancedParryPer100 = 100;
        MaxAdvancedDodgePer100 = 100;
        UpdateState();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateState()
    {
        HealthBar.UpdateBarValue(_player.ActualHealth / MaxHealth);
        StaminaBar.UpdateBarValue(_player.ActualStamina / MaxStamina);
        HealingCharges.text = $"{_player.ActualHealingCharges}";
        AdvancedAttack.UpdateObjects(_player.AdvancedAttackProgress, _player.AdvancedAttackCharges);
        AdvancedParry.UpdateObjects(_player.AdvancedParryProgress, _player.AdvancedParryCharges);
        AdvancedDodge.UpdateObjects(_player.AdvancedDodgeProgress, _player.AdvancedDodgeCharges);
    }
}
