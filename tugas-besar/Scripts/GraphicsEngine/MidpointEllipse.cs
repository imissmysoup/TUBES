namespace EvacSimulator.GraphicsEngine;

using EvacSimulator.GraphicsEngine.Interfaces;
using Godot;
using System;
using System.Collections.Generic;

public class MidpointEllipse : IShapeRenderer
{
    public List<Vector2> GenerateShape(int centerX, int centerY, int rx, int ry)
    {
        var pts = new List<Vector2>();
        if (rx <= 0 || ry <= 0) return pts;

        long rx2 = (long)rx * rx;
        long ry2 = (long)ry * ry;
        long twoRx2 = 2 * rx2;
        long twoRy2 = 2 * ry2;

        int x = 0;
        int y = ry;
        long px = 0;
        long py = twoRx2 * y;

        PlotEllipsePoints(pts, centerX, centerY, x, y);

        // Region 1
        long p = (long)Math.Round(ry2 - (rx2 * ry) + (0.25 * rx2));
        while (px < py)
        {
            x++;
            px += twoRy2;
            if (p < 0)
            {
                p += ry2 + px;
            }
            else
            {
                y--;
                py -= twoRx2;
                p += ry2 + px - py;
            }
            PlotEllipsePoints(pts, centerX, centerY, x, y);
        }

        // Region 2
        p = (long)Math.Round(ry2 * (x + 0.5) * (x + 0.5) + rx2 * (y - 1) * (y - 1) - rx2 * ry2);
        while (y > 0)
        {
            y--;
            py -= twoRx2;
            if (p > 0)
            {
                p += rx2 - py;
            }
            else
            {
                x++;
                px += twoRy2;
                p += rx2 - py + px;
            }
            PlotEllipsePoints(pts, centerX, centerY, x, y);
        }

        return pts;
    }

    private void PlotEllipsePoints(List<Vector2> pts, int xc, int yc, int x, int y)
    {
        pts.Add(new Vector2(xc + x, yc + y));
        pts.Add(new Vector2(xc - x, yc + y));
        pts.Add(new Vector2(xc + x, yc - y));
        pts.Add(new Vector2(xc - x, yc - y));
    }
}