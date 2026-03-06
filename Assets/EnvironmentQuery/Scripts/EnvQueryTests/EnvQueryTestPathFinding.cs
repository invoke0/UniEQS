using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnvQueryTestPathFinding : EnvQueryTest
{
    public enum PathFindingTestType
    {
        PathExist,
        PathLength
    }

    public PathFindingTestType PathFindingType;
    public EnvQueryContext Target;

    public override void RunTest(EnvQueryInstance queryInstance, int currentTest)
    {
        if (!IsActive || Target == null || queryInstance.Items == null) return;

        if (!queryInstance.PrepareContext(Target, out List<Vector3> contextLocations))
        {
            return;
        }
        
        if (contextLocations.Count == 0) return;

        Vector3 targetPos = contextLocations[0];
        NavMeshPath path = new NavMeshPath();

        foreach (EnvQueryItem item in queryInstance.Items)
        {
            if (!item.IsValid) continue;

            float result = 0.0f;
            NavMesh.CalculatePath(item.GetWorldPosition(), targetPos, NavMesh.AllAreas, path);

            if (PathFindingType == PathFindingTestType.PathExist)
            {
                result = (path.status == NavMeshPathStatus.PathComplete) ? 1.0f : 0.0f;
            }
            else if (PathFindingType == PathFindingTestType.PathLength)
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    result = CalculatePathLength(item.GetWorldPosition(), path);
                }
                else
                {
                    result = 10000.0f; // Large value for unreachable
                }
            }

            item.TestResults[currentTest] = result;
            FilterItem(item, result);
        }
    }

    private float CalculatePathLength(Vector3 startPosition, NavMeshPath path)
    {
        if (path.corners.Length < 1) return 0.0f;

        float lengthSoFar = Vector3.Distance(startPosition, path.corners[0]);
        Vector3 previousCorner = path.corners[0];
        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 currentCorner = path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
            previousCorner = currentCorner;
        }

        return lengthSoFar;
    }
}
