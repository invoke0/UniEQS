using System.Collections.Generic;
using UnityEngine;

/**
 * Actors of Class Generator
 * Generates items from actors of a specific class (Tag in Unity).
 */
public class EnvQueryGeneratorActorsOfClass : EnvQueryGenerator
{
    private string _searchedTag;
    private float _searchRadius;
    private bool _useRadius;

    public EnvQueryGeneratorActorsOfClass(string tag, float radius = 50.0f, bool useRadius = true)
    {
        _searchedTag = tag;
        _searchRadius = radius;
        _useRadius = useRadius;
    }

    public List<EnvQueryItem> GenerateItems(int numTests, Transform centerOfItems)
    {
        List<EnvQueryItem> items = new List<EnvQueryItem>();
        if (string.IsNullOrEmpty(_searchedTag)) return items;

        GameObject[] actors = GameObject.FindGameObjectsWithTag(_searchedTag);
        float radiusSq = _searchRadius * _searchRadius;

        foreach (GameObject actor in actors)
        {
            if (_useRadius && centerOfItems != null)
            {
                if (Vector3.SqrMagnitude(actor.transform.position - centerOfItems.position) > radiusSq)
                {
                    continue;
                }
            }

            // For actor items, we use their position as absolute and store the actor reference
            items.Add(new EnvQueryItem(numTests, Vector3.zero, centerOfItems, actor));
        }

        return items;
    }
}
