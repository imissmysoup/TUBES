namespace EvacSimulator.Core;

public enum SimulationMode { Simulation, FloorplanOnly }

public static class AppState
{
	public static SimulationMode Mode { get; set; } = SimulationMode.Simulation;
}