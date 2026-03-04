using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnvQueryItem
{
    public float Score;
    public bool IsValid;
    public float[] TestResults;
    public GameObject Actor; // Store associated actor if this item is an actor

    private Transform centerOfItems; // Base position
    private Vector3 location; // Relative location
    private Vector3 navLocation; // Relative location (after NavMesh projection)

    public EnvQueryItem(int numTests, Vector3 location, Transform centerOfItems, GameObject actor = null)
    {
        Score = 0.0f;
        IsValid = true;
        TestResults = new float[numTests];
        for (int i = 0; i < numTests; i++) TestResults[i] = EnvQueryTypes.SkippedItemValue;
        this.centerOfItems = centerOfItems;
        this.location = location;
        this.navLocation = location;
        this.Actor = actor;
    }

    public Vector3 GetWorldPosition()
    {
        if (Actor != null) return Actor.transform.position;
        return centerOfItems.position + navLocation;
    }

    public void Discard()
    {
        IsValid = false;
        Score = -float.MaxValue;
    }

    public void UpdateNavMeshProjection()
    {
        IsValid = true;

        NavMeshHit result;
        Vector3 worldPosition = centerOfItems.position + location;
        if (NavMesh.SamplePosition(worldPosition, out result, 3.0f, NavMesh.AllAreas))
        {
            float diff = (result.position.x - worldPosition.x)*(result.position.x - worldPosition.x)
                       + (result.position.z - worldPosition.z)*(result.position.z - worldPosition.z);

            if(diff < 0.0001f) // Loosened the precision requirement slightly
            {
                IsValid = true;
                navLocation = result.position - centerOfItems.position;
            }
            else
            {
                IsValid = false;
                navLocation = location;
            }
        }
        else
        {
            IsValid = false;
            navLocation = location;
        }
    }
}
