using Godot;

namespace GodotTools.extensions;

public static class ImageExtensions
{
    public static ImageTexture Texture(this Image image)
    {
        return ImageTexture.CreateFromImage(image);
    }
}