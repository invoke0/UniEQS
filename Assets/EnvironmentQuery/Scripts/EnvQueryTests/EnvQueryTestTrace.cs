using System.Collections.Generic;
using UnityEngine;

public class EnvQueryTestTrace : EnvQueryTest
{
    private enum TraceType
    {
        Visible,
        Invisible
    }

    [SerializeField]
    private TraceType traceType = TraceType.Visible;

    public EnvQueryContext TraceFrom;

    public float ItemHeightOffset;
    public float TargetHeightOffset;

    public override void RunTest(EnvQueryInstance queryInstance, int currentTest)
    {
        if (!IsActive || TraceFrom == null || queryInstance.Items == null) return;

        List<Vector3> contextLocations;
        TraceFrom.ProvideContext(queryInstance, out contextLocations);
        if (contextLocations.Count == 0) return;

        Vector3 fromPos = contextLocations[0] + Vector3.up * TargetHeightOffset;

        foreach (EnvQueryItem item in queryInstance.Items)
        {
            if (!item.IsValid) continue;

            Vector3 itemPosition = item.GetWorldPosition() + Vector3.up * ItemHeightOffset;
            Vector3 direction = fromPos - itemPosition;
            float distance = direction.magnitude;

            bool hit = Physics.Raycast(itemPosition, direction.normalized, out RaycastHit raycastHit, distance);

            float result = 0.0f;
            if (!hit) // Visible
            {
                result = (traceType == TraceType.Visible) ? 1.0f : 0.0f;
            }
            else
            {
                result = (traceType == TraceType.Invisible) ? 1.0f : 0.0f;
            }

            item.TestResults[currentTest] = result;
            FilterItem(item, result);
        }
    }
}
