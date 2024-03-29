using Godot;
using System;

public partial class MagicPotion : Collectable
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Bounce");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void UsePotion()
    {
        GameManager.Player.UpdateMana(10);
        QueueFree();
    }

    public void _on_area_2d_body_entered(Node body)
    {
        if (body is PlayerController)
        {
            GetNode<RichTextLabel>("Node2D/RichTextLabel").Show();
        }
    }

    public void _on_area_2d_body_exited(Node body)
    {
        if (body is PlayerController)
        {
            GetNode<RichTextLabel>("Node2D/RichTextLabel").Hide();
        }
    }
}
