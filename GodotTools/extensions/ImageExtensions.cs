namespace Godot;

public static class ImageExtensions
{
    public static ImageTexture Texture(this Image image)
    {
        return ImageTexture.CreateFromImage(image);
    }

    /// <summary>
    /// steamworks image to godot image
    /// </summary>
    /// <param name="steamworksImage"></param>
    /// <param name="useMipmaps"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static Image GodotImage(this Steamworks.Data.Image steamworksImage, bool useMipmaps = false,
        Image.Format format = Image.Format.Rgba8)
    {
        return Image.CreateFromData((int)steamworksImage.Width, (int)steamworksImage.Height, useMipmaps,
            format,
            steamworksImage.Data);
    }
}