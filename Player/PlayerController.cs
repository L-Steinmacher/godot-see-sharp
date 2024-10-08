using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
    public enum PlayerState
    {
        Idle,
        Running,
        Falling,
        Dashing,
        WallJumping,
        MeleAttacking,
        TakingDamage,
    }

    public PlayerState CurrentState = PlayerState.Idle;
    public Vector2 facingDirection = new(0, 0);
    private Vector2 velocity = new();
    private const float Acceleration = 0.2f;
    public const float DashGravity = 0.0f;
    private const float DashSpeed = 500.0f;
    private const float Friction = 0.3f;
    public float minJumpVelocity;
    public float maxJumpVelocity;
    public float minJumpHeight = Globals.UNIT_SIZE * .8f;
    public float maxJumpHeight = Globals.UNIT_SIZE * 5f;
    private float jumpDuration = 0.5f;
    private bool isFalling;
    private double fallTimer = 3.0;
    private double fallTimerReset = 3.0;
    public const float Speed = Globals.UNIT_SIZE * 10f;
    public const float WallJumpVerticalVelocity = Globals.UNIT_SIZE * -10;
    private bool isDashing = false;
    private bool canDash = true;
    private double dashTimer = .3;
    private double dashTimeReset = .3;
    private double dashCooldownTimer = 1;
    private double dashCooldownTimeReset = 0.7;
    private double damageTimer = .3;
    private double damageTimerReset = .3;
    public int attackCount = 0;
    public bool isAttacking = false;
    private float comboResetTime = 0.5f;
    private Timer comboTimer;
    private bool isWallJumping = false;
    private double wallJumpTimer = .3;
    private double wallJumpTimeReset = .3;
    private bool canDoubleJump = true;
    private bool isDoubleJumping = false;
    private bool isDead = false;
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    public float gravityReset = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private AnimatedSprite2D animatedSprite2D;
    public float health = 3;
    private float maxHealth = 3;
    private bool isTakingDammage = false;
    public float mana = 100f;
    private float maxMana = 100f;
    [Signal]
    public delegate void DeathEventHandler();
    [Export]
    public PackedScene GhostPlayerInstance;
    [Export]
    public PackedScene JumpEffectsInstance;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        GameManager.Player = this;
        minJumpVelocity = -MathF.Sqrt(2 * gravity * minJumpHeight);
        maxJumpVelocity = -MathF.Sqrt(2 * gravity * maxJumpHeight);
        comboTimer = new Timer();
        AddChild(comboTimer);
        comboTimer.WaitTime = comboResetTime;
        comboTimer.OneShot = true;
        comboTimer.Connect("timeout", new Callable(this, nameof(OnComboTimeout)));
        ;
    }

    public override void _PhysicsProcess(double delta)
    {
        InterfaceManager.UpdateHealthBar(health, maxHealth);
        InterfaceManager.UpdateManaBar(mana, maxMana);

        ProcessTimers(delta);
        if (CurrentState != PlayerState.TakingDamage)
        {
            InputManager(delta);
        }
        switch (CurrentState)
        {
            case PlayerState.MeleAttacking:
                if (!isAttacking)
                    // TODO handle sequence attacks with meleAttack
                    ProcessMeleAttack();
                // StartMeleeAttack();
                break;
            case PlayerState.Idle:
                ProcessIdle(ref velocity);
                break;
            case PlayerState.Running:
                break;
            case PlayerState.Dashing:
                Velocity = ProcessDash(velocity);
                break;
            case PlayerState.TakingDamage:
                break;
            default:
                break;
        }
    }

    private void InputManager(double delta)
    {
        if (health > 0)
        {
            velocity = Velocity;
            // Add the gravity.
            if (!IsOnFloor())
            {
                isFalling = true;
                velocity.Y += gravity * (float)delta;
            }

            if (IsOnFloor())
            {
                isFalling = false;
                fallTimer = fallTimerReset;
                isDoubleJumping = false;
                if (!isDashing && dashCooldownTimer <= 0)
                {
                    canDash = true;
                    dashCooldownTimer = dashCooldownTimeReset;
                }
            }

            if (velocity.X > 0)
            {
                animatedSprite2D.FlipH = false;
            }
            else if (velocity.X < 0)
            {
                animatedSprite2D.FlipH = true;
            }
            if (Input.IsActionJustPressed("attack"))
            {
                StartMeleeAttack();
            }

            if (!isAttacking)
            {

                if (Input.IsActionJustPressed("jump"))
                {
                    velocity = ProcessJump(velocity);
                }
                if (Input.IsActionJustReleased("jump") && velocity.Y < minJumpVelocity)
                {
                    velocity.Y = minJumpVelocity;
                }
                if (Input.IsActionJustPressed("dash"))
                {
                    CurrentState = PlayerState.Dashing;
                    // velocity = ProcessDash(delta, velocity, facingDirection);
                }
                if (Input.IsActionJustPressed("interact"))
                {
                    InteractWithObject();
                }

                if (Input.IsActionJustPressed("cycle_attack"))
                {
                    GameManager.MagicController.CycleAttack();
                }
                if (Input.IsActionJustPressed("open_menu"))
                {
                    /**
                    *TODO: This pauses the entire game and needs to pause a nested pause node so we can unpause!
                    */
                    if (GetTree().Paused)
                    {
                        GetTree().Paused = false; // Unpause the game
                        GD.Print("Game Unpaused");
                    }
                    else
                    {
                        GetTree().Paused = true; // Pause the game
                        GD.Print("Game Paused");
                    }
                }
                facingDirection = ProcessMovement(ref velocity);
                Velocity = velocity;
                MoveAndSlide();
            }
        }
    }

    private void StartMeleeAttack()
    {
        if (!comboTimer.IsStopped()) // Combo is active
        {
            attackCount = Mathf.Min(attackCount + 1, 3); // Limit to triple attack
        }
        else
        {
            attackCount = 1; // Start a new combo
        }

        comboTimer.Start(); // Reset combo timer
        ExecuteMeleeAttack();
    }

    private void ExecuteMeleeAttack()
    {
        isAttacking = true;
        bool faceDirection = animatedSprite2D.FlipH;

        switch (attackCount)
        {
            case 1:
                animatedSprite2D.Play("Attack");
                GameManager.MagicController.MeleCast(faceDirection, MeleAttack.CastAnimation.Single);
                break;
            case 2:
                animatedSprite2D.Play("Attack");
                GameManager.MagicController.MeleCast(faceDirection, MeleAttack.CastAnimation.Double);
                break;
            case 3:
                GameManager.MagicController.MeleCast(faceDirection, MeleAttack.CastAnimation.Tripple);
                OnComboTimeout();
                break;
            default:
                break;
        }
        isAttacking = false;
    }

    private void ProcessTimers(double delta)
    {
        if (isFalling)
        {

            fallTimer -= delta;
            if (fallTimer <= 0)
            {
                EmitSignal(nameof(Death));
                isFalling = false;
                fallTimer = fallTimerReset;
            }
        }
        if (isTakingDammage)
        {
            damageTimer -= delta;
            if (damageTimer <= 0)
            {
                CurrentState = PlayerState.Idle;
                damageTimer = damageTimerReset;
                isTakingDammage = false;
            }
        }
        if (isWallJumping)
        {
            wallJumpTimer -= delta;
            if (wallJumpTimer <= 0)
            {
                isWallJumping = false;
                wallJumpTimer = wallJumpTimeReset;
            }
        }
        if (isDashing)
        {
            dashTimer -= delta;
            dashCooldownTimer -= delta;
            GhostPlayer ghostPlayer = GhostPlayerInstance.Instantiate() as GhostPlayer;
            Owner.AddChild(ghostPlayer);
            ghostPlayer.GlobalPosition = GlobalPosition;
            ghostPlayer.SetHValue(animatedSprite2D.FlipH);

            if (dashTimer <= 0)
            {
                isDashing = false;
                dashTimer = dashTimeReset;
                Velocity = new Vector2(0, 0);
                CurrentState = PlayerState.Idle;
                gravity = gravityReset;
            }
        }
    }
    private void ProcessIdle(ref Vector2 velocity)
    {
        if (velocity.X < 5 && velocity.X > -5 && health > 0 && !isAttacking)
        {
            // animatedSprite2D.Play("Idle_2");
            animatedSprite2D.Play("Idle");
        }
        velocity.X = Mathf.Lerp(velocity.X, 0, Friction);
    }
    private Vector2 ProcessMovement(ref Vector2 velocity)
    {

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("move_left", "move_right", "climb_up", "climb_down");
        if (!isTakingDammage && !isDashing && !isAttacking)
        {
            if (Input.IsActionJustPressed("dash"))
            {
                CurrentState = PlayerState.Dashing;
            }
            if (direction.X < 0)
            {
                facingDirection.X = -1;
                animatedSprite2D.FlipH = true;
            }
            else if (direction.X > 0)
            {
                facingDirection.X = 1;
                animatedSprite2D.FlipH = false;
            }
            else if (direction.Y < 0)
            {
                facingDirection.Y = -1;
            }
            else if (direction.Y > 0)
            {
                facingDirection.Y = 1;
            }
            if (IsOnFloor())
            {
                if (direction != Vector2.Zero)
                {
                    animatedSprite2D.Play("Run");
                    velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, Acceleration);
                }
                else
                {
                    ProcessIdle(ref velocity);
                }
            }
            else
            {
                velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, Acceleration);
            }
        }
        return direction;
    }

    private Vector2 ProcessDash(Vector2 velocity)
    {
        // var direction = animatedSprite2D.FlipH ? -1 : 1;
        if (canDash && !isDashing)
        {
            dashTimer = dashTimeReset;
            isDashing = true;
            int direction = animatedSprite2D.FlipH ? -1 : 1;
            Velocity = new Vector2(Speed, 0);
            velocity.X = DashSpeed * direction;

            gravity = DashGravity;
            if (facingDirection.Y < .5 && facingDirection.Y > -.5)
            {
                velocity.Y = 0;
            }
            else if (facingDirection.Y <= -.5 && facingDirection.Y >= -.85)
            {
                velocity.Y = DashSpeed * -.5f;
            }
            else if (facingDirection.Y < -.85)
            {
                gravity = gravityReset;
                velocity.Y = DashSpeed * -1;
            }
            else if (facingDirection.Y >= .5 && facingDirection.Y <= .85)
            {
                velocity.Y = DashSpeed * .5f;
            }
            else if (facingDirection.Y > .85)
            {
                velocity.Y = DashSpeed * 1;
            }
        }
        return velocity;
    }

    private Vector2 ProcessJump(Vector2 velocity)
    {
        bool isColliding = GetNode<RayCast2D>("LeftRayCast2D").IsColliding() || GetNode<RayCast2D>("RightRayCast2D").IsColliding();
        RayCast2D leftRayCast = GetNode<RayCast2D>("LeftRayCast2D");
        RayCast2D rightRayCast = GetNode<RayCast2D>("RightRayCast2D");
        string collidingRaycast = null;

        if (leftRayCast.IsColliding() && leftRayCast.GetCollider().GetType().Name == "TileMapLayer")
        {
            collidingRaycast = "LeftRayCast2D";
        }
        else if (rightRayCast.IsColliding() && rightRayCast.GetCollider().GetType().Name == "TileMapLayer")
        {
            collidingRaycast = "RightRayCast2D";
        }

        if (!IsOnFloor())
        {
            if (velocity.Y < 0)
            {
                animatedSprite2D.Play("Jump");
            }
            else
            {
                animatedSprite2D.Play("Fall");
                isFalling = true;
            }

            if (!isWallJumping && isColliding && collidingRaycast != null)
            {
                float colliderDirection = collidingRaycast == "LeftRayCast2D" ? -1 : 1;
                bool flipHDirection = collidingRaycast == "LeftRayCast2D" ? false : true;
                animatedSprite2D.FlipH = flipHDirection;
                velocity.Y = maxJumpVelocity;
                velocity.X = maxJumpVelocity * colliderDirection;
                isWallJumping = true;
            }

            if (collidingRaycast == null && canDoubleJump && !isDoubleJumping)
            {
                if (Input.IsActionJustPressed("jump") && canDoubleJump && !isDoubleJumping)
                {
                    PlayJumpEffects();
                    velocity.Y = maxJumpVelocity;
                    isDoubleJumping = true;
                    animatedSprite2D.Play("DoubleJump");
                }
            }
        }
        else
        {
            PlayJumpEffects();
            velocity.Y = maxJumpVelocity;
        }
        return velocity;
    }

    private void PlayJumpEffects()
    {
        JumpEffects je = JumpEffectsInstance.Instantiate() as JumpEffects;
        Owner.AddChild(je);
        je.GetNode<AnimatedSprite2D>("AnimatedSprite2D").GlobalPosition = GlobalPosition;
        je.Liftoff();
    }

    private void ProcessMeleAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            bool faceDirection = animatedSprite2D.FlipH;
            GameManager.MagicController.CastSpell(faceDirection);
            animatedSprite2D.Play("Attack");
        }
    }
    public void TakeDamage(int damage)
    {
        if (!isTakingDammage && !isDead)
        {
            Velocity = new Vector2(-200 * facingDirection.X, -100);
            animatedSprite2D.Play("TakeDamage");
            CurrentState = PlayerState.TakingDamage;
            isTakingDammage = true;
            health -= damage;
            InterfaceManager.UpdateHealthBar(health, maxHealth);
            if (health <= 0)
            {
                isDead = true;
                health = 0;
                InterfaceManager.UpdateHealthBar(maxHealth, maxHealth);
                animatedSprite2D.Play("Death");
            }
        }
    }
    public void UpdateMana(float amount)
    {
        mana += amount;
        if (mana > maxMana)
        {
            mana = maxMana;
        }
        else if (mana < 0)
        {
            mana = 0;
        }
    }
    public void RespawnPlayer()
    {
        isDead = false;
        Velocity = new Vector2(0, 0);
        health = maxHealth;
        animatedSprite2D.Play("Idle");
        animatedSprite2D.Show();
        isTakingDammage = false;
    }

    public void InteractWithObject()
    {
        var IsColliding = GetNode<RayCast2D>("LeftRayCast2D").IsColliding() || GetNode<RayCast2D>("RightRayCast2D").IsColliding();
        if (IsColliding)
        {
            Node obj = (Node)GetNode<RayCast2D>("LeftRayCast2D").GetCollider() ?? (Node)GetNode<RayCast2D>("RightRayCast2D").GetCollider();
            if (obj.Owner is Collectable)
            {
                if (obj.Owner is MagicPotion)
                {
                    MagicPotion mp = (MagicPotion)obj.Owner;
                    mp.InteractWithObject();
                }
            }
        }
    }
    // TODO Handle sequence attacks with mele attack.
    public void _on_animated_sprite_animation_finished()
    {
        switch (animatedSprite2D.Animation)
        {
            case "Death":
                // Handle death animation completion
                animatedSprite2D.Stop();
                animatedSprite2D.Hide();
                EmitSignal(nameof(Death));
                break;

            case "DoubleJump":
                // Automatically transition to fall after double jump finishes
                animatedSprite2D.Play("Fall");
                break;

            case "Attack":
                // Handle attack animation completion and manage combo attacks
                attackCount -= 1;
                if (attackCount > 0)
                {
                    // TODO: WHAT DO HERE?!
                }

                else
                {
                    // End the combo and reset the player's state to Idle
                    isAttacking = false;
                    CurrentState = PlayerState.Idle;
                }
                break;

            case "TakeDamage":
                // When damage animation ends, mark the player as no longer taking damage
                isTakingDammage = false;
                break;

            default:
                GD.Print("Unhandled animation finished: ", animatedSprite2D.Animation);
                break;
        }
    }

    public void OnComboTimeout()
    {
        // Reset combo when time runs out
        attackCount = 0;
        isAttacking = false;
        CurrentState = PlayerState.Idle;
    }
}
