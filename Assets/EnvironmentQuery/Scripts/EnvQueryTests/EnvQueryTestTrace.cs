using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class EnvQueryTestTrace : EnvQueryTest
{
    public EnvQueryContext TraceFrom;
    public EnvQueryContext TraceTo;
    public LayerMask TraceMask;
    public bool Inverse = false; // Inverse result (hit is bad vs good)

    public override void RunTest(EnvQueryInstance queryInstance)
    {
        if (!queryInstance.PrepareContext(TraceFrom, out List<Vector3> fromPoints)) return;
        // TraceTo usually is the item itself, but could be another context
        // Assuming TraceTo is not set => use Item location
        // If TraceTo is set => use Context location

        Vector3 fromPos = fromPoints.Count > 0 ? fromPoints[0] : Vector3.zero;

        for (int i = 0; i < queryInstance.Items.Length; i++)
        {
            var item = queryInstance.Items[i];
            if (!item.IsValid) continue;

            Vector3 itemPos = item.GetWorldPosition();
            
            // Vector from 'From' to 'Item'
            Vector3 direction = (itemPos - fromPos).normalized;
            float distance = Vector3.Distance(fromPos, itemPos);
            
            bool hit = Physics.Raycast(fromPos, direction, out RaycastHit hitInfo, distance, TraceMask);
            
            // If inverse: hit means BLOCKED (bad visibility), no hit means CLEAR (good visibility)
            // If not inverse: hit means YES (can see something?), usually trace is for VISIBILITY
            // Standard visibility test: No Hit = Visible (Good)
            
            bool isVisible = !hit;
            if (Inverse) isVisible = !isVisible;

            // Score: 1 if visible, 0 if not (Boolean test)
            float score = isVisible ? 1.0f : 0.0f;
            
            queryInstance.SetTestResult(i, queryInstance.currentTestIndex, score);
        }

        NormalizeItemScores(queryInstance);
    }
}
