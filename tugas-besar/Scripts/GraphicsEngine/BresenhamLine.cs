namespace EvacSimulator.GraphicsEngine;

using EvacSimulator.GraphicsEngine.Interfaces;
using Godot;
using System;
using System.Collections.Generic;

public class BresenhamLine : ILineRenderer
{
    public List<Vector2> GenerateLine(int x0, int y0, int x1, int y1)
    {
        var pts = new List<Vector2>();
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        int x = x0;
        int y = y0;

        while (true)
        {
            pts.Add(new Vector2(x, y));
            if (x == x1 && y == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }

        return pts;
    }
}