using Godot;
using System;
using EvacSimulator.Core;

[GlobalClass]
public partial class MainMenu : Control
{
	public override void _Ready()
	{
		ConfigLoader.Load();
		SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

		var center = new CenterContainer();
		AddChild(center);
		center.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

		var box = new VBoxContainer();
		center.AddChild(box);

		box.AddChild(new Label
		{
			Text = "SIMULASI EVAKUASI KEBAKARAN",
			HorizontalAlignment = HorizontalAlignment.Center
		});

		AddMenuButton(box, "Mulai Simulasi", () => Open(SimulationMode.Simulation));
		AddMenuButton(box, "Lihat Denah", () => Open(SimulationMode.FloorplanOnly));
		AddMenuButton(box, "Keluar", () => GetTree().Quit());
	}

	private void AddMenuButton(Container parent, string text, Action onPressed)
	{
		var button = new Button { Text = text };
		button.Pressed += onPressed;
		parent.AddChild(button);
	}

	private void Open(SimulationMode mode)
	{
		AppState.Mode = mode;
		GetTree().ChangeSceneToFile(ConfigLoader.Config.SimulatorScenePath);
	}
}
