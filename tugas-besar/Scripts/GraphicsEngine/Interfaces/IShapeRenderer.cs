namespace EvacSimulator.GraphicsEngine.Interfaces;

using Godot;
using System.Collections.Generic;

public interface IShapeRenderer
{
    List<Vector2> GenerateShape(int centerX, int centerY, int paramA, int paramB = 0);
}