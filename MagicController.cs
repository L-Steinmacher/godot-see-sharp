using Godot;
using System;
using System.Collections.Generic;

public partial class MagicController : Node
{
    public PackedScene EquippedSpell;
    public List<PackedScene> AvSpells = new List<PackedScene>();
    public MeleAttack meleAttack = new MeleAttack();

    // Called when the node enters the scene tree for the first time.

    public MagicController()
    {
        PackedScene meleAttackScene = (PackedScene)ResourceLoader.Load(meleAttack.LoadResourcePath());
        AvSpells.Add(meleAttackScene);
        EquippedSpell = AvSpells[0];
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void CastSpell(bool facingDirection)
    {
        Spell equippedSpell = (Spell)EquippedSpell.Instantiate();
        GD.Print("Castig " + equippedSpell.Name);

        if (!facingDirection)
        {
            equippedSpell.GlobalPosition = GameManager.Player.GetNode<Marker2D>("SpellCastRight").GlobalPosition;
        }
        else
        {
            equippedSpell.GlobalPosition = GameManager.Player.GetNode<Marker2D>("SpellCastLeft").GlobalPosition;
        }
        GameManager.GlobalGameManager.AddChild(equippedSpell);

        equippedSpell.CastSpell(facingDirection);
        GameManager.Player.UpdateMana(-equippedSpell.ManaCost);
    }
}
