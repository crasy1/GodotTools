namespace Godot;

// This code will not compile until you build the project with the Source Generators

[Singleton]
public partial class SampleEntity
{
    public int Id { get; } = 42;
    public string? Name { get; } = "Sample";

    public void Test()
    {
        var id = Instance.Id;
    }
}