using Godot;
using System;

public partial class InterfaceManager : CanvasLayer
{
    public static ProgressBar HealthBar;
    public static ProgressBar ManaBar;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        HealthBar = GetNode<ProgressBar>("MainInterface/HealthBar");
        ManaBar = GetNode<ProgressBar>("MainInterface/ManaBar");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public static void UpdateHealthBar(float health, float maxHealth)
    {
        HealthBar.Value = health / maxHealth * HealthBar.MaxValue;
    }

    public static void UpdateManaBar(float mana, float maxMana)
    {
        ManaBar.Value = mana / maxMana * ManaBar.MaxValue;
    }
}
