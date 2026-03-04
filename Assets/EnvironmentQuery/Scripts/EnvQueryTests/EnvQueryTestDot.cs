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
    public Transform Target;

    public DirModeForLineB DirectionB; 
    public GameObject DirectionVectorObj;
    public Transform LineFrom;
    public Transform LineTo;

    public override void RunTest(EnvQueryInstance queryInstance, int currentTest)
    {
        if (!IsActive || Target == null || queryInstance.Items == null) return;

        foreach (EnvQueryItem item in queryInstance.Items)
        {
            if (!item.IsValid) continue;

            Vector3 a = Vector3.zero;
            Vector3 b = Vector3.zero;

            if (DirectionA == DirModeForLineA.FromTargetToEachItem)
            {
                a = (item.GetWorldPosition() - Target.position).normalized;
            }
            else if (DirectionA == DirModeForLineA.FromEachItemToTarget)
            {
                a = (Target.position - item.GetWorldPosition()).normalized;
            }

            if (DirectionB == DirModeForLineB.TwoPoints && LineFrom != null && LineTo != null)
            {
                b = (LineTo.position - LineFrom.position).normalized;
            }
            else if (DirectionB == DirModeForLineB.DirectionVector_Forward && DirectionVectorObj != null)
            {
                b = DirectionVectorObj.transform.forward;
            }
            else if (DirectionB == DirModeForLineB.DirectionVector_Backward && DirectionVectorObj != null)
            {
                b = -(DirectionVectorObj.transform.forward);
            }
            else if (DirectionB == DirModeForLineB.DirectionVector_Right && DirectionVectorObj != null)
            {
                b = DirectionVectorObj.transform.right;
            }
            else if (DirectionB == DirModeForLineB.DirectionVector_Left && DirectionVectorObj != null)
            {
                b = -(DirectionVectorObj.transform.right);
            }

            float dotValue = Vector3.Dot(a, b);
            if (AbsoluteValue)
            {
                dotValue = Mathf.Abs(dotValue);
            }

            item.TestResults[currentTest] = dotValue;
            FilterItem(item, dotValue);
        }
    }
}
