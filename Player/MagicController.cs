using Godot;
using System;
using System.Collections.Generic;

public partial class MagicController : Node
{
    public PackedScene EquippedSpell;
    public List<PackedScene> AvSpells = new();
    public MeleAttack meleAttack = new();
    public Projectile projectileAttack = new();
    private PackedScene equipedMele;
    // Called when the node enters the scene tree for the first time.

    public MagicController()
    {
        PackedScene meleAttackScene = (PackedScene)ResourceLoader.Load(meleAttack.LoadResourcePath());
        PackedScene projectileAttackScene = (PackedScene)ResourceLoader.Load(projectileAttack.LoadResourcePath());
        AvSpells.Add(meleAttackScene);
        AvSpells.Add(projectileAttackScene);
        EquippedSpell = AvSpells[0];
        equipedMele = AvSpells[0];
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void CastSpell(bool facingDirection)
    {
        // TODO ADD CHECK TO SEE IS THERE IS ENOUGH MANA TO CAST SPELL
        Spell equippedSpell = (Spell)EquippedSpell.Instantiate();
        string rayCastDirection = facingDirection ? "SpellCastLeft" : "SpellCastRight";
        equippedSpell.GlobalPosition = GameManager.Player.GetNode<Marker2D>(rayCastDirection).GlobalPosition;
        GameManager.GlobalGameManager.AddChild(equippedSpell);
        equippedSpell.CastSpell(facingDirection);
        GameManager.Player.UpdateMana(-equippedSpell.ManaCost);
    }

    public void CycleAttack()
    {
        int curIndex = AvSpells.IndexOf(EquippedSpell);
        curIndex += 1;

        GD.Print(curIndex);
        if (curIndex >= AvSpells.Count)
        {
            GD.Print("Cycle Attack" + EquippedSpell.GetType().Name);
            curIndex = 0;
        }
        EquippedSpell = AvSpells[curIndex];
    }
}
