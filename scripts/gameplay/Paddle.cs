using Godot;

public partial class Paddle : Node2D
{
    [Export] public Vector2 Size { get; set; } = new(118, 20);
    [Export] public Color BodyColor { get; set; } = NeonTheme.Cyan;
    [Export] public Color GlowColor { get; set; } = new(0.0f, 0.50f, 1.0f, 0.22f);

    public Rect2 MovementBounds { get; set; } = new(0, 0, 540, 960);

    public Rect2 CollisionRect => new(GlobalPosition - Size * 0.5f, Size);

    public void MoveToX(float x)
    {
        Position = new Vector2(Mathf.Clamp(x, MovementBounds.Position.X + Size.X * 0.5f, MovementBounds.End.X - Size.X * 0.5f), Position.Y);
    }

    public void MoveBy(float amount)
    {
        MoveToX(Position.X + amount);
    }

    public override void _Draw()
    {
        var glowRect = new Rect2(-Size * 0.5f - new Vector2(7, 7), Size + new Vector2(14, 14));
        var bodyRect = new Rect2(-Size * 0.5f, Size);
        DrawRect(glowRect, GlowColor);
        DrawRect(bodyRect, BodyColor);
        DrawRect(new Rect2(bodyRect.Position + new Vector2(0, 3), new Vector2(Size.X, 3)), Colors.White with { A = 0.35f });
    }
}
