namespace Godot;

public static class NodePaths
{
    public static readonly NodePath SelfModulate = new(CanvasItem.PropertyName.SelfModulate);
    public static readonly NodePath Modulate = new(CanvasItem.PropertyName.Modulate);
    public static readonly NodePath ControlPosition = new(Control.PropertyName.Position);
    public static readonly NodePath ControlScale = new(Control.PropertyName.Scale);
    public static readonly NodePath ControlRotationDegrees = new(Control.PropertyName.RotationDegrees);
    public static readonly NodePath ControlGlobalPosition = new(Control.PropertyName.GlobalPosition);
    public static readonly NodePath ControlAnchorLeft = new(Control.PropertyName.AnchorLeft);
    public static readonly NodePath ControlAnchorRight = new(Control.PropertyName.AnchorRight);
    public static readonly NodePath ControlAnchorTop = new(Control.PropertyName.AnchorTop);
    public static readonly NodePath ControlAnchorBottom = new(Control.PropertyName.AnchorBottom);
    public static readonly NodePath ControlOffsetLeft = new(Control.PropertyName.OffsetLeft);
    public static readonly NodePath ControlOffsetRight = new(Control.PropertyName.OffsetRight);
    public static readonly NodePath ControlOffsetTop = new(Control.PropertyName.OffsetTop);
    public static readonly NodePath ControlOffsetBottom = new(Control.PropertyName.OffsetBottom);
    public static readonly NodePath Camera2DOffset = new(Camera2D.PropertyName.Offset);
    public static readonly NodePath VolumeDb = new(AudioStreamPlayer.PropertyName.VolumeDb);
    public static readonly NodePath RangeValue = new(Range.PropertyName.Value);
    public static readonly NodePath ScrollVertical = new(ScrollContainer.PropertyName.ScrollVertical);
    public static readonly NodePath Color = new(ColorRect.PropertyName.Color);
    public static readonly string ColorA = $"{Color}:a";
    public static readonly string SelfModulateA = $"{SelfModulate}:a";
}