namespace EvacSimulator.Animation;

using EvacSimulator.Core;
using EvacSimulator.GraphicsEngine;
using EvacSimulator.GraphicsEngine.Interfaces;
using Godot;
using System;
using System.Collections.Generic;

public class AgentPathFollower : IAnimatable
{
	private readonly List<Vector2> _path;
	private readonly AppConfigData _cfg;
	private readonly Color _color;
	private readonly float _radiusWorld;

	private Vector2 _position;
	private float _angle;
	private int _target = 1;

	public bool Finished { get; private set; }

	public AgentPathFollower(List<Vector2> path, string colorHex, AppConfigData cfg)
	{
		_path = path;
		_cfg = cfg;
		_color = Color.FromHtml(colorHex);
		_radiusWorld = cfg.PixelsToWorld(cfg.ActorRadius);
		Reset();
	}

	public void Reset()
	{
		_position = _path.Count > 0 ? _path[0] : Vector2.Zero;
		_target = 1;
		Finished = _path.Count < 2;
		if (!Finished)
		{
			Vector2 d = _path[1] - _path[0];
			_angle = _cfg.ActorRotatesToHeading ? MathF.Atan2(d.Y, d.X) : 0f;
		}
	}

	public void UpdateAnimation(float dt, float totalTime)
	{
		if (Finished || totalTime < _cfg.AgentStartDelay) return;

		float remaining = _cfg.ActorMoveSpeed * dt;
		while (remaining > 0f && _target < _path.Count)
		{
			Vector2 toTarget = _path[_target] - _position;
			float dist = toTarget.Length();

			if (dist <= remaining)
			{
				_position = _path[_target];
				remaining -= dist;
				_target++;
				continue;
			}

			Vector2 dir = toTarget / dist;

			// Translasi T(dx, dy) lewat matriks
			Vector2 step = dir * remaining;
			_position = Matrix3x3.CreateTranslation(step.X, step.Y).TransformPoint(_position);
			remaining = 0f;

			// Rotasi halus menuju arah gerak (opsional via config)
			if (_cfg.ActorRotatesToHeading)
			{
				float targetAngle = MathF.Atan2(dir.Y, dir.X);
				float diff = MathF.IEEERemainder(targetAngle - _angle, 2f * MathF.PI);
				float maxTurn = _cfg.ActorTurnSpeedDeg * MathF.PI / 180f * dt;
				_angle += Math.Clamp(diff, -maxTurn, maxTurn);
			}
		}

		if (_target >= _path.Count) Finished = true;
	}

	public List<Vector2> GetRenderPixels(Matrix3x3 viewMatrix, ILineRenderer lineRenderer)
	{
		var pixels = new List<Vector2>();

		float radiusPx = _radiusWorld * viewMatrix.GetScaleX();
		int rings = (int)MathF.Ceiling(radiusPx / _cfg.PixelSize);

		// Belah ketupat: 4 titik sudut pada sumbu lokal
		Vector2[] local =
		{
			new Vector2(_radiusWorld, 0f),
			new Vector2(0f, _radiusWorld),
			new Vector2(-_radiusWorld, 0f),
			new Vector2(0f, -_radiusWorld)
		};

		for (int i = 1; i <= rings; i++)
		{
			float f = (float)i / rings;

			// T(pos) * R(theta) * S(f, f)
			Matrix3x3 mvp = viewMatrix *
				CompositeAnimator.Compose(_position, _angle, new Vector2(f, f), Vector2.Zero);

			for (int v = 0; v < local.Length; v++)
			{
				Vector2 a = mvp.TransformPoint(local[v]);
				Vector2 b = mvp.TransformPoint(local[(v + 1) % local.Length]);

				pixels.AddRange(lineRenderer.GenerateLine(
					(int)MathF.Round(a.X), (int)MathF.Round(a.Y),
					(int)MathF.Round(b.X), (int)MathF.Round(b.Y)));
			}
		}

		return pixels;
	}

	public Color GetRenderColor() => _color;
}
