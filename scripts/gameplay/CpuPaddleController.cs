using Godot;

public partial class CpuPaddleController : Node
{
    [Export] public float MaxSpeed { get; set; } = 470f;
    [Export] public float TrackingLead { get; set; } = 0.22f;
    [Export] public float Imperfection { get; set; } = 28f;

    private Paddle? _paddle;
    private Ball? _ball;

    public void Configure(Paddle paddle, Ball ball)
    {
        _paddle = paddle;
        _ball = ball;
    }

    public void Tick(float delta, float slowScale)
    {
        if (_paddle is null || _ball is null)
        {
            return;
        }

        var targetX = _ball.Position.X + _ball.Velocity.X * TrackingLead;
        var wave = Mathf.Sin((float)Time.GetTicksMsec() * 0.0023f) * Imperfection;
        var difference = targetX + wave - _paddle.Position.X;
        var step = Mathf.Clamp(difference, -MaxSpeed * delta * slowScale, MaxSpeed * delta * slowScale);
        _paddle.MoveBy(step);
    }
}
