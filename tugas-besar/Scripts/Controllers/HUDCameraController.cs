using Godot;
using System;
using EvacSimulator.Core;
using EvacSimulator.GraphicsEngine;

namespace EvacSimulator.Controllers
{
	public class HUDCameraController
	{
		public float ZoomFactor { get; private set; } = 1.0f;
		public Vector2 PanOffset { get; private set; } = Vector2.Zero;
		public float RotationAngleRad { get; private set; } = 0.0f;

		private Vector2 _floorplanCenter = Vector2.Zero;
		private bool _isDragging = false;
		private Vector2 _lastMousePos = Vector2.Zero;

		public void Initialize(Vector2 floorplanCenter)
		{
			_floorplanCenter = floorplanCenter;
			ResetView();
		}

		public void ResetView()
		{
			ZoomFactor = 1.0f;
			PanOffset = Vector2.Zero;
			RotationAngleRad = 0.0f;
		}

		public void ZoomIn()
		{
			var cfg = ConfigLoader.Config;
			float nextScale = (ZoomFactor * (1.0f + cfg.ZoomSpeed)) * cfg.DefaultScale;
			if (nextScale <= cfg.MaxZoom)
			{
				ZoomFactor *= (1.0f + cfg.ZoomSpeed);
			}
		}

		public void ZoomOut()
		{
			var cfg = ConfigLoader.Config;
			float nextScale = (ZoomFactor * (1.0f - cfg.ZoomSpeed)) * cfg.DefaultScale;
			if (nextScale >= cfg.MinZoom)
			{
				ZoomFactor *= (1.0f - cfg.ZoomSpeed);
			}
		}

		public void RotateClockwise()
		{
			float stepRad = ConfigLoader.Config.RotateSpeedDeg * MathF.PI / 180.0f;
			RotationAngleRad += stepRad;
		}

		public void RotateCounterClockwise()
		{
			float stepRad = ConfigLoader.Config.RotateSpeedDeg * MathF.PI / 180.0f;
			RotationAngleRad -= stepRad;
		}

		public void Pan(Vector2 deltaScreenPixels)
		{
			PanOffset += deltaScreenPixels * ConfigLoader.Config.PanSpeed;
		}

		public bool HandleInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton mouseBtn)
			{
				if (mouseBtn.ButtonIndex == MouseButton.WheelUp && mouseBtn.Pressed)
				{
					ZoomIn();
					return true;
				}
				if (mouseBtn.ButtonIndex == MouseButton.WheelDown && mouseBtn.Pressed)
				{
					ZoomOut();
					return true;
				}
				if (mouseBtn.ButtonIndex == MouseButton.Left || mouseBtn.ButtonIndex == MouseButton.Middle)
				{
					_isDragging = mouseBtn.Pressed;
					_lastMousePos = mouseBtn.Position;
					return true;
				}
			}
			else if (@event is InputEventMouseMotion mouseMotion && _isDragging)
			{
				Vector2 delta = mouseMotion.Position - _lastMousePos;
				_lastMousePos = mouseMotion.Position;
				Pan(delta);
				return true;
			}
			else if (@event is InputEventKey keyEvent && keyEvent.Pressed)
			{
				switch (keyEvent.Keycode)
				{
					case Key.Q:
						RotateCounterClockwise();
						return true;
					case Key.E:
						RotateClockwise();
						return true;
					case Key.R:
						ResetView();
						return true;
				}
			}

			return false;
		}

		public Matrix3x3 GetViewMatrix(Vector2 currentViewportSize)
		{
			var cfg = ConfigLoader.Config;

			float ratioX = currentViewportSize.X / cfg.DefaultWidth;
			float ratioY = currentViewportSize.Y / cfg.DefaultHeight;
			float windowResizeScale = MathF.Min(ratioX, ratioY);

			float finalScale = cfg.DefaultScale * ZoomFactor * windowResizeScale;
			Vector2 screenCenter = currentViewportSize * 0.5f;

			Matrix3x3 tToOrigin = Matrix3x3.CreateTranslation(-_floorplanCenter.X, -_floorplanCenter.Y);
			Matrix3x3 sMatrix = Matrix3x3.CreateScaling(finalScale, finalScale);
			Matrix3x3 rMatrix = Matrix3x3.CreateRotation(RotationAngleRad);
			Matrix3x3 tToScreen = Matrix3x3.CreateTranslation(
				screenCenter.X + PanOffset.X,
				screenCenter.Y + PanOffset.Y
			);

			return tToScreen * rMatrix * sMatrix * tToOrigin;
		}
	}
}
