namespace EvacSimulator.Core;

using EvacSimulator.Animation;
using EvacSimulator.GraphicsEngine.Interfaces;
using Godot;
using System.Collections.Generic;
using System.Linq;

public static class SimulationFactory
{
	public static List<IAnimatable> Build(AppConfigData cfg, IShapeRenderer circle, IShapeRenderer ellipse)
	{
		Vector2 fire = cfg.FireOrigin.ToVector2();
		var list = new List<IAnimatable>
		{
			new FireHazard(fire, circle, cfg),
			new SmokeHazard(fire, ellipse, cfg),
			new AgentPathFollower(cfg.ActorAPath.Select(p => p.ToVector2()).ToList(), cfg.ActorAColorHex, cfg),
			new AgentPathFollower(cfg.ActorBPath.Select(p => p.ToVector2()).ToList(), cfg.ActorBColorHex, cfg)
		};

		foreach (var pos in cfg.AlarmPositions)
			list.Add(new AlarmIndicator(pos.ToVector2(), circle, cfg));

		return list;
	}
}
