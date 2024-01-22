using Godot;
using System;

public abstract partial class Spell : Node2D
{

    public bool facingDirection;
    [Export]
    public int DamageAmount;
    [Export]
    public float ManaCost;
    [Export]
    public int Speed;
    public abstract void CastSpell(bool faceDirection);
    public abstract string LoadResourcePath();
    public abstract void SetUp(bool faceDirection);
}
