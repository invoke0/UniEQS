using System.Collections.Generic;
using UnityEngine;

public class EnvQueryTestDot : EnvQueryTest
{
    public enum DirModeForLineA
    {
        FromTargetToEachItem,
        FromEachItemToTarget,
    }

    public enum DirModeForLineB
    {
        DirectionVector_Forward,
        DirectionVector_Backward,
        DirectionVector_Right,
        DirectionVector_Left,
        TwoPoints,
    }

    public bool AbsoluteValue;

    public DirModeForLineA DirectionA;
    public EnvQueryContext TargetContext; // Use context instead of Transform

    public DirModeForLineB DirectionB; 
    public EnvQueryContext DirectionVectorContext; // Use context
    public EnvQueryContext LineFromContext;
    public EnvQueryContext LineToContext;

    public override void RunTest(EnvQueryInstance queryInstance, int currentTest)
    {
        if (!IsActive || queryInstance.Items == null) return;

        // Prepare A
        if (!queryInstance.PrepareContext(TargetContext, out List<Vector3> targetLocations) || targetLocations.Count == 0) return;
        Vector3 targetPos = targetLocations[0];

        // Prepare B (Direction)
        Vector3 bDir = Vector3.forward;
        if (DirectionB == DirModeForLineB.TwoPoints)
        {
            if (queryInstance.PrepareContext(LineFromContext, out List<Vector3> fromLocs) && fromLocs.Count > 0 &&
                queryInstance.PrepareContext(LineToContext, out List<Vector3> toLocs) && toLocs.Count > 0)
            {
                bDir = (toLocs[0] - fromLocs[0]).normalized;
            }
        }
        else
        {
            if (queryInstance.PrepareContext(DirectionVectorContext, out List<GameObject> contextActors) && contextActors.Count > 0)
            {
                Transform contextTransform = contextActors[0].transform;
                switch (DirectionB)
                {
                    case DirModeForLineB.DirectionVector_Forward: bDir = contextTransform.forward; break;
                    case DirModeForLineB.DirectionVector_Backward: bDir = -contextTransform.forward; break;
                    case DirModeForLineB.DirectionVector_Right: bDir = contextTransform.right; break;
                    case DirModeForLineB.DirectionVector_Left: bDir = -contextTransform.right; break;
                }
            }
        }

        foreach (EnvQueryItem item in queryInstance.Items)
        {
            if (!item.IsValid) continue;

            Vector3 itemPos = item.GetWorldPosition();
            Vector3 aDir = Vector3.zero;

            if (DirectionA == DirModeForLineA.FromTargetToEachItem)
            {
                aDir = (itemPos - targetPos).normalized;
            }
            else if (DirectionA == DirModeForLineA.FromEachItemToTarget)
            {
                aDir = (targetPos - itemPos).normalized;
            }

            float dotValue = Vector3.Dot(aDir, bDir);
            if (AbsoluteValue)
            {
                dotValue = Mathf.Abs(dotValue);
            }

            item.TestResults[currentTest] = dotValue;
            FilterItem(item, dotValue);
        }
    }
}
