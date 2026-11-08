namespace EvacSimulator.Animation;

using EvacSimulator.GraphicsEngine;
using EvacSimulator.GraphicsEngine.Interfaces;
using Godot;
using System.Collections.Generic;

public class CompositeAnimator
{
	private readonly List<IAnimatable> _objects = new();

	public IReadOnlyList<IAnimatable> Objects => _objects;
	public bool IsPlaying { get; private set; }
	public float TotalTime { get; private set; }

	public void Register(IAnimatable a) { if (!_objects.Contains(a)) _objects.Add(a); }
	public void Clear() { _objects.Clear(); TotalTime = 0f; }
	public void Play() => IsPlaying = true;
	public void Pause() => IsPlaying = false;

	public void Reset()
	{
		TotalTime = 0f;
		foreach (var o in _objects) o.Reset();
	}

	public void Update(float dt)
	{
		if (!IsPlaying) return;
		TotalTime += dt;
		foreach (var o in _objects) o.UpdateAnimation(dt, TotalTime);
	}

	// M_anim = T(pos) * R(theta) * S(scale) * T(-pivot)
	public static Matrix3x3 Compose(Vector2 position, float angleRad, Vector2 scale, Vector2 pivot)
	{
		return Matrix3x3.CreateTranslation(position.X, position.Y)
			 * Matrix3x3.CreateRotation(angleRad)
			 * Matrix3x3.CreateScaling(scale.X, scale.Y)
			 * Matrix3x3.CreateTranslation(-pivot.X, -pivot.Y);
	}
}