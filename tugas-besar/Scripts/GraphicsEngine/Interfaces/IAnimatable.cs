namespace EvacSimulator.GraphicsEngine.Interfaces;

using Godot;
using System.Collections.Generic;

public interface IAnimatable
{
    void UpdateAnimation(float deltaTime, float totalTime);
    List<Vector2> GetRenderPixels(Matrix3x3 viewMatrix, ILineRenderer lineRenderer);
    Color GetRenderColor();
    void Reset();
}