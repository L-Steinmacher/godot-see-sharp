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

    public void CastSpell()
    {
        Spell equippedSpell = (Spell)EquippedSpell.Instantiate() as Spell;
        equippedSpell.SetUp();
        // GameManager.
    }
}
