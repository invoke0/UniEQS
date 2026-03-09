using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Collections;

public class EnvQueryTestPathFinding : EnvQueryTest
{
    public EnvQueryContext Target;
    public bool TestPathExist = true;
    public bool TestPathCost = false;
    public bool TestPathLength = false;
    public bool FilterIfNoPath = true;

    public override void RunTest(EnvQueryInstance queryInstance)
    {
        if (!queryInstance.PrepareContext(Target, out List<Vector3> targetPoints)) return;

        if (targetPoints.Count == 0) return;
        Vector3 targetPos = targetPoints[0]; // Simplified single target

        NavMeshPath path = new NavMeshPath();

        for (int i = 0; i < queryInstance.Items.Length; i++)
        {
            var item = queryInstance.Items[i];
            if (!item.IsValid) continue;

            bool hasPath = NavMesh.CalculatePath(item.GetWorldPosition(), targetPos, NavMesh.AllAreas, path);
            
            if (FilterIfNoPath && (!hasPath || path.status != NavMeshPathStatus.PathComplete))
            {
                item.IsValid = false;
                queryInstance.Items[i] = item;
                // Also set result to skipped so it doesn't affect normalization
                queryInstance.SetTestResult(i, queryInstance.currentTestIndex, EnvQueryTypes.SkippedItemValue);
                continue;
            }

            float score = 0f;
            if (TestPathLength)
            {
                float length = 0f;
                if (path.corners.Length > 1)
                {
                    for (int k = 0; k < path.corners.Length - 1; k++)
                    {
                        length += Vector3.Distance(path.corners[k], path.corners[k + 1]);
                    }
                }
                score = length;
            }
            // Cost is harder to get directly without extra setup, skipping for now or treating as length

            // Store raw score (distance)
            // Smaller distance is usually better, handled by negative ScoringFactor or InverseLinear equation
            queryInstance.SetTestResult(i, queryInstance.currentTestIndex, score);
        }

        NormalizeItemScores(queryInstance);
    }
}
