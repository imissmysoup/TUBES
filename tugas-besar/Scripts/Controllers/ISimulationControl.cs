namespace EvacSimulator.Controllers;

public interface ISimulationControl
{
	bool IsPlaying { get; }
	string GetActiveAlgorithmName();
	void ToggleLineAlgorithm();
	void TogglePlayPause();
	void RestartSimulation();
}