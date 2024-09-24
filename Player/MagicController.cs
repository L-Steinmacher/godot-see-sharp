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

    public MagicController()
    {
        PackedScene meleAttackScene = (PackedScene)ResourceLoader.Load(meleAttack.LoadResourcePath());
        PackedScene projectileAttackScene = (PackedScene)ResourceLoader.Load(projectileAttack.LoadResourcePath());
        AvSpells.Add(meleAttackScene);
        AvSpells.Add(projectileAttackScene);
        EquippedSpell = AvSpells[0];
        equipedMele = AvSpells[0];
    }

    public void CastSpell(bool facingDirection)
    {
        // TODO ADD CHECK TO SEE IS THERE IS ENOUGH MANA TO CAST SPELL
        Spell equippedSpell = (Spell)EquippedSpell.Instantiate();
        if (GameManager.Player.mana >= equippedSpell.ManaCost)
        {
            string rayCastDirection = facingDirection ? "SpellCastLeft" : "SpellCastRight";
            equippedSpell.GlobalPosition = GameManager.Player.GetNode<Marker2D>(rayCastDirection).GlobalPosition;
            GameManager.GlobalGameManager.AddChild(equippedSpell);
            equippedSpell.CastSpell(facingDirection);
            GameManager.Player.UpdateMana(-equippedSpell.ManaCost);
        }
        else
        {
            GD.Print("uhhh ohhh no mana!");
        }
    }

    public void MeleCast(bool faceDirection, MeleAttack.CastAnimation cast)
    {
        MeleAttack mele = (MeleAttack)equipedMele.Instantiate();
        string rayCastDirection = faceDirection ? "SpellCastLeft" : "SpellCastRight";
        mele.GlobalPosition = GameManager.Player.GetNode<Marker2D>(rayCastDirection).GlobalPosition;
        GameManager.GlobalGameManager.AddChild(mele);
        mele.MeleCast(faceDirection, cast);
    }

    public void CycleAttack()
    {
        int curIndex = AvSpells.IndexOf(EquippedSpell);
        curIndex += 1;

        if (curIndex >= AvSpells.Count)
        {
            curIndex = 0;
        }
        EquippedSpell = AvSpells[curIndex];
    }
}
