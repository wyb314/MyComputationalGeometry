using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using Vectrosity;

public class GiftWrapExample : MonoBehaviour
{

    public List<FVector2> inputPoints;
    
    private int[] convexHull;
    public int curExtremaPointIdx;
    public int curRefreToPointIdx;
    public int curTestPointIdx;
    public int curM;
    public int curH;
    public int pointDensity = 100;
    public int raidus = 10;
    void OnGUI()
    {
        if (GUILayout.Button("Start"))
        {
            this.convexHull = null;
            this.curExtremaPointIdx = 0;
            this.curRefreToPointIdx = 0;
            this.curTestPointIdx = 0;
            this.curM = 0;
            this.curH = 0;
            List<FVector2> list = new List<FVector2>(100);
            for (int i = 0; i < pointDensity; i++)
            {

                FVector2 point = new FVector2(
                    UnityEngine.Random.Range(-raidus, raidus),
                    UnityEngine.Random.Range(-raidus, raidus));

                list.Add(point);

            }
            this.inputPoints = list;
            this.StartCoroutine(this.Excute());

            
        }

    }


    public float depth = 5;
    public void Update()
    {
        if (this.curM != 0)
        {
            List<Vector3> list = new List<Vector3>();
            for (int i = 0; i < this.curM; i++)
            {
                FVector2 v = this.inputPoints[this.convexHull[i]];
                list.Add(new Vector3(v.X,v.Y, depth));
            }

            FVector2 _v = this.inputPoints[this.curH];
            list.Add(new Vector3(_v.X, _v.Y, depth));

            for (int i = 0; i < list.Count; i++)
            {
                Vector3 a = list[i];
                Vector3 b = list[(i + 1 == list.Count) ? 0 : (i + 1)];
                

                Debug.DrawLine(a, b, Color.red);
            }
        }
    }

    public float dotSize = 2.0f;
    public int numberOfDots = 100;
    private void DrawPoints()
    {
        //var dots = new VectorLine("Dots", new List<Vector2>(dotPoints), dotSize, LineType.Points);
        ////dots.SetColors(new List<Color32>(dotColors));
        //for (int i = 0; i < numberOfRings; i++)
        //{
        //    dots.MakeCircle(new Vector2(Screen.width / 2, Screen.height / 2), Screen.height / (i + 2), numberOfDots, numberOfDots * i);
        //}
        //dots.Draw();
    }

    private int CalculateFirstExtremaPoint()
    {
        // Find the right most point on the hull
        int i0 = 0;
        float x0 = inputPoints[0].X;
        for (int i = 1; i < inputPoints.Count; ++i)
        {
            float x = inputPoints[i].X;
            if (x > x0 || (x == x0 && inputPoints[i].Y < inputPoints[i0].Y))
            {
                i0 = i;
                x0 = x;
            }
        }

        return i0;
    }

    public float delayInvoke = 0.05f;
    IEnumerator Excute()
    {
        int i0 = this.CalculateFirstExtremaPoint();
        this.curH = i0;
        this.convexHull = new int[this.inputPoints.Count];
        while (true)
        {
            this.convexHull[this.curM] = this.curH;
            this.curRefreToPointIdx = 0;
            for (int i = 1; i < inputPoints.Count; i++)
            {
                this.curRefreToPointIdx = i;
                this.ExcuteTestExtremaPoint();
                yield return new WaitForSeconds(delayInvoke);
            }

            ++this.curM;
            this.curH = this.curRefreToPointIdx;

            if (this.curRefreToPointIdx == i0)
            {
                break;
            }
        }
    }

    private void ExcuteTestExtremaPoint()
    {
        FVector2 r = inputPoints[curRefreToPointIdx] - inputPoints[convexHull[curExtremaPointIdx]];
        FVector2 v = inputPoints[curTestPointIdx] - inputPoints[convexHull[curExtremaPointIdx]];
        float c = Cross(r, v);
        if (c < 0.0f)
        {
            this.curRefreToPointIdx = curTestPointIdx;
        }

        // Collinearity check
        if (c == 0.0f && v.LengthSquared() > r.LengthSquared())
        {
            this.curRefreToPointIdx = curTestPointIdx;
        }
    }


    public void GetConvexHull(List<FVector2> vertices)
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
        //return result;
    }

    public static float Cross(FVector2 a, FVector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }



}
