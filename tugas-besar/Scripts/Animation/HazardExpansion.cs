namespace EvacSimulator.Animation;

using EvacSimulator.Core;
using EvacSimulator.GraphicsEngine;
using EvacSimulator.GraphicsEngine.Interfaces;
using Godot;
using System;
using System.Collections.Generic;

public class FireHazard : IAnimatable
{
	private readonly Vector2 _origin;
	private readonly IShapeRenderer _circle;
	private readonly AppConfigData _cfg;
	private readonly Color _color;
	private readonly float _baseRadiusWorld;
	private float _scale = 1f;

	public FireHazard(Vector2 origin, IShapeRenderer circle, AppConfigData cfg)
	{
		_origin = origin; _circle = circle; _cfg = cfg;
		_color = Color.FromHtml(cfg.FireColorHex);
		_baseRadiusWorld = cfg.PixelsToWorld(cfg.FireBaseRadius);
	}

	public void Reset() => _scale = 1f;

	public void UpdateAnimation(float dt, float totalTime)
	{
		float growth = MathF.Min(1f + _cfg.FireGrowthRate * totalTime, _cfg.FireMaxScale);
		float pulse = 1f + _cfg.FirePulseAmplitude * MathF.Sin(_cfg.FirePulseFrequency * totalTime);
		_scale = growth * pulse; // S(1 + e sin(wt))
	}

	public List<Vector2> GetRenderPixels(Matrix3x3 viewMatrix, ILineRenderer lineRenderer)
	{
		Matrix3x3 model = CompositeAnimator.Compose(_origin, 0f, new Vector2(_scale, _scale), _origin);
		Matrix3x3 mvp = viewMatrix * model;

		Vector2 c = mvp.TransformPoint(_origin);
		float r = _baseRadiusWorld * mvp.GetScaleX();

		var pixels = new List<Vector2>();
		for (int k = 1; k <= _cfg.FireRingCount; k++)
		{
			int rk = (int)MathF.Round(r * k / _cfg.FireRingCount);
			pixels.AddRange(_circle.GenerateShape((int)MathF.Round(c.X), (int)MathF.Round(c.Y), rk));
		}
		return pixels;
	}

	public Color GetRenderColor() => _color;
}

public class SmokeHazard : IAnimatable
{
	private readonly Vector2 _origin;
	private readonly IShapeRenderer _ellipse;
	private readonly AppConfigData _cfg;
	private readonly Color _color;
	private readonly float _baseRxWorld, _baseRyWorld, _driftWorld, _windRad;
	private readonly Vector2 _windDir;

	private Vector2 _center;
	private float _scale = 1f;
	private bool _visible;

	public SmokeHazard(Vector2 origin, IShapeRenderer ellipse, AppConfigData cfg)
	{
		_origin = origin; _ellipse = ellipse; _cfg = cfg;
		_color = Color.FromHtml(cfg.SmokeColorHex);
		_baseRxWorld = cfg.PixelsToWorld(cfg.SmokeBaseRx);
		_baseRyWorld = cfg.PixelsToWorld(cfg.SmokeBaseRy);
		_driftWorld = cfg.PixelsToWorld(cfg.SmokeDriftSpeed);
		_windRad = cfg.SmokeWindAngleDeg * MathF.PI / 180f;
		_windDir = new Vector2(MathF.Cos(_windRad), MathF.Sin(_windRad));
		Reset();
	}

	public void Reset() { _center = _origin; _scale = 1f; _visible = false; }

	public void UpdateAnimation(float dt, float totalTime)
	{
		float t = totalTime - _cfg.SmokeStartDelay;
		_visible = t >= 0f;
		if (!_visible) return;

		_scale = MathF.Min(1f + _cfg.SmokeExpansionRate * t, _cfg.SmokeMaxScale);
		Vector2 drift = _windDir * (_driftWorld * t);
		_center = Matrix3x3.CreateTranslation(drift.X, drift.Y).TransformPoint(_origin);
	}

	public List<Vector2> GetRenderPixels(Matrix3x3 viewMatrix, ILineRenderer lineRenderer)
	{
		if (!_visible) return new List<Vector2>();

		// T(center) * R(wind) * S(scale)
		Matrix3x3 model = CompositeAnimator.Compose(_center, _windRad, new Vector2(_scale, _scale), Vector2.Zero);
		Matrix3x3 mvp = viewMatrix * model;

		Vector2 c = mvp.TransformPoint(Vector2.Zero);
		int rx = Math.Max(1, (int)MathF.Round(_baseRxWorld * mvp.GetScaleX()));
		int ry = Math.Max(1, (int)MathF.Round(_baseRyWorld * mvp.GetScaleY()));

		// Elips axis-aligned di titik (0,0), lalu diputar & ditranslasi dengan matriks
		var local = _ellipse.GenerateShape(0, 0, rx, ry);
		Matrix3x3 place = Matrix3x3.CreateTranslation(c.X, c.Y) * Matrix3x3.CreateRotation(mvp.GetRotation());
		return place.TransformPoints(local);
	}

	public Color GetRenderColor() => _color;
}