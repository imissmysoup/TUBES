namespace EvacSimulator.Core;

using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;

public class AppConfigData
{
	public string DatasetPath { get; set; } = "res://Datasets/1966cbb279b58258eed0405669915734.json";
	public string MenuScenePath { get; set; } = "res://Scenes/MainMenu.tscn";
	public string SimulatorScenePath { get; set; } = "res://Scenes/MainSimulator.tscn";
	public int DefaultWidth { get; set; } = 1280;
	public int DefaultHeight { get; set; } = 720;
	public string WallColorHex { get; set; } = "#FFFFFF";
	public string FireColorHex { get; set; } = "#FF2222";
	public string SmokeColorHex { get; set; } = "#777777";
	public string ActorAColorHex { get; set; } = "#FFD700";
	public string ActorBColorHex { get; set; } = "#00E5FF";
	public string AlarmColorHex { get; set; } = "#3322FF";
	public float ZoomSpeed { get; set; } = 0.15f;
	public float MinZoom { get; set; } = 5.0f;
	public float MaxZoom { get; set; } = 100.0f;
	public float DefaultScale { get; set; } = 38.0f;
	public float PanSpeed { get; set; } = 1.0f;
	public float RotateSpeedDeg { get; set; } = 15.0f;
	public float FireBaseRadius { get; set; } = 25.0f;
	public float FirePulseAmplitude { get; set; } = 0.35f;
	public float FirePulseFrequency { get; set; } = 4.0f;
	public float SmokeBaseRx { get; set; } = 40.0f;
	public float SmokeBaseRy { get; set; } = 22.0f;
	public float SmokeExpansionRate { get; set; } = 0.25f;
	public float SmokeDriftSpeed { get; set; } = 12.0f;
	public float SmokeWindAngleDeg { get; set; } = -30.0f;
	public float ActorRadius { get; set; } = 10.0f;
	public float ActorMoveSpeed { get; set; } = 1.8f;
	public float AlarmRadius { get; set; } = 16.0f;
	public float AlarmRotationSpeedDeg { get; set; } = 180.0f;
	public class PointConfig {
		public float X { get; set; }
		public float Y { get; set; }
		public Vector2 ToVector2() => new Vector2(X, Y);
	}
	public float PixelSize { get; set; } = 2.0f;
	public float ActorHeadingFactor { get; set; } = 1.8f;
	public bool ActorRotatesToHeading { get; set; } = true;
	public float ActorTurnSpeedDeg { get; set; } = 360.0f;
	public float AgentStartDelay { get; set; } = 2.5f;
	public float FireGrowthRate { get; set; } = 0.1f;
	public float FireMaxScale { get; set; } = 2.0f;
	public int FireRingCount { get; set; } = 3;
	public float SmokeStartDelay { get; set; } = 1.5f;
	public float SmokeMaxScale { get; set; } = 4.0f;
	public float AlarmActivationDelay { get; set; } = 1.0f;
	public float AlarmPulseAmplitude { get; set; } = 0.25f;
	public float AlarmPulseFrequency { get; set; } = 6.0f;
	public PointConfig FireOrigin { get; set; } = new();
	public List<PointConfig> AlarmPositions { get; set; } = new();
	public List<PointConfig> ActorAPath { get; set; } = new();
	public List<PointConfig> ActorBPath { get; set; } = new();

	public float PixelsToWorld(float px) => px / DefaultScale;
}

public static class ConfigLoader
{
	public static AppConfigData Config { get; private set; } = new AppConfigData();

	public static void Load(string configPath = "res://Config/app_config.json")
	{
		if (!FileAccess.FileExists(configPath))
		{
			GD.PrintErr($"[ConfigLoader] File tidak ditemukan: {configPath}. Menggunakan nilai fallback.");
			return;
		}

		using var file = FileAccess.Open(configPath, FileAccess.ModeFlags.Read);
		string json = file.GetAsText();

		try
		{
			var parsed = JsonSerializer.Deserialize<AppConfigData>(json);
			if (parsed != null) Config = parsed;
		}
		catch (Exception ex)
		{
			GD.PrintErr($"[ConfigLoader] Gagal mem-parsing JSON: {ex.Message}");
		}
	}
}
