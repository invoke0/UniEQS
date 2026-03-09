using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class EnvQueryTestDot : EnvQueryTest
{
    public EnvQueryContext LineFromContext;
    public EnvQueryContext LineToContext;
    public EnvQueryContext DirectionVectorContext;

    public bool IsAbsolute = false;

    public override void RunTest(EnvQueryInstance queryInstance)
    {
        if (!queryInstance.PrepareContext(LineFromContext, out List<Vector3> fromPoints)) return;
        if (!queryInstance.PrepareContext(LineToContext, out List<Vector3> toPoints)) return;
        if (!queryInstance.PrepareContext(DirectionVectorContext, out List<Vector3> dirVectors)) return;

        if (fromPoints.Count == 0 || toPoints.Count == 0 || dirVectors.Count == 0) return;

        Vector3 fromPos = fromPoints[0];
        Vector3 toPos = toPoints[0];
        Vector3 dirVec = dirVectors[0].normalized;

        for (int i = 0; i < queryInstance.Items.Length; i++)
        {
            var item = queryInstance.Items[i];
            Vector3 itemPos = item.GetWorldPosition();

            // Logic: Dot product of (Item -> LineTo) against (DirectionVector)
            // Assuming Item is the varying factor, usually 'from' or 'to' involves Item
            // If LineFrom is context (e.g. Querier), LineTo is context (e.g. Enemy), this is static for all items!
            // Wait, usually one end of the line is the Item.
            // If the user wants (Item -> Target) dot (Forward), LineFrom should be ItemContext (special context).
            
            // For simplicity and common usage: 
            // - If LineFromContext is null/default, use Item? No, Context must be provided.
            // - We need a special 'EnvQueryContext_Item' to represent the item itself.
            // - BUT, in PrepareContext, we can't easily get 'Item' context because it varies per loop.
            
            // Standard UE Dot Test:
            // Line A: rotation of context
            // Line B: vector from context to item
            
            // Let's implement: Dot(Direction of Context, (Item - Context).normalized)
            // This checks if Item is 'in front' of Context.
            
            // But here we have 3 contexts.
            // Let's stick to the previous implementation for now:
            // (To - Item).normalized DOT Dir
            
            Vector3 itemToTarget = (toPos - itemPos).normalized;
            float dot = Vector3.Dot(itemToTarget, dirVec);
            
            if (IsAbsolute) dot = Mathf.Abs(dot);

            queryInstance.SetTestResult(i, queryInstance.currentTestIndex, dot);
        }
        
        // Normalize and score all items based on the raw values we just set
        NormalizeItemScores(queryInstance);
    }
}
