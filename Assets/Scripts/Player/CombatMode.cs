using UnityEngine;

public abstract class CombatMode 
{
    public PlayerController player;
    
    protected DashData dashData;
    public float DashDistance => dashData.distance;
    public float DashDuration => dashData.duration;
    public float DashCooldown => dashData.cooldown;

    public CombatMode(PlayerController _player, DashData _dashData)
    {
        this.player = _player;
        dashData = _dashData;
    }

    public abstract void OnEnter();

    public abstract void OnExit();

    public abstract void OnUpdate();

    public abstract void PrimaryAction();

    public abstract void SpecialAction();

    public abstract string ModeName { get; }
}
