using Godot;
using System;

public abstract partial class Spell : Node2D
{
    private string ResourcePath;
    [Export]
    public int DamageAmount;
    [Export]
    public float ManaCost;
    [Export]
    public int Speed;
    public abstract void CastSpell();
    public abstract void LoadResourcePath();
    public abstract void SetUp();
}
