using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneratorActorsOfClass", menuName = "Environment Query/Generators/Actors Of Class")]
public class EnvQueryGeneratorActorsOfClass : EnvQueryGenerator
{
    public string SearchedTag = "Enemy";
    public float SearchRadius = 50.0f;
    public bool UseRadius = true;

    public override List<EnvQueryItem> GenerateItems(int numTests, Transform centerOfItems)
    {
        List<EnvQueryItem> items = new List<EnvQueryItem>();
        if (string.IsNullOrEmpty(SearchedTag)) return items;

        GameObject[] actors = GameObject.FindGameObjectsWithTag(SearchedTag);
        float radiusSq = SearchRadius * SearchRadius;

        foreach (GameObject actor in actors)
        {
            if (UseRadius && centerOfItems != null)
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
