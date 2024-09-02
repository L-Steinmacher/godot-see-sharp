using Godot;
using System;


public partial class Turret : Enemy
{
    private enum TurretState
    {
        Idle,
        Tracking,
        Attacking,
        TakeDamage,
    }
    private TurretState currentState;
    private double attackCooldown = 1.5;
    private double attackCooldownReset = 1.5;
    private bool isAttacking = false;
    private bool active;
    private Sprite2D turretHeadSprite;
    private Node2D turretHeadNode;
    private float initialRotation;
    private PlayerController player;
    [Export]
    public PackedScene projectileScene;
    private RayCast2D targeting;

    public override void _Ready()
    {
        Health = 1;
        DamageDealtAmount = 1;
        turretHeadNode = GetNode<Node2D>("TurretHead");
        turretHeadSprite = turretHeadNode.GetNode<Sprite2D>("TurretHeadSprite2D");
        projectileScene = (PackedScene)ResourceLoader.Load("res://Enemies/Turret/Projectile.tscn");
        targeting = turretHeadNode.GetNode<RayCast2D>("RayCast2D");
        initialRotation = turretHeadNode.GlobalRotation;
        currentState = TurretState.Idle;
    }

    public override void _Process(double delta)
    {
        ProcessTimers(delta);
        switch (currentState)
        {
            case TurretState.Idle:
                turretHeadNode.GlobalRotation = Mathf.LerpAngle(turretHeadNode.GlobalRotation, initialRotation, 0.07f);
                break;
            case TurretState.Tracking:
                ProcessTracking();
                break;
            case TurretState.Attacking:
                ProcessAttack();
                break;
            case TurretState.TakeDamage:
                break;
        }
    }

    public override void TakeDamage(int DamageAmount)
    {
        Health -= DamageAmount;
        if (Health <= 0)
        {
            QueueFree();
        }
    }

    private void ProcessTracking()
    {
        if (active)
        {
            // get the angle of the player to the turret
            var angle = GlobalPosition.AngleToPoint(player.GlobalPosition);
            bool playerInFrontOfTurret = Mathf.Abs(angle) > Mathf.Pi / 2;
            // TODO This is jank and needs to be handled  by the node
            turretHeadSprite.FlipH = playerInFrontOfTurret;
            turretHeadSprite.FlipV = playerInFrontOfTurret;

            // slowly move the head towards the players position
            var r = turretHeadNode.GlobalRotation;
            turretHeadNode.GlobalRotation = (float)Mathf.LerpAngle(r, angle, .07);

            object target;
            bool isPlayer;
            if (targeting.IsColliding())
            {

                target = targeting.GetCollider();
                isPlayer = target.GetType().Name is "PlayerController";

                if (isPlayer && !isAttacking && playerInFrontOfTurret)
                {
                    currentState = TurretState.Attacking;
                }
            }
        }
        else
        {
            currentState = TurretState.Idle;
        }

    }

    private void ProcessAttack()
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
            Marker2D projectileSpawn = turretHeadNode.GetNode<Marker2D>("ProjectileSpawn");
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
                currentState = TurretState.Tracking;
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
        GD.Print("body: " + body.Name + " has entered the detection radious");
        if (body is PlayerController)
        {
            player = body as PlayerController;
            active = true;
            currentState = TurretState.Tracking;
        }
    }

    private void _on_detection_radius_body_exited(Node2D body)
    {
        // GD.Print("body: " + body.Name + " has exited the detection radious");
        if (body is PlayerController)
        {
            currentState = TurretState.Idle;
        }
    }
}
