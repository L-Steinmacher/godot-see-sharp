using Godot;
using System;

public abstract partial class Spell : Node2D
{
    public bool facingDirection;
    private string ResourcePath;
    [Export]
    public int DamageAmount;
    [Export]
    public float ManaCost;
    [Export]
    public int Speed;
    public abstract void CastSpell(bool facingDirection);
    public abstract void LoadResourcePath();
    public abstract void SetUp(bool faceDirection);
}
