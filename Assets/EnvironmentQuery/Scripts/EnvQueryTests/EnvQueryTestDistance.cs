using System.Collections.Generic;
using UnityEngine;

public class EnvQueryTestDistance : EnvQueryTest
{
    public EnvQueryContext DistanceTo;

    public override void RunTest(EnvQueryInstance queryInstance, int currentTest)
    {
        if (!IsActive || DistanceTo == null || queryInstance.Items == null) return;

        List<Vector3> contextLocations;
        DistanceTo.ProvideContext(queryInstance, out contextLocations);

        if (contextLocations.Count == 0) return;

        foreach (EnvQueryItem item in queryInstance.Items)
        {
            if (!item.IsValid) continue;

            float distance = Vector3.Distance(contextLocations[0], item.GetWorldPosition());
            item.TestResults[currentTest] = distance;
            FilterItem(item, distance);
        }
    }
}
