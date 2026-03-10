using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class EnvQueryGeneratorDonut : EnvQueryGenerator_ProjectedPoints
{
    public EnvQueryContext SearchCenter;
    public float InnerRadius = 2.0f;
    public float OuterRadius = 6.0f;
    public int NumberOfRings = 4;
    public int PointsPerRing = 8;

    public override void GenerateItems(EnvQueryInstance queryInstance)
    {
        if (!queryInstance.PrepareContext(SearchCenter, out List<Vector3> centerPoints)) return;
        List<Vector3> rawPoints = new List<Vector3>();
        
        float radiusStep = (OuterRadius - InnerRadius) / Mathf.Max(1, NumberOfRings - 1);
        float angleDelta = 2 * Mathf.PI / PointsPerRing;

        foreach (Vector3 centerPos in centerPoints)
        {
            for (int ringIdx = 0; ringIdx < NumberOfRings; ringIdx++)
            {
                float currentRadius = InnerRadius + (radiusStep * ringIdx);
                for (int pointIdx = 0; pointIdx < PointsPerRing; pointIdx++)
                {
                    float angle = pointIdx * angleDelta;
                    Vector3 offset = new Vector3(
                        currentRadius * Mathf.Cos(angle),
                        0,
                        currentRadius * Mathf.Sin(angle)
                    );
                    rawPoints.Add(centerPos + offset);
                }
            }
        }
        
        ProjectAndFilterPoints(rawPoints, queryInstance);
    }
}
