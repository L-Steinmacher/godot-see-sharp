using Godot;
using System;

public partial class MagicController : Node
{
    public PackedScene EquippedSpell = ResourceLoader.Load<PackedScene>("res://Spells/MeleAttack.tscn");
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void CastSpell(bool facingDirection)
    {
        Spell equippedSpell = (Spell)EquippedSpell.Instantiate() as Spell;
        equippedSpell.SetUp(facingDirection);
        if (facingDirection)
        {
            equippedSpell.GlobalPosition = GameManager.Player.GetNode<Marker2D>("SpellCastRight").GlobalPosition;
        }
        else
        {
            equippedSpell.GlobalPosition = GameManager.Player.GetNode<Marker2D>("SpellCastLeft").GlobalPosition;
        }
        GameManager.GlobalGameManager.AddChild(equippedSpell);
        GameManager.Player.UpdateMana(-equippedSpell.ManaCost);
    }
}
