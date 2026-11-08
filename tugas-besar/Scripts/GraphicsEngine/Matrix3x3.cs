namespace EvacSimulator.GraphicsEngine;

using Godot;
using System;
using System.Collections.Generic;

public class Matrix3x3
{
    public float[,] M { get; private set; } = new float[3, 3];

    public Matrix3x3()
    {
        SetIdentity();
    }

    public void SetIdentity()
    {
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                M[r, c] = (r == c) ? 1.0f : 0.0f;
    }

    public static Matrix3x3 Identity() => new Matrix3x3();

    // Matriks Translasi T(dx, dy)
    public static Matrix3x3 CreateTranslation(float dx, float dy)
    {
        var mat = new Matrix3x3();
        mat.M[0, 2] = dx;
        mat.M[1, 2] = dy;
        return mat;
    }

    // Matriks Scaling S(sx, sy) terhadap (0,0)
    public static Matrix3x3 CreateScaling(float sx, float sy)
    {
        var mat = new Matrix3x3();
        mat.M[0, 0] = sx;
        mat.M[1, 1] = sy;
        return mat;
    }

    // Matriks Rotasi R(theta) dalam radian terhadap (0,0)
    public static Matrix3x3 CreateRotation(float angleRad)
    {
        var mat = new Matrix3x3();
        float cosA = MathF.Cos(angleRad);
        float sinA = MathF.Sin(angleRad);

        mat.M[0, 0] = cosA;
        mat.M[0, 1] = -sinA;
        mat.M[1, 0] = sinA;
        mat.M[1, 1] = cosA;
        return mat;
    }

    // Komposisi 3-Langkah Pivot: T(px, py) * Transform * T(-px, -py)
    public static Matrix3x3 CreatePivotTransformation(Vector2 pivot, float angleRad, Vector2 scale)
    {
        Matrix3x3 tToOrigin = CreateTranslation(-pivot.X, -pivot.Y);
        Matrix3x3 sMat = CreateScaling(scale.X, scale.Y);
        Matrix3x3 rMat = CreateRotation(angleRad);
        Matrix3x3 tBack = CreateTranslation(pivot.X, pivot.Y);

        return tBack * rMat * sMat * tToOrigin;
    }

    // Perkalian Matriks 3x3 (A * B)
    public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b)
    {
        var res = new Matrix3x3();
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                res.M[r, c] = a.M[r, 0] * b.M[0, c] +
                              a.M[r, 1] * b.M[1, c] +
                              a.M[r, 2] * b.M[2, c];
            }
        }
        return res;
    }

    // Mengalikan titik 2D [x, y, 1]^T dengan Matriks 3x3
    public Vector2 TransformPoint(Vector2 pt)
    {
        float newX = M[0, 0] * pt.X + M[0, 1] * pt.Y + M[0, 2];
        float newY = M[1, 0] * pt.X + M[1, 1] * pt.Y + M[1, 2];
        return new Vector2(newX, newY);
    }

    public List<Vector2> TransformPoints(List<Vector2> points)
    {
        var result = new List<Vector2>(points.Count);
        foreach (var pt in points)
        {
            result.Add(TransformPoint(pt));
        }
        return result;
    }
}