/// <summary>
/// 游戏设置
/// </summary>
public class GameOption
{
    public string Resolution { set; get; } = "1280 x 720";
    public string WindowMode { set; get; } = "Windowed";
    public string Language { set; get; } = "中文";
    public string VSync { set; get; } = "Enable";
    public string Fps { set; get; } = "60";
    public string GameSpeed { set; get; } = "1 X";
    public double AudioMaster { set; get; } = 1;
    public double AudioBgm { set; get; } = 1;
    public double AudioSfx { set; get; } = 1;
    public double FontSize { set; get; } = 40;
}