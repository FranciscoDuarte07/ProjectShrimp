using UnityEngine;

public abstract class CombatMode 
{
    public PlayerController player;

    public CombatMode(PlayerController _player)
    {
        this.player = _player;
    }

    public abstract void OnEnter();

    public abstract void OnExit();

    public abstract void OnUpdate();

    public abstract void PrimaryAction();

    public abstract string ModeName { get; }
}
