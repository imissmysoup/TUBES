namespace EvacSimulator.GraphicsEngine;

using EvacSimulator.GraphicsEngine.Interfaces;
using Godot;
using System;
using System.Collections.Generic;

public class DDALine : ILineRenderer
{
    public List<Vector2> GenerateLine(int x0, int y0, int x1, int y1)
    {
        var pts = new List<Vector2>();
        int dx = x1 - x0;
        int dy = y1 - y0;
        int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

        if (steps == 0)
        {
            pts.Add(new Vector2(x0, y0));
            return pts;
        }

        float xInc = (float)dx / steps;
        float yInc = (float)dy / steps;

        float x = x0;
        float y = y0;

        for (int i = 0; i <= steps; i++)
        {
            pts.Add(new Vector2(MathF.Round(x), MathF.Round(y)));
            x += xInc;
            y += yInc;
        }

        return pts;
    }
}