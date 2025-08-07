using Godot;
using System;
using Steamworks.Data;

[SceneTree]
public partial class Test2dPlayer : CharacterBody2D
{
    public const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;

    [Export(PropertyHint.Range, "0,1,0.02")]
    public double UpdateRate { set; get; } = 0.2;

    public bool IsLocal { set; get; }
    private double LastUpdateTime { set; get; }
    public SteamSocket SteamSocket { set; get; }

    public TestMsg TestMsg { set; get; }

    public override void _PhysicsProcess(double delta)
    {
        LastUpdateTime += delta;
        if (IsLocal)
        {
            Vector2 velocity = Velocity;

            // Add the gravity.
            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float)delta;
            }

            // Handle Jump.
            if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            {
                velocity.Y = JumpVelocity;
            }

            // Get the input direction and handle the movement/deceleration.
            // As good practice, you should replace UI actions with custom gameplay actions.
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (direction != Vector2.Zero)
            {
                velocity.X = direction.X * Speed;
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            }

            Velocity = velocity;
            MoveAndSlide();
            if (LastUpdateTime > UpdateRate)
            {
                var protoBufMsg = ProtoBufMsg.From(new TestMsg()
                {
                    Position = Position,
                    Rotation = Rotation
                });
                SteamSocket.Send(protoBufMsg, SendType.Unreliable);
                LastUpdateTime = 0;
            }
        }
        else
        {
            if (TestMsg != null)
            {
                Position = Position.Lerp(TestMsg.Position, 0.5f);
                Rotation = Mathf.LerpAngle(Rotation, TestMsg.Rotation, 0.5f);
            }
        }
    }
}