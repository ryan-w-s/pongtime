using System;
using Godot;

public partial class GameplayScreen : Control
{
    public event Action<MatchResult>? MatchFinished;

    private const int VirtualWidth = 540;
    private const int VirtualHeight = 960;
    private const int TargetScore = 5;
    private const float ResetDelay = 0.85f;
    private const float SlowDuration = 2.0f;
    private const float SlowCooldown = 6.0f;
    private const float SlowScale = 0.38f;

    private readonly Rect2 _playfield = new(28, 112, 484, 764);

    private Ball? _ball;
    private Paddle? _playerPaddle;
    private Paddle? _cpuPaddle;
    private PlayerPaddleController? _playerController;
    private CpuPaddleController? _cpuController;
    private Label? _scoreLabel;
    private Label? _statusLabel;
    private Control? _slowChargeBar;
    private ColorRect? _slowChargeFill;
    private Label? _slowChargeLabel;
    private Button? _slowChargeButton;

    private int _playerScore;
    private int _cpuScore;
    private int _nextLaunchDirection = 1;
    private float _resetTimer = ResetDelay;
    private float _slowTimer;
    private float _cooldownTimer;
    private bool _matchOver;

    private bool SlowActive => _slowTimer > 0f;
    private bool SlowReady => _cooldownTimer <= 0f && !SlowActive && !_matchOver;

    public override void _Ready()
    {
        CustomMinimumSize = new Vector2(VirtualWidth, VirtualHeight);
        BuildInterface();
        BuildGameplayObjects();
        ResetRound(launchTowardPlayer: true);
        UpdateUi();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && !key.Echo && key.Keycode == Key.Space)
        {
            TryActivateSlowTime();
        }
    }

    public override void _Process(double delta)
    {
        var step = (float)delta;

        if (_matchOver)
        {
            return;
        }

        TickAbility(step);

        if (_resetTimer > 0f)
        {
            _resetTimer -= step;
            if (_resetTimer <= 0f)
            {
                LaunchBall();
            }

            UpdateUi();
            QueueRedraw();
            return;
        }

        var simScale = SlowActive ? SlowScale : 1f;
        _ball?.Step(step, _playfield, simScale);
        _cpuController?.Tick(step, simScale);
        HandlePaddleCollisions();
        HandleScoring();
        UpdateUi();
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(Vector2.Zero, new Vector2(VirtualWidth, VirtualHeight)), NeonTheme.Background);
        DrawRect(_playfield.Grow(10), new Color(0.0f, 0.35f, 0.70f, SlowActive ? 0.16f : 0.08f));
        DrawRect(_playfield, new Color(0.0f, 0.08f, 0.15f));
        DrawRect(_playfield, NeonTheme.Cyan, filled: false, width: 2f);

        for (var y = _playfield.Position.Y + 20f; y < _playfield.End.Y; y += 34f)
        {
            DrawLine(new Vector2(_playfield.Position.X + 24f, y), new Vector2(_playfield.End.X - 24f, y), new Color(0.0f, 0.75f, 1.0f, 0.28f), 2f);
        }

        if (SlowActive)
        {
            DrawRect(_playfield, new Color(0.0f, 0.55f, 1.0f, 0.08f));
        }
    }

    private void BuildInterface()
    {
        _scoreLabel = new Label();
        NeonTheme.StyleLabel(_scoreLabel, 44, NeonTheme.Text);
        _scoreLabel.SetAnchorsPreset(LayoutPreset.TopWide);
        _scoreLabel.OffsetTop = 24;
        _scoreLabel.OffsetBottom = 84;
        AddChild(_scoreLabel);

        _statusLabel = new Label();
        NeonTheme.StyleLabel(_statusLabel, 16, NeonTheme.Muted);
        _statusLabel.SetAnchorsPreset(LayoutPreset.TopWide);
        _statusLabel.OffsetTop = 76;
        _statusLabel.OffsetBottom = 108;
        AddChild(_statusLabel);

        _slowChargeBar = new Panel
        {
            ClipContents = true
        };
        _slowChargeBar.SetAnchorsPreset(LayoutPreset.BottomWide);
        _slowChargeBar.OffsetLeft = 28;
        _slowChargeBar.OffsetRight = -28;
        _slowChargeBar.OffsetTop = -78;
        _slowChargeBar.OffsetBottom = -22;
        _slowChargeBar.AddThemeStyleboxOverride("panel", NeonTheme.MakeButtonStyle(new Color(0.01f, 0.07f, 0.12f), NeonTheme.Cyan));
        AddChild(_slowChargeBar);

        _slowChargeFill = new ColorRect
        {
            Color = new Color(0.0f, 0.82f, 1.0f, 0.84f),
            MouseFilter = MouseFilterEnum.Ignore
        };
        _slowChargeFill.SetAnchorsPreset(LayoutPreset.FullRect);
        _slowChargeFill.AnchorRight = 1f;
        _slowChargeFill.OffsetLeft = 8f;
        _slowChargeFill.OffsetTop = 10f;
        _slowChargeFill.OffsetRight = 0f;
        _slowChargeFill.OffsetBottom = -10f;
        _slowChargeBar.AddChild(_slowChargeFill);

        _slowChargeLabel = new Label
        {
            MouseFilter = MouseFilterEnum.Ignore,
            VerticalAlignment = VerticalAlignment.Center
        };
        NeonTheme.StyleLabel(_slowChargeLabel, 22, NeonTheme.Text);
        _slowChargeLabel.SetAnchorsPreset(LayoutPreset.FullRect);
        _slowChargeBar.AddChild(_slowChargeLabel);

        _slowChargeButton = new Button
        {
            Text = string.Empty,
            FocusMode = FocusModeEnum.None,
            MouseDefaultCursorShape = CursorShape.PointingHand
        };
        _slowChargeButton.SetAnchorsPreset(LayoutPreset.FullRect);
        _slowChargeButton.AddThemeStyleboxOverride("normal", new StyleBoxEmpty());
        _slowChargeButton.AddThemeStyleboxOverride("hover", new StyleBoxEmpty());
        _slowChargeButton.AddThemeStyleboxOverride("pressed", new StyleBoxEmpty());
        _slowChargeButton.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());
        _slowChargeButton.Pressed += TryActivateSlowTime;
        _slowChargeBar.AddChild(_slowChargeButton);
    }

    private void BuildGameplayObjects()
    {
        var ballScene = GD.Load<PackedScene>("res://scenes/gameplay/Ball.tscn");
        var paddleScene = GD.Load<PackedScene>("res://scenes/gameplay/Paddle.tscn");

        _ball = ballScene.Instantiate<Ball>();
        _playerPaddle = paddleScene.Instantiate<Paddle>();
        _cpuPaddle = paddleScene.Instantiate<Paddle>();

        _playerPaddle.Name = "PlayerPaddle";
        _cpuPaddle.Name = "CpuPaddle";
        _playerPaddle.MovementBounds = _playfield;
        _cpuPaddle.MovementBounds = _playfield;
        _cpuPaddle.BodyColor = new Color(0.14f, 0.46f, 1.0f);

        AddChild(_cpuPaddle);
        AddChild(_playerPaddle);
        AddChild(_ball);

        _playerController = new PlayerPaddleController { Name = "PlayerPaddleController" };
        _playerController.Configure(_playerPaddle);
        AddChild(_playerController);

        _cpuController = new CpuPaddleController { Name = "CpuPaddleController" };
        _cpuController.Configure(_cpuPaddle, _ball);
        AddChild(_cpuController);
    }

    private void ResetRound(bool launchTowardPlayer)
    {
        if (_ball is null || _playerPaddle is null || _cpuPaddle is null)
        {
            return;
        }

        _playerPaddle.Position = new Vector2(_playfield.GetCenter().X, _playfield.End.Y - 48f);
        _cpuPaddle.Position = new Vector2(_playfield.GetCenter().X, _playfield.Position.Y + 48f);
        _ball.ResetTo(_playfield.GetCenter());
        _nextLaunchDirection = launchTowardPlayer ? 1 : -1;
        _resetTimer = ResetDelay;
    }

    private void LaunchBall()
    {
        var biasSeed = (_playerScore * 2.17f) - (_cpuScore * 1.41f) + (_nextLaunchDirection * 0.3f);
        var bias = Mathf.Clamp(Mathf.Sin(biasSeed) * 0.48f, -0.48f, 0.48f);
        _ball?.Launch(_nextLaunchDirection, bias);
    }

    private void HandlePaddleCollisions()
    {
        if (_ball is null || _playerPaddle is null || _cpuPaddle is null)
        {
            return;
        }

        if (_ball.Velocity.Y > 0f && _ball.CollisionRect.Intersects(_playerPaddle.CollisionRect))
        {
            _ball.BounceFrom(_playerPaddle, towardTop: true);
        }
        else if (_ball.Velocity.Y < 0f && _ball.CollisionRect.Intersects(_cpuPaddle.CollisionRect))
        {
            _ball.BounceFrom(_cpuPaddle, towardTop: false);
        }
    }

    private void HandleScoring()
    {
        if (_ball is null)
        {
            return;
        }

        if (_ball.Position.Y < _playfield.Position.Y - 34f)
        {
            ScorePoint(playerScored: true);
        }
        else if (_ball.Position.Y > _playfield.End.Y + 34f)
        {
            ScorePoint(playerScored: false);
        }
    }

    private void ScorePoint(bool playerScored)
    {
        if (playerScored)
        {
            _playerScore++;
        }
        else
        {
            _cpuScore++;
        }

        if (_playerScore >= TargetScore || _cpuScore >= TargetScore)
        {
            _matchOver = true;
            MatchFinished?.Invoke(new MatchResult(_playerScore, _cpuScore));
            return;
        }

        ResetRound(launchTowardPlayer: playerScored);
    }

    private void TryActivateSlowTime()
    {
        if (!SlowReady)
        {
            return;
        }

        _slowTimer = SlowDuration;
        _cooldownTimer = SlowCooldown;
        UpdateUi();
        QueueRedraw();
    }

    private void TickAbility(float delta)
    {
        if (_slowTimer > 0f)
        {
            _slowTimer = Mathf.Max(0f, _slowTimer - delta);
        }

        if (_cooldownTimer > 0f)
        {
            _cooldownTimer = Mathf.Max(0f, _cooldownTimer - delta);
        }
    }

    private void UpdateUi()
    {
        if (_scoreLabel is not null)
        {
            _scoreLabel.Text = $"{_playerScore}   { _cpuScore}";
        }

        if (_statusLabel is not null)
        {
            _statusLabel.Text = _resetTimer > 0f ? "GET READY" : "FIRST TO 5";
        }

        UpdateSlowChargeBar();
    }

    private void UpdateSlowChargeBar()
    {
        if (_slowChargeFill is null || _slowChargeLabel is null || _slowChargeButton is null)
        {
            return;
        }

        var fillAmount = SlowActive || SlowReady
            ? 1f
            : Mathf.Clamp(1f - (_cooldownTimer / SlowCooldown), 0f, 1f);

        _slowChargeFill.AnchorRight = fillAmount;
        _slowChargeFill.OffsetLeft = 8f;
        _slowChargeFill.OffsetTop = 10f;
        _slowChargeFill.OffsetRight = -8f;
        _slowChargeFill.OffsetBottom = -10f;
        _slowChargeFill.Visible = fillAmount > 0.01f;
        _slowChargeFill.Color = SlowReady
            ? new Color(0.0f, 0.95f, 1.0f, 0.92f)
            : SlowActive
                ? new Color(0.16f, 0.55f, 1.0f, 0.88f)
                : new Color(0.0f, 0.65f, 0.95f, 0.68f);

        _slowChargeLabel.Text = SlowActive
            ? "TIME SLOWED"
            : SlowReady
                ? "SLOW"
                : "CHARGING";

        _slowChargeButton.MouseDefaultCursorShape = SlowReady ? CursorShape.PointingHand : CursorShape.Arrow;
    }
}
