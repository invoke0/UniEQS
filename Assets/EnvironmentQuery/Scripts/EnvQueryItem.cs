using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnvQueryItem
{
    public float Score;
    public bool IsValid;
    public float[] TestResults;
    public GameObject Actor; // Store associated actor if this item is an actor

    private Vector3 worldPosition; // Absolute world position
    private Vector3 navWorldPosition; // Absolute world position (after NavMesh projection)

    public EnvQueryItem(int numTests, Vector3 absoluteWorldPosition, GameObject actor = null)
    {
        Score = 0.0f;
        IsValid = true;
        TestResults = new float[numTests];
        for (int i = 0; i < numTests; i++) TestResults[i] = EnvQueryTypes.SkippedItemValue;
        
        this.worldPosition = absoluteWorldPosition;
        this.navWorldPosition = absoluteWorldPosition;
        this.Actor = actor;
    }

    public Vector3 GetWorldPosition()
    {
        if (Actor != null) return Actor.transform.position;
        return navWorldPosition;
    }

    public void Discard()
    {
        IsValid = false;
        Score = -float.MaxValue;
    }

    public void UpdateNavMeshProjection()
    {
        // If it's an actor, we don't project its position (usually)
        if (Actor != null)
        {
            IsValid = true;
            return;
        }

        NavMeshHit result;
        if (NavMesh.SamplePosition(worldPosition, out result, 3.0f, NavMesh.AllAreas))
        {
            float diff = (result.position.x - worldPosition.x)*(result.position.x - worldPosition.x)
                       + (result.position.z - worldPosition.z)*(result.position.z - worldPosition.z);

            if(diff < 0.0001f)
            {
                IsValid = true;
                navWorldPosition = result.position;
            }
            else
            {
                IsValid = false;
                navWorldPosition = worldPosition;
            }
        }
        else
        {
            IsValid = false;
            navWorldPosition = worldPosition;
        }
    }
}
