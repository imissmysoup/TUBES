using Godot;
using System;
using System.Collections.Generic;
using EvacSimulator.Core;
using EvacSimulator.Animation;
using EvacSimulator.GraphicsEngine;
using EvacSimulator.GraphicsEngine.Interfaces;
using EvacSimulator.Controllers;

[GlobalClass]
public partial class FloorplanRenderer : Node2D, ISimulationControl
{
	public bool UseBresenham { get; private set; } = true;
	public HUDCameraController CameraController { get; private set; } = new HUDCameraController();
	public CompositeAnimator Animator { get; } = new CompositeAnimator();

	private FloorplanModel _floorplan = new FloorplanModel();
	private readonly ILineRenderer _ddaRenderer = new DDALine();
	private readonly ILineRenderer _bresenhamRenderer = new BresenhamLine();

	private Color _wallColor = Colors.White;
	private float _pixelSize;

	public bool IsPlaying => Animator.IsPlaying;

	public void TogglePlayPause()
	{
		if (Animator.IsPlaying) Animator.Pause();
		else Animator.Play();
	}

	public void RestartSimulation()
	{
		Animator.Reset();
		QueueRedraw();
	}

	public override void _Ready()
	{
		ConfigLoader.Load();
		var cfg = ConfigLoader.Config;

		_wallColor = Color.FromHtml(cfg.WallColorHex);
		_pixelSize = cfg.PixelSize;

		_floorplan = DatasetParser.ParseHouseExpo(cfg.DatasetPath);
		CameraController.Initialize(_floorplan.Center);

		foreach (var obj in SimulationFactory.Build(cfg, new MidpointCircle(), new MidpointEllipse()))
		{
			Animator.Register(obj);
		}
		if (HasAnimation)
		{
		foreach (var obj in SimulationFactory.Build(cfg, new MidpointCircle(), new MidpointEllipse()))
		{
			Animator.Register(obj);
		}
		Animator.Play();
		}
		var hudLayer = new CanvasLayer();
		AddChild(hudLayer);
		var hud = new HudPanel();
		hud.Init(this);
		hudLayer.AddChild(hud);

		GetTree().Root.SizeChanged += OnWindowSizeChanged;

		QueueRedraw();
	}

	public override void _ExitTree()
	{
		if (GetTree()?.Root != null)
		{
			GetTree().Root.SizeChanged -= OnWindowSizeChanged;
		}
	}

	public override void _Process(double delta)
	{
		if (Animator.IsPlaying)
		{
			Animator.Update((float)delta);
			QueueRedraw();
		}
	}

	private void OnWindowSizeChanged()
	{
		QueueRedraw();
	}

	public void ToggleLineAlgorithm()
	{
		UseBresenham = !UseBresenham;
		QueueRedraw();
	}

	public void SetLineAlgorithm(bool useBresenham)
	{
		UseBresenham = useBresenham;
		QueueRedraw();
	}

	public string GetActiveAlgorithmName() => UseBresenham ? "Bresenham" : "DDA";

	public void RegisterAnimatable(IAnimatable animatable)
	{
		Animator.Register(animatable);
		QueueRedraw();
	}

	public void ClearAnimatables()
	{
		Animator.Clear();
		QueueRedraw();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		// Debug: klik kanan untuk mencetak koordinat dunia (untuk menentukan jalur)
		if (@event is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Right)
		{
			var inv = CameraController.GetViewMatrix(GetViewportRect().Size).Inverse();
			Vector2 w = inv.TransformPoint(mb.Position);
			GD.Print($"World: ({w.X:F2}, {w.Y:F2})");
			return;
		}

		if (CameraController.HandleInput(@event))
		{
			QueueRedraw();
		}
	}

	public override void _Draw()
	{
		if (_floorplan.WallVertices.Count < 2) return;

		Vector2 viewportSize = GetViewportRect().Size;
		Matrix3x3 viewMatrix = CameraController.GetViewMatrix(viewportSize);
		ILineRenderer activeLineRenderer = UseBresenham ? _bresenhamRenderer : _ddaRenderer;

		DrawFloorplanWalls(viewMatrix, activeLineRenderer);

		foreach (var animObj in Animator.Objects)
		{
			List<Vector2> pixels = animObj.GetRenderPixels(viewMatrix, activeLineRenderer);
			Color objColor = animObj.GetRenderColor();
			DrawPixelBatch(pixels, objColor, _pixelSize);
		}
	}

	private void DrawFloorplanWalls(Matrix3x3 viewMatrix, ILineRenderer lineRenderer)
	{
		int count = _floorplan.WallVertices.Count;
		var wallPixels = new List<Vector2>();

		for (int i = 0; i < count; i++)
		{
			Vector2 worldP1 = _floorplan.WallVertices[i];
			Vector2 worldP2 = _floorplan.WallVertices[(i + 1) % count];

			Vector2 screenP1 = viewMatrix.TransformPoint(worldP1);
			Vector2 screenP2 = viewMatrix.TransformPoint(worldP2);

			int x0 = (int)MathF.Round(screenP1.X);
			int y0 = (int)MathF.Round(screenP1.Y);
			int x1 = (int)MathF.Round(screenP2.X);
			int y1 = (int)MathF.Round(screenP2.Y);

			wallPixels.AddRange(lineRenderer.GenerateLine(x0, y0, x1, y1));
		}

		DrawPixelBatch(wallPixels, _wallColor, _pixelSize);
	}

	private void DrawPixelBatch(List<Vector2> pixels, Color color, float size)
	{
		Vector2 rectSize = new Vector2(size, size);
		foreach (var pt in pixels)
		{
			DrawRect(new Rect2(pt, rectSize), color);
		}
	}

	public bool HasAnimation => AppState.Mode == SimulationMode.Simulation;

	public void BackToMenu()
	{
		GetTree().ChangeSceneToFile(ConfigLoader.Config.MenuScenePath);
	}
}
