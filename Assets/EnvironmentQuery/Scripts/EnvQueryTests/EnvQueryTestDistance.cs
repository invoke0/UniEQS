using System.Collections.Generic;
using UnityEngine;

public class EnvQueryTestDistance : EnvQueryTest
{
    public EnvQueryContext DistanceTo;

    public override void RunTest(EnvQueryInstance queryInstance, int currentTest)
    {
        if (!IsActive || DistanceTo == null || queryInstance.Items == null) return;

        if (!queryInstance.PrepareContext(DistanceTo, out List<Vector3> contextLocations))
        {
            return;
        }

        if (contextLocations.Count == 0) return;

        Vector3 contextPos = contextLocations[0];

        foreach (EnvQueryItem item in queryInstance.Items)
        {
            if (!item.IsValid) continue;

            float distance = Vector3.Distance(contextPos, item.GetWorldPosition());
            item.TestResults[currentTest] = distance;
            FilterItem(item, distance);
        }
    }
}
