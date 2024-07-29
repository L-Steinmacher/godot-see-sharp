using Godot;
using System;

public abstract partial class Enemy : CharacterBody2D
{
    [Export]
	public int Health;

    [Export]
    public int DamageDealtAmount;
    public bool FacingDirection;
    public abstract void TakeDamage(int DamageAmount);
}
