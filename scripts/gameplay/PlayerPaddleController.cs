using Godot;

public partial class PlayerPaddleController : Node
{
    [Export] public float KeyboardSpeed { get; set; } = 640f;

    private Paddle? _paddle;
    private bool _dragging;

    public void Configure(Paddle paddle)
    {
        _paddle = paddle;
        SetProcess(true);
        SetProcessInput(true);
    }

    public override void _Input(InputEvent @event)
    {
        if (_paddle is null)
        {
            return;
        }

        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)
        {
            _dragging = mouseButton.Pressed;
            if (_dragging)
            {
                _paddle.MoveToX(mouseButton.Position.X);
            }
        }
        else if (@event is InputEventMouseMotion mouseMotion && _dragging)
        {
            _paddle.MoveToX(mouseMotion.Position.X);
        }
        else if (@event is InputEventScreenTouch touch)
        {
            _dragging = touch.Pressed;
            if (_dragging)
            {
                _paddle.MoveToX(touch.Position.X);
            }
        }
        else if (@event is InputEventScreenDrag drag)
        {
            _paddle.MoveToX(drag.Position.X);
        }
    }

    public override void _Process(double delta)
    {
        if (_paddle is null)
        {
            return;
        }

        var direction = 0f;
        if (Input.IsKeyPressed(Key.Left) || Input.IsKeyPressed(Key.A))
        {
            direction -= 1f;
        }

        if (Input.IsKeyPressed(Key.Right) || Input.IsKeyPressed(Key.D))
        {
            direction += 1f;
        }

        if (!Mathf.IsZeroApprox(direction))
        {
            _paddle.MoveBy(direction * KeyboardSpeed * (float)delta);
        }
    }
}
