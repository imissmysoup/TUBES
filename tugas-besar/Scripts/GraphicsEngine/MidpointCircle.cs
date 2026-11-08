namespace EvacSimulator.GraphicsEngine;

using EvacSimulator.GraphicsEngine.Interfaces;
using Godot;
using System.Collections.Generic;

public class MidpointCircle : IShapeRenderer
{
    public List<Vector2> GenerateShape(int centerX, int centerY, int radius, int unused = 0)
    {
        var pts = new List<Vector2>();
        if (radius <= 0) return pts;

        int x = 0;
        int y = radius;
        int p = 1 - radius;

        PlotCirclePoints(pts, centerX, centerY, x, y);

        while (x < y)
        {
            x++;
            if (p < 0)
            {
                p += 2 * x + 1;
            }
            else
            {
                y--;
                p += 2 * (x - y) + 1;
            }
            PlotCirclePoints(pts, centerX, centerY, x, y);
        }

        return pts;
    }

    private void PlotCirclePoints(List<Vector2> pts, int xc, int yc, int x, int y)
    {
        pts.Add(new Vector2(xc + x, yc + y));
        pts.Add(new Vector2(xc - x, yc + y));
        pts.Add(new Vector2(xc + x, yc - y));
        pts.Add(new Vector2(xc - x, yc - y));
        pts.Add(new Vector2(xc + y, yc + x));
        pts.Add(new Vector2(xc - y, yc + x));
        pts.Add(new Vector2(xc + y, yc - x));
        pts.Add(new Vector2(xc - y, yc - x));
    }
}