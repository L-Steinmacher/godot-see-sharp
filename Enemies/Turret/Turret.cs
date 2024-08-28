using Godot;
using System;


public partial class Turret : Enemy
{
    private double attackCooldown = 1.5;
    private double attackCooldownReset = 1.5;
    private bool isAttacking = false;
    private bool active;
    private Sprite2D sprite;
    private Sprite2D turretHead;
    private PlayerController player;
    [Export]
    public PackedScene projectileScene;

    public override void _Ready()
    {
        Health = 1;
        DamageDealtAmount = 1;
        sprite = GetNode<Sprite2D>("Sprite2D");
        turretHead = GetNode<Sprite2D>("TurretHeadSprite2D");
        projectileScene = (PackedScene)ResourceLoader.Load("res://Enemies/Turret/Projectile.tscn");
    }


    public override void _Process(double delta)
    {
        ProcessTimers(delta);
        ProcessAttack();
    }

    public override void TakeDamage(int DamageAmount)
    {
        Health -= DamageAmount;
        if (Health <= 0)
        {
            QueueFree();
        }
    }

    private void ProcessAttack()
    {
        if (active)
        {
            var angle = GlobalPosition.AngleToPoint(player.GlobalPosition);
            bool playerInFrontOfTurret = Mathf.Abs(angle) > Mathf.Pi / 2;
            // if (Mathf.Abs(angle) > Mathf.Pi / 2)
            // {
            //     sprite.FlipH = false;
            //     turretHead.FlipH = false;
            // }
            // else
            // {
            //     sprite.FlipH = true;
            //     turretHead.FlipH = true;
            // }
            if (playerInFrontOfTurret)
            {
                turretHead.FlipH = playerInFrontOfTurret;
                turretHead.FlipV = playerInFrontOfTurret;
                //  TODO have the head slowly track the player and stop tracking when about to fire and wait till fire animation is finished.
                turretHead.LookAt(player.GlobalPosition);
            }
            if (playerInFrontOfTurret && !isAttacking)
            {
                // Godot Intersect Ray requires this layout. SO DONT CHANGE IT!!!
                var queryParameters = new PhysicsRayQueryParameters2D
                {
                    From = Position,
                    To = player.Position,
                    Exclude = new Godot.Collections.Array<Rid> { GetRid() }
                };

                var spaceState = GetWorld2D().DirectSpaceState;
                Godot.Collections.Dictionary result = spaceState.IntersectRay(queryParameters);
                if (result != null && result.ContainsKey("collider"))
                {
                    Marker2D projectileSpawn = GetNode<Marker2D>("ProjectileSpawn");

                    Node2D collider = result["collider"].As<Node2D>();
                    if (collider is PlayerController)
                    {

                        projectileSpawn.LookAt(player.Position);
                        Projectile projectile = (Projectile)projectileScene.Instantiate();
                        Owner.AddChild(projectile);
                        projectile.shooter = this;


                        Vector2 direction = (player.GlobalPosition - projectileSpawn.GlobalPosition).Normalized();
                        projectile.velocity = direction * projectile.speed;
                        projectile.GlobalTransform = projectileSpawn.GlobalTransform;
                        // GD.Print("Pew pew X: " + projectile.Transform.X);
                        isAttacking = true;
                    }
                }
            }
        }
    }

    private void ProcessTimers(double delta)
    {
        if (isAttacking)
        {
            attackCooldown -= delta;
            if (attackCooldown <= 0)
            {
                isAttacking = false;
                attackCooldown = attackCooldownReset;
            }
        }
    }
    private void _on_detection_radius_body_entered(Node2D body)
    {
        // GD.Print("body: " + body.Name + " has entered the detection radious");
        if (body is PlayerController)
        {
            player = body as PlayerController;
            active = true;
        }
    }

    private void _on_detection_radius_body_exited(Node2D body)
    {
        // GD.Print("body: " + body.Name + " has exited the detection radious");
        if (body is PlayerController)
        {
            active = false;
        }
    }
}
