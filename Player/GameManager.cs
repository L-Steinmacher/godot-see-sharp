using Godot;
using System;

public partial class GameManager : Node2D
{
    [Export]
    public Marker2D RespawnPoint;
    public static GameManager GlobalGameManager;
    public static PlayerController Player;
    public static MagicController MagicController;

    public override void _Ready()
    {
        if (GlobalGameManager == null)
        {
            GlobalGameManager = this;
        }
        else
        {
            QueueFree();
        }
        MagicController = new MagicController();
    }

    public override void _Process(double delta)
    {
    }

    public void RespawnPlayer()
    {
        PlayerController pc = GetNode<PlayerController>("Player");
        pc.GlobalPosition = RespawnPoint.GlobalPosition;
        pc.RespawnPlayer();
    }

    private void _on_player_death()
    {
        RespawnPlayer();
    }
}
