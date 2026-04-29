using Godot;

public partial class Ball : Node2D
{
    [Export] public float Radius { get; set; } = 9f;
    [Export] public float BaseSpeed { get; set; } = 390f;
    [Export] public float MaxSpeed { get; set; } = 620f;
    [Export] public float HitSpeedBoost { get; set; } = 24f;

    public Vector2 Velocity { get; private set; } = Vector2.Zero;
    public float CurrentSpeed { get; private set; }
    public Rect2 CollisionRect => new(GlobalPosition - new Vector2(Radius, Radius), new Vector2(Radius * 2f, Radius * 2f));

    public override void _Ready()
    {
        CurrentSpeed = BaseSpeed;
    }

    public void ResetTo(Vector2 position)
    {
        Position = position;
        Velocity = Vector2.Zero;
        CurrentSpeed = BaseSpeed;
        QueueRedraw();
    }

    public void Launch(int verticalDirection, float horizontalBias)
    {
        var direction = new Vector2(horizontalBias, verticalDirection).Normalized();
        Velocity = direction * CurrentSpeed;
    }

    public void Step(float delta, Rect2 bounds, float timeScale)
    {
        Position += Velocity * delta * timeScale;

        if (Position.X - Radius <= bounds.Position.X)
        {
            Position = new Vector2(bounds.Position.X + Radius, Position.Y);
            Velocity = new Vector2(Mathf.Abs(Velocity.X), Velocity.Y);
        }
        else if (Position.X + Radius >= bounds.End.X)
        {
            Position = new Vector2(bounds.End.X - Radius, Position.Y);
            Velocity = new Vector2(-Mathf.Abs(Velocity.X), Velocity.Y);
        }
    }

    public void BounceFrom(Paddle paddle, bool towardTop)
    {
        var offset = Mathf.Clamp((Position.X - paddle.Position.X) / (paddle.Size.X * 0.5f), -1f, 1f);
        CurrentSpeed = Mathf.Min(CurrentSpeed + HitSpeedBoost, MaxSpeed);
        var yDirection = towardTop ? -1f : 1f;
        Velocity = new Vector2(offset * 0.82f, yDirection).Normalized() * CurrentSpeed;
        Position = new Vector2(Position.X, paddle.Position.Y + yDirection * (paddle.Size.Y * 0.5f + Radius + 1f));
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, Radius + 7f, new Color(0.0f, 0.50f, 1.0f, 0.18f));
        DrawCircle(Vector2.Zero, Radius, Colors.White);
        DrawCircle(Vector2.Zero, Radius * 0.62f, NeonTheme.Cyan);
    }
}
