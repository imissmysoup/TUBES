using Godot;

namespace EvacSimulator.Controllers
{
	public partial class HudPanel : Control
	{
		private ISimulationControl _sim;
		private Button _algoButton;
		private Button _playButton;

		public void Init(ISimulationControl sim) => _sim = sim;

		public override void _Ready()
		{
			var box = new VBoxContainer();
			AddChild(box);

			_algoButton = new Button();
			_algoButton.Pressed += () => { _sim.ToggleLineAlgorithm(); Refresh(); };
			box.AddChild(_algoButton);

			if (_sim.HasAnimation)
			{
				_playButton = new Button();
				_playButton.Pressed += () => { _sim.TogglePlayPause(); Refresh(); };
				box.AddChild(_playButton);

				var restartButton = new Button { Text = "Restart" };
				restartButton.Pressed += () => { _sim.RestartSimulation(); Refresh(); };
				box.AddChild(restartButton);
			}

			var backButton = new Button { Text = "Back to Main Menu" };
			backButton.Pressed += () => _sim.BackToMenu();
			box.AddChild(backButton);

			Refresh();
		}

		private void Refresh()
		{
			_algoButton.Text = $"Garis: {_sim.GetActiveAlgorithmName()}";
			if (_playButton != null)
				_playButton.Text = _sim.IsPlaying ? "Pause" : "Play";
		}
	}
}