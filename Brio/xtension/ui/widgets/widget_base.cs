using System;

namespace Brio.Xtension.ui.widgets;

public abstract class XTWidget {
    public abstract void Draw();
    public abstract XTWidgetCategory WidgetCategory { get; }
}

[Flags]
public enum XTWidgetCategory {
    None = 0,
    Posing = 2,
    Camera = 4,
    Lighting = 8,
    Expression = 16,
    Scene = 32,
    Selection = 64,
    Config = 128,
}
