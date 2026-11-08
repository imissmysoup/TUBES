namespace EvacSimulator.GraphicsEngine.Interfaces;

using Godot;
using System.Collections.Generic;

public interface ILineRenderer
{
    List<Vector2> GenerateLine(int x0, int y0, int x1, int y1);
}