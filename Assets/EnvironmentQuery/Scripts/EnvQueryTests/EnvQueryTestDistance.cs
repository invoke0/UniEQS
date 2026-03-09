using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class EnvQueryTestDistance : EnvQueryTest
{
    public EnvQueryContext DistanceTo;

    public override void RunTest(EnvQueryInstance queryInstance)
    {
        if (DistanceTo == null || !queryInstance.Items.IsCreated) return;

        if (!queryInstance.PrepareContext(DistanceTo, out List<Vector3> contextLocations))
        {
            return;
        }

        if (contextLocations.Count == 0) return;

        // Simplified: using first context location. 
        // Ideally we should iterate all context locations and find Min/Max distance depending on settings.
        Vector3 contextPos = contextLocations[0];

        for (int i = 0; i < queryInstance.Items.Length; i++)
        {
            var item = queryInstance.Items[i];
            if (!item.IsValid) continue;

            float distance = Vector3.Distance(contextPos, item.GetWorldPosition());
            
            // Set raw score
            queryInstance.SetTestResult(i, queryInstance.currentTestIndex, distance);
        }

        // Apply normalization, filtering and scoring
        NormalizeItemScores(queryInstance);
    }
}
