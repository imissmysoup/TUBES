namespace EvacSimulator.Core;

using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class BBoxData
{
    [JsonPropertyName("min")]
    public List<float> Min { get; set; } = new();

    [JsonPropertyName("max")]
    public List<float> Max { get; set; } = new();
}

public class HouseExpoRawData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("verts")]
    public List<List<float>> Verts { get; set; } = new();

    [JsonPropertyName("bbox")]
    public BBoxData BBox { get; set; } = new();
}

public class FloorplanModel
{
    public string Id { get; set; } = "";
    public List<Vector2> WallVertices { get; set; } = new();
    public Vector2 BBoxMin { get; set; }
    public Vector2 BBoxMax { get; set; }
    public Vector2 Center => (BBoxMin + BBoxMax) * 0.5f;
}

public static class DatasetParser
{
    public static FloorplanModel ParseHouseExpo(string filePath)
    {
        var model = new FloorplanModel();

        if (!FileAccess.FileExists(filePath))
        {
            GD.PrintErr($"[DatasetParser] File dataset tidak ditemukan: {filePath}");
            return model;
        }

        using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();

        try
        {
            var raw = JsonSerializer.Deserialize<HouseExpoRawData>(json);
            if (raw == null) return model;

            model.Id = raw.Id;

            if (raw.BBox?.Min?.Count >= 2 && raw.BBox?.Max?.Count >= 2)
            {
                model.BBoxMin = new Vector2(raw.BBox.Min[0], raw.BBox.Min[1]);
                model.BBoxMax = new Vector2(raw.BBox.Max[0], raw.BBox.Max[1]);
            }

            foreach (var v in raw.Verts)
            {
                if (v.Count >= 2)
                {
                    model.WallVertices.Add(new Vector2(v[0], v[1]));
                }
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[DatasetParser] Error saat parsing dataset: {ex.Message}");
        }

        return model;
    }
}