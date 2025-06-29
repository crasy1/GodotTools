namespace Godot;

public static class ImageExtensions
{
    public static ImageTexture Texture(this Image image)
    {
        return ImageTexture.CreateFromImage(image);
    }
}