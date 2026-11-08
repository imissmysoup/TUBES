namespace EvacSimulator.Animation;

using EvacSimulator.Core;
using EvacSimulator.GraphicsEngine;
using EvacSimulator.GraphicsEngine.Interfaces;
using Godot;
using System;
using System.Collections.Generic;

public class AlarmIndicator : IAnimatable
{
	private readonly Vector2 _pivot;
	private readonly IShapeRenderer _circle;
	private readonly AppConfigData _cfg;
	private readonly Color _color;
	private readonly float _radiusWorld;
	private float _theta;
	private float _scale = 1f;

	public AlarmIndicator(Vector2 pivot, IShapeRenderer circle, AppConfigData cfg)
	{
		_pivot = pivot; _circle = circle; _cfg = cfg;
		_color = Color.FromHtml(cfg.AlarmColorHex);
		_radiusWorld = cfg.PixelsToWorld(cfg.AlarmRadius);
	}

	public void Reset() { _theta = 0f; _scale = 1f; }

	public void UpdateAnimation(float dt, float totalTime)
	{
		if (totalTime < _cfg.AlarmActivationDelay) { Reset(); return; }

		float active = totalTime - _cfg.AlarmActivationDelay;
		_theta = _cfg.AlarmRotationSpeedDeg * MathF.PI / 180f * active;
		_scale = 1f + _cfg.AlarmPulseAmplitude * MathF.Sin(_cfg.AlarmPulseFrequency * totalTime);
	}

	public List<Vector2> GetRenderPixels(Matrix3x3 viewMatrix, ILineRenderer lineRenderer)
	{
		// T(p) * R(theta) * S(s) * T(-p)
		Matrix3x3 model = CompositeAnimator.Compose(_pivot, _theta, new Vector2(_scale, _scale), _pivot);
		Matrix3x3 mvp = viewMatrix * model;

		Vector2 c = mvp.TransformPoint(_pivot);
		int r = Math.Max(1, (int)MathF.Round(_radiusWorld * mvp.GetScaleX()));
		var pixels = _circle.GenerateShape((int)MathF.Round(c.X), (int)MathF.Round(c.Y), r);

		// Garis sapuan radar
		Vector2 tip = mvp.TransformPoint(_pivot + new Vector2(_radiusWorld, 0f));
		pixels.AddRange(lineRenderer.GenerateLine(
			(int)MathF.Round(c.X), (int)MathF.Round(c.Y),
			(int)MathF.Round(tip.X), (int)MathF.Round(tip.Y)));
		return pixels;
	}

	public Color GetRenderColor() => _color;
}