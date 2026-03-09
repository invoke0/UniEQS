using System;
using UnityEngine;

[Serializable]
public struct EnvQueryItem
{
    public float Score;
    public bool IsValid;

    private Vector3 worldPosition;
    private Vector3 navWorldPosition;
    
    // Using InstanceID (int) instead of reference
    public int ActorInstanceID;

    // We store the START INDEX into the flat TestResults array in EnvQueryInstance
    // For example, if there are 3 tests, item 0 starts at index 0, item 1 starts at index 3...
    // Actually we can just compute this index from ItemIndex * NumTests if we know NumTests
    // But keeping it flexible if items differ (which they shouldn't in one query)
    // Simpler: Just rely on flat indexing logic in Instance

    public EnvQueryItem(Vector3 position, int actorID = 0)
    {
        Score = 0.0f;
        IsValid = true;
        this.worldPosition = position;
        this.navWorldPosition = position;
        this.ActorInstanceID = actorID;
    }

    public Vector3 GetWorldPosition()
    {
        // If ActorID != 0, we can't efficiently resolve it here without context.
        // The context/manager should resolve it if needed, or update worldPosition before query.
        // For performance, we assume worldPosition is updated or valid.
        return navWorldPosition;
    }

    public void SetWorldPosition(Vector3 pos)
    {
        worldPosition = pos;
        navWorldPosition = pos;
    }
}
