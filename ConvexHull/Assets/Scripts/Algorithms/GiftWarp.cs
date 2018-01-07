using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEditor;
using UnityEngine;

public static class GiftWarp
{
    public static List<FVector2> GetConvexHull(List<FVector2> vertices)
    {
        // Find the right most point on the hull
        int i0 = 0;
        float x0 = vertices[0].X;
        for (int i = 1; i < vertices.Count; ++i)
        {
            float x = vertices[i].X;
            if (x > x0 || (x == x0 && vertices[i].Y < vertices[i0].Y))
            {
                i0 = i;
                x0 = x;
            }
        }

        int[] hull = new int[vertices.Count];
        int m = 0;
        int ih = i0;

        for (;;)
        {
            hull[m] = ih;

            int ie = 0;
            for (int j = 1; j < vertices.Count; ++j)
            {
                if (ie == ih)
                {
                    ie = j;
                    continue;
                }

                FVector2 r = vertices[ie] - vertices[hull[m]];
                FVector2 v = vertices[j] - vertices[hull[m]];
                float c = Cross(r, v);
                if (c < 0.0f)
                {
                    ie = j;
                }

                // Collinearity check
                if (c == 0.0f && v.LengthSquared() > r.LengthSquared())
                {
                    ie = j;
                }
            }

            ++m;
            ih = ie;

            if (ie == i0)
            {
                break;
            }
        }

        List<FVector2> result = new List<FVector2>();

        // Copy vertices.
        for (int i = 0; i < m; ++i)
        {
            result.Add(vertices[hull[i]]);
        }
        return result;
    }

    public static float Cross(FVector2 a, FVector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }
}
