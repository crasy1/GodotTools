using System;
using Godot;

[SceneTree]
[Singleton]
public partial class SceneManager : CanvasLayer
{
    
    [Signal]
    public delegate void SceneChangedEventHandler();

    private SceneTree SceneTree { set; get; }

    public override void _Ready()
    {
        SceneTree = GetTree();
        SceneChanged += () => { TipLabel.SelfModulate = TipLabel.SelfModulate with { A = 0 }; };
        TipLabel.SelfModulate = Colors.White with { A = 0 };
        SimpleColorRect.Color = SimpleColorRect.Color with { A = 0 };
    }

    /// <summary>
    /// 简单转场效果
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="tips"></param>
    /// <param name="duration"></param>
    /// <param name="showTip"></param>
    public async void SimpleColorChange(PackedScene scene, string tips = null, double duration = 0.4,
        bool showTip = true)
    {
        StopBgm();
        SceneTree.Paused = true;
        // StopBgm();
        if (showTip)
        {
            ShowTips(tips);
        }

        var tween = CreateTween().SetPauseMode(Tween.TweenPauseMode.Process);
        tween.TweenProperty(SimpleColorRect, NodePaths.ColorA, 1, duration);
        await ToSignal(tween, Tween.SignalName.Finished);
        var beforeSceneName = GetTree().CurrentScene.Name;
        SceneTree.ChangeSceneToPacked(scene);
        await ToSignal(SceneTree, SceneTree.SignalName.TreeChanged);
        EmitSignal(SignalName.SceneChanged);
        GD.Print($"切换场景 {beforeSceneName} => {GetTree().CurrentScene.Name}");
        tween = CreateTween().SetPauseMode(Tween.TweenPauseMode.Process);
        tween.TweenProperty(SimpleColorRect, NodePaths.ColorA, 0, duration);
        await ToSignal(tween, Tween.SignalName.Finished);
        SceneTree.Paused = false;
    }

    /// <summary>
    /// 简单转场效果
    /// </summary>
    /// <param name="action"></param>
    /// <param name="tips"></param>
    /// <param name="duration"></param>
    /// <param name="showTip"></param>
    public async void SimpleColorChange(Action action, string tips = null, double duration = 0.4, bool showTip = true)
    {
        StopBgm();
        SceneTree.Paused = true;
        if (showTip)
        {
            ShowTips(tips);
        }

        var tween = CreateTween().SetPauseMode(Tween.TweenPauseMode.Process);
        tween.TweenProperty(SimpleColorRect, NodePaths.ColorA, 1, duration);
        await ToSignal(tween, Tween.SignalName.Finished);
        action.Invoke();
        tween = CreateTween().SetPauseMode(Tween.TweenPauseMode.Process);
        tween.TweenProperty(SimpleColorRect, NodePaths.ColorA, 0, duration);
        await ToSignal(tween, Tween.SignalName.Finished);
        SceneTree.Paused = false;
    }

    public void ShowTips(string tips, double duration = 0.4)
    {
        if (string.IsNullOrEmpty(tips))
        {
            TipLabel.Text = "";
            return;
        }

        TipLabel.Text = tips;
        CreateTween().SetPauseMode(Tween.TweenPauseMode.Process)
            .TweenProperty(TipLabel, NodePaths.SelfModulateA, 1, duration);
    }

    public void StopBgm(double duration = 0.4)
    {
        if (!BgmPlayer.Playing)
        {
            return;
        }

        var tween = CreateTween();
        tween.TweenProperty(BgmPlayer, NodePaths.VolumeDb, Mathf.LinearToDb(0), duration);
        tween.Chain().TweenCallback(Callable.From(BgmPlayer.Stop));
    }

    public void PlayBgm(AudioStream audioStream, double duration = 0.4)
    {
        BgmPlayer.Stream = audioStream;
        BgmPlayer.Play();
        var tween = CreateTween();
        tween.TweenProperty(BgmPlayer, NodePaths.VolumeDb, Mathf.LinearToDb(1), duration);
    }
}