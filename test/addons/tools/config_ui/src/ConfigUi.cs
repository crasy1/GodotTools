using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Godot;

[SceneTree]
[Singleton]
public partial class ConfigUi : CanvasLayer
{
    [Export] public Key UiOptionsKey = Key.Escape;

    public const string UiOptionsAction = "ui_options_action";

    public Control FirstUiControl => FontSizeSlider;

    private static readonly Dictionary<string, Vector2I> Resolutions = new()
    {
        { "640 x 480", new Vector2I(640, 480) },
        { "800 x 600", new Vector2I(800, 600) },
        { "1024 x 768", new Vector2I(1024, 768) },
        { "1280 x 720", new Vector2I(1280, 720) },
        { "1920 x 1080", new Vector2I(1920, 1080) },
        { "2560 x 1440", new Vector2I(2560, 1440) },
        { "2560 x 1600", new Vector2I(2560, 1600) },
        { "3840 x 2160", new Vector2I(3840, 2160) },
    };

    private static readonly Dictionary<string, DisplayServer.WindowMode> WindowModes = new()
    {
        { "Windowed", DisplayServer.WindowMode.Windowed },
        { "Full Screen", DisplayServer.WindowMode.Fullscreen }
    };

    private static readonly Dictionary<string, DisplayServer.VSyncMode> WindowVSyncModes = new()
    {
        { "Disable", DisplayServer.VSyncMode.Disabled },
        { "Enable", DisplayServer.VSyncMode.Enabled },
    };

    private static readonly Dictionary<string, int> Fps = new()
    {
        { "24", 24 },
        { "30", 30 },
        { "60", 60 },
        { "90", 90 },
        { "120", 120 },
        { "144", 144 },
        { "240", 240 },
        { "Unlimited", 0 },
    };

    private static readonly Dictionary<string, double> GameSpeeds = new()
    {
        { "0.5 X", 0.5 },
        { "1 X", 1 },
        { "1.5 X", 1.5 },
        { "2 X", 2 },
        { "4 X", 4 },
    };

    private static readonly Dictionary<string, string> Languages = new()
    {
        { "English", "en" },
        { "中文", "zh" },
    };

    /// <summary>
    /// TODO 本地化文件夹,po文件会在这些文件夹下查找
    /// </summary>
    private static readonly List<string> PoFileDirs =
    [
        Paths.LocalizationUi,
        Paths.LocalizationSrc,
        Paths.LocalizationUser,
    ];

    public static GameOption GameOption { set; get; }

	
    public override void _Ready()
    {
        SimpleColorRect.Color = SimpleColorRect.Color with { A = 0 };
        DisplayBuild();
        AudioBuild();
        OtherBuild();
    }

    /**
     * 加载并构建ui
     */
    public void LoadAndBuildUi()
    {
        LoadOptions();
        AddTranslations();
        ApplyGameOptions(GameOption);
    }

    private void LoadOptions()
    {
        if (FileAccess.FileExists(Paths.GameOption))
        {
            GameOption = JsonUtil.FileToObj<GameOption>(Paths.GameOption);
        }
        else
        {
            GameOption = new GameOption();
        }

        Log.Info($"加载游戏设置 {JsonUtil.ToJsonString(GameOption)}");
    }

    private static void SaveOptions()
    {
        JsonUtil.ObjToFile(GameOption, Paths.GameOption);
        Log.Info($"保存游戏设置 {JsonUtil.ToJsonString(GameOption)}");
    }


    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventAction eventAction)
        {
            if (eventAction.Pressed && UiOptionsAction.Equals(eventAction.Action))
            {
                Options.Visible = !Options.Visible;
            }
        }

        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed && UiOptionsKey.Equals(eventKey.Keycode))
            {
                Options.Visible = !Options.Visible;
            }
        }
    }

    public void ApplyGameOptions(GameOption gameOption)
    {
        if (gameOption is null)
        {
            gameOption = GameOption = new GameOption();
        }

        // resolution
        var index = Resolutions.Keys.ToList().IndexOf(gameOption.Resolution);
        ResolutionBtn.Select(index);
        ResolutionBtn.EmitSignal(OptionButton.SignalName.ItemSelected, index);
        // window mode
        index = WindowModes.Keys.ToList().IndexOf(gameOption.WindowMode);
        WindowModeBtn.Select(index);
        WindowModeBtn.EmitSignal(OptionButton.SignalName.ItemSelected, index);
        // vsync
        index = WindowVSyncModes.Keys.ToList().IndexOf(gameOption.VSync);
        WindowVSyncModeBtn.Select(index);
        WindowVSyncModeBtn.EmitSignal(OptionButton.SignalName.ItemSelected, index);
        // fps
        index = Fps.Keys.ToList().IndexOf(gameOption.Fps);
        MaxFpsBtn.Select(index);
        MaxFpsBtn.EmitSignal(OptionButton.SignalName.ItemSelected, index);
        // language
        index = Languages.Keys.ToList().IndexOf(gameOption.Language);
        LocalizationBtn.Select(index);
        LocalizationBtn.EmitSignal(OptionButton.SignalName.ItemSelected, index);
        // game speed
        index = GameSpeeds.Keys.ToList().IndexOf(gameOption.GameSpeed);
        GameSpeedBtn.Select(index);
        GameSpeedBtn.EmitSignal(OptionButton.SignalName.ItemSelected, index);
        // font size
        FontSizeSlider.Value = gameOption.FontSize;
        FontSizeSlider.EmitSignal(Range.SignalName.ValueChanged, gameOption.FontSize);
        // master
        AudioMasterSlider.Value = gameOption.AudioMaster;
        AudioMasterSlider.EmitSignal(Range.SignalName.ValueChanged, gameOption.AudioMaster);
        // bgm
        var busIndex = AudioServer.GetBusIndex("Bgm");
        if (busIndex >= 0)
        {
            AudioBgmSlider.Value = gameOption.AudioBgm;
            AudioBgmSlider.EmitSignal(Range.SignalName.ValueChanged, gameOption.AudioBgm);
        }

        // sfx
        busIndex = AudioServer.GetBusIndex("Sfx");
        if (busIndex >= 0)
        {
            AudioSfxSlider.Value = gameOption.AudioSfx;
            AudioSfxSlider.EmitSignal(Range.SignalName.ValueChanged, gameOption.AudioSfx);
        }
    }

    private void OtherBuild()
    {
        Localization();
        GameSpeed();
        FontSize();
        ResetToDefaultOptions();
        OptionsKey();
    }

    private void OptionsKey()
    {
        if (!InputMap.HasAction(UiOptionsAction))
        {
            InputMap.AddAction(UiOptionsAction);
            InputMap.ActionAddEvent(UiOptionsAction, new InputEventKey { Keycode = UiOptionsKey });
        }

        ExitOptionsBtn.Pressed += Options.Hide;
    }

    private void ResetToDefaultOptions()
    {
        ResetToDefaultOptionsBtn.Pressed += () =>
        {
            GameOption = new GameOption();

            ApplyGameOptions(GameOption);
        };
    }

    private void FontSize()
    {
        FontSizeSlider.ValueChanged += (value) =>
        {
            GameOption.FontSize = value;
            Resources.DefaultTheme.DefaultFontSize = (int)value;
            Log.Info($"字体大小改变 => {value}");
        };
    }

    private void GameSpeed()
    {
        for (int i = 0; i < GameSpeedBtn.ItemCount; i++)
        {
            GameSpeedBtn.RemoveItem(i);
        }

        GameSpeedBtn.ItemSelected += (index) =>
        {
            GameOption.GameSpeed = GameSpeeds.Keys.ToArray()[index];
            Engine.TimeScale = GameSpeeds[GameOption.GameSpeed];
            Log.Info($"游戏速度改变 => {GameOption.GameSpeed}");
        };
        foreach (var gameSpeedKey in GameSpeeds.Keys)
        {
            GameSpeedBtn.AddItem(gameSpeedKey);
        }
    }

    private void AddTranslations()
    {
        var gameName = Project.GameName;
        var gameNameTranslation = Project.GameNameLocalized;
        //  locale/fallback 设置的语言po一定要有翻译,否则切换不生效
        foreach (var parentLocalDir in PoFileDirs)
        {
            foreach (var subDir in FileUtil.GetDirsRecursive(parentLocalDir))
            {
                using var dirAccess = DirAccess.Open(subDir);
                foreach (var file in dirAccess.GetFiles())
                {
                    if (file.EndsWith(".po") || file.EndsWith(".mo"))
                    {
                        var filePath = Path.Join(subDir, file);
                        var translation = GD.Load<Translation>(filePath);
                        if (gameNameTranslation.TryGetValue(translation.Locale, out var value))
                        {
                            translation.AddMessage(gameName, value);
                        }

                        TranslationServer.AddTranslation(translation);
                        Log.Info($"添加本地化文件 =>{filePath}");
                    }
                }
            }
        }


        Log.Info($"已有本地化语言 => {TranslationServer.GetLoadedLocales().Join(",")}");
    }

    /**
     * 本地化
     * https://docs.godotengine.org/zh-cn/4.x/tutorials/i18n/internationalizing_games.html#doc-internationalizing-games
     */
    private void Localization()
    {
        for (int i = 0; i < LocalizationBtn.ItemCount; i++)
        {
            LocalizationBtn.RemoveItem(i);
        }

        LocalizationBtn.ItemSelected += (index) =>
        {
            GameOption.Language = Languages.Keys.ToArray()[index];
            TranslationServer.SetLocale(Languages[GameOption.Language]);
            Log.Info($"本地化语言改变 => {GameOption.Language}");
        };
        foreach (var language in Languages.Keys)
        {
            LocalizationBtn.AddItem(language);
        }
    }


    /**
     * 声音设置
     */
    private void AudioBuild()
    {
        AudioMasterSlider.ValueChanged += (v) => OnAudioValueChanged(v, "Master");
        var busIndex = AudioServer.GetBusIndex("Bgm");
        if (busIndex >= 0)
        {
            AudioBgmSlider.ValueChanged += (v) => OnAudioValueChanged(v, "Bgm");
        }

        busIndex = AudioServer.GetBusIndex("Sfx");
        if (busIndex >= 0)
        {
            AudioSfxSlider.ValueChanged += (v) => OnAudioValueChanged(v, "Sfx");
        }
    }


    /**
     * 画面设置
     */
    private void DisplayBuild()
    {
        Options.Hide();
        Options.VisibilityChanged += () =>
        {
            if (!Options.Visible)
            {
                SaveOptions();
            }
            else
            {
                FirstUiControl.GrabFocus();
            }
        };
        WindowResizeable(false);
        Resolution();
        WindowMode();
        WindowVSyncMode();
        MaxFps();
    }

    private void MaxFps()
    {
        for (int i = 0; i < MaxFpsBtn.ItemCount; i++)
        {
            MaxFpsBtn.RemoveItem(i);
        }

        MaxFpsBtn.ItemSelected += (index) =>
        {
            GameOption.Fps = Fps.Keys.ToArray()[index];
            Engine.MaxFps = Fps[GameOption.Fps];
            Log.Info($"最大帧率改变 => {GameOption.Fps}");
        };
        foreach (var gameSpeedKey in Fps.Keys)
        {
            MaxFpsBtn.AddItem(gameSpeedKey);
        }
    }


    /// <summary>
    /// 调整窗口大小 https://github.com/godotengine/godot/issues/84876
    /// </summary>
    /// <param name="resizeable"></param>
    private void WindowResizeable(bool resizeable)
    {
        GetViewport().GetWindow().Unresizable = !resizeable;
    }

    private void WindowMode()
    {
        for (int i = 0; i < WindowModeBtn.ItemCount; i++)
        {
            WindowModeBtn.RemoveItem(i);
        }

        WindowModeBtn.ItemSelected += (index) =>
        {
            GameOption.WindowMode = WindowModes.Keys.ToArray()[index];
            DisplayServer.WindowSetMode(WindowModes[GameOption.WindowMode]);
            ResolutionBtn.EmitSignal(OptionButton.SignalName.ItemSelected, ResolutionBtn.Selected);
            Log.Info($"窗口模式改变 => {GameOption.WindowMode}");
        };
        foreach (var modesKey in WindowModes.Keys)
        {
            WindowModeBtn.AddItem(modesKey);
        }
    }

    /**
     * 垂直同步
     */
    private void WindowVSyncMode()
    {
        for (int i = 0; i < WindowVSyncModeBtn.ItemCount; i++)
        {
            WindowVSyncModeBtn.RemoveItem(i);
        }

        WindowVSyncModeBtn.ItemSelected += (index) =>
        {
            GameOption.VSync = WindowVSyncModes.Keys.ToArray()[index];
            DisplayServer.WindowSetVsyncMode(WindowVSyncModes[GameOption.VSync]);
            Log.Info($"垂直同步 => {GameOption.VSync}");
        };
        foreach (var modesKey in WindowVSyncModes.Keys)
        {
            WindowVSyncModeBtn.AddItem(modesKey);
        }
    }

    /**
         * 分辨率
         */
    private void Resolution()
    {
        for (int i = 0; i < ResolutionBtn.ItemCount; i++)
        {
            ResolutionBtn.RemoveItem(i);
        }

        ResolutionBtn.ItemSelected += (index) =>
        {
            GameOption.Resolution = Resolutions.Keys.ToArray()[index];
            var resolution = Resolutions[GameOption.Resolution];
            // var windowSize = DisplayServer.WindowGetSize();
            // TODO 貌似窗口和分辨率会有 (16,16) 差值
            // if (Mathf.Abs(windowSize.X - resolution.X) < 50 && Mathf.Abs(windowSize.Y - resolution.Y) < 50)
            // {
            //     return;
            // }
            DisplayServer.WindowSetSize(resolution);
            Log.Info($"分辨率改变 => {GameOption.Resolution}");
        };
        foreach (var resolutionListKey in Resolutions.Keys)
        {
            ResolutionBtn.AddItem(resolutionListKey);
        }
    }

    private void OnAudioValueChanged(double value, string busName)
    {
        var busIndex = AudioServer.GetBusIndex(busName);
        if (busIndex < 0)
        {
            Log.Error($"音频总线 {busName} 不存在");
        }
        else
        {
            var db = (float)Mathf.LinearToDb(value);
            AudioServer.SetBusVolumeDb(busIndex, db);
            Log.Info($"音频总线 {busName} 值 {value} 设置db {db}");
            switch (busName)
            {
                case "Master":
                    GameOption.AudioMaster = value;
                    break;
                case "Bgm":
                    GameOption.AudioBgm = value;
                    break;
                case "Sfx":
                    GameOption.AudioSfx = value;
                    break;
            }
        }
    }

    public async void ChangeScene(PackedScene scene)
    {
        var tree = GetTree();
        tree.Paused = true;
        var tween = CreateTween()
            .SetPauseMode(Tween.TweenPauseMode.Process)
            .TweenProperty(SimpleColorRect, NodePaths.ColorA, 1, 0.2f);
        await tree.ToSignal(tween, Tweener.SignalName.Finished);
        tree.ChangeSceneToPacked(scene);
        await tree.ToSignal(tree, SceneTree.SignalName.TreeChanged);
        tree.Paused = false;
        CreateTween().TweenProperty(SimpleColorRect, NodePaths.ColorA, 0, 0.2f);
    }
}