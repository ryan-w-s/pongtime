using Godot;

public static class NeonTheme
{
    public static readonly Color Background = new(0.015f, 0.025f, 0.06f);
    public static readonly Color Panel = new(0.02f, 0.055f, 0.11f);
    public static readonly Color Cyan = new(0.0f, 0.82f, 1.0f);
    public static readonly Color Blue = new(0.05f, 0.28f, 1.0f);
    public static readonly Color Text = new(0.72f, 0.95f, 1.0f);
    public static readonly Color Muted = new(0.30f, 0.55f, 0.68f);
    public static readonly Color Warning = new(0.45f, 0.92f, 1.0f);

    public static StyleBoxFlat MakeButtonStyle(Color fill, Color border)
    {
        var style = new StyleBoxFlat
        {
            BgColor = fill,
            BorderColor = border,
            BorderWidthBottom = 2,
            BorderWidthLeft = 2,
            BorderWidthRight = 2,
            BorderWidthTop = 2,
            CornerRadiusBottomLeft = 6,
            CornerRadiusBottomRight = 6,
            CornerRadiusTopLeft = 6,
            CornerRadiusTopRight = 6,
            ContentMarginBottom = 12,
            ContentMarginLeft = 18,
            ContentMarginRight = 18,
            ContentMarginTop = 12
        };

        return style;
    }

    public static void StyleButton(Button button)
    {
        button.FocusMode = Control.FocusModeEnum.None;
        button.AddThemeFontSizeOverride("font_size", 22);
        button.AddThemeColorOverride("font_color", Text);
        button.AddThemeColorOverride("font_hover_color", Colors.White);
        button.AddThemeStyleboxOverride("normal", MakeButtonStyle(new Color(0.02f, 0.10f, 0.18f), Cyan));
        button.AddThemeStyleboxOverride("hover", MakeButtonStyle(new Color(0.03f, 0.18f, 0.28f), Colors.White));
        button.AddThemeStyleboxOverride("pressed", MakeButtonStyle(new Color(0.0f, 0.28f, 0.40f), Cyan));
        button.AddThemeStyleboxOverride("disabled", MakeButtonStyle(new Color(0.02f, 0.05f, 0.08f), Muted));
    }

    public static void StyleLabel(Label label, int fontSize, Color? color = null)
    {
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.AddThemeFontSizeOverride("font_size", fontSize);
        label.AddThemeColorOverride("font_color", color ?? Text);
    }
}
