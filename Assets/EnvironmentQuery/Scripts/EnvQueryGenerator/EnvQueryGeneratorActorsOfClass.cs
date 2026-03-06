using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneratorActorsOfClass", menuName = "Environment Query/Generators/Actors Of Class")]
public class EnvQueryGeneratorActorsOfClass : EnvQueryGenerator
{
    public EnvQueryContext SearchCenter;
    public string SearchedTag = "Enemy";
    public float SearchRadius = 50.0f;
    public bool UseRadius = true;

    public override List<EnvQueryItem> GenerateItems(EnvQueryInstance queryInstance)
    {
        List<EnvQueryItem> items = new List<EnvQueryItem>();
        if (string.IsNullOrEmpty(SearchedTag)) return items;

        GameObject[] allActors = GameObject.FindGameObjectsWithTag(SearchedTag);
        
        if (!queryInstance.PrepareContext(SearchCenter, out List<Vector3> centerPoints))
        {
            return items;
        }

        int numTests = queryInstance.GetNumTests();
        float radiusSq = SearchRadius * SearchRadius;

        foreach (Vector3 centerPos in centerPoints)
        {
            foreach (GameObject actor in allActors)
            {
                if (UseRadius)
                {
                    if (Vector3.SqrMagnitude(actor.transform.position - centerPos) > radiusSq)
                    {
                        continue;
                    }
                }

                // For actor items, we store the actor reference. 
                // Note: We might want to avoid adding the same actor multiple times if multiple centers are used.
                if (!items.Exists(it => it.Actor == actor))
                {
                    items.Add(new EnvQueryItem(numTests, actor.transform.position, actor));
                }
            }
        }

        return items;
    }
}
