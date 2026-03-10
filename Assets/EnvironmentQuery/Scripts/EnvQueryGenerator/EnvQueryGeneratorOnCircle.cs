using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnvQueryGeneratorOnCircle : EnvQueryGenerator_ProjectedPoints
{
    public EnvQueryContext SearchCenter;
    public float Radius = 10.0f;
    public float SpaceBetween = 2.0f;
    public int NumberOfPoints = 8;
    
    public enum PointSpacingMethod
    {
        BySpaceBetween,
        ByNumberOfPoints
    }
    public PointSpacingMethod SpacingMethod = PointSpacingMethod.ByNumberOfPoints;

    [Header("Radial Trace")]
    public EnvTraceData TraceData = new EnvTraceData(EnvQueryTrace.Navigation);

    public override void GenerateItems(EnvQueryInstance queryInstance)
    {
        if (!queryInstance.PrepareContext(SearchCenter, out List<Vector3> centerPoints)) return;

        List<Vector3> rawPoints = new List<Vector3>();

        foreach (Vector3 centerPos in centerPoints)
        {
            // First calculate how many points we need
            int stepsCount;
            float angleStep;

            if (SpacingMethod == PointSpacingMethod.BySpaceBetween)
            {
                // Circumference = 2 * PI * R
                float circumference = 2.0f * Mathf.PI * Radius;
                stepsCount = Mathf.CeilToInt(circumference / SpaceBetween);
                if (stepsCount <= 0) stepsCount = 1;
                angleStep = 360.0f / stepsCount;
            }
            else
            {
                stepsCount = NumberOfPoints;
                if (stepsCount <= 0) stepsCount = 1;
                angleStep = 360.0f / stepsCount;
            }

            for (int step = 0; step < stepsCount; step++)
            {
                float angleRad = step * angleStep * Mathf.Deg2Rad;
                
                // Assuming Vector3.forward is the starting angle 0
                Vector3 dir = new Vector3(Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad));
                Vector3 targetPos = centerPos + dir * Radius;

                if (TraceData.TraceMode != EnvQueryTrace.None)
                {
                    float dist = Radius;
                    
                    if (TraceData.TraceMode == EnvQueryTrace.Geometry)
                    {
                        if (Physics.Raycast(centerPos, dir, out RaycastHit hit, dist, TraceData.GeometryLayer))
                        {
                            rawPoints.Add(hit.point);
                            continue;
                        }
                    }
                    else if (TraceData.TraceMode == EnvQueryTrace.Navigation)
                    {
                        if (NavMesh.Raycast(centerPos, targetPos, out NavMeshHit hit, NavMesh.AllAreas))
                        {
                            rawPoints.Add(hit.position);
                            continue;
                        }
                    }
                }

                rawPoints.Add(targetPos);
            }
        }

        ProjectAndFilterPoints(rawPoints, queryInstance);
    }
}
