using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class EnvQueryGeneratorActorsOfClass : EnvQueryGenerator
{
    public EnvQueryContext SearchCenter;
    public string TagToSearch = "Enemy";
    public float SearchRadius = 50.0f;

    public override void GenerateItems(EnvQueryInstance queryInstance)
    {
        if (!queryInstance.PrepareContext(SearchCenter, out List<Vector3> centerPoints)) return;

        // Find all GameObjects with tag
        GameObject[] actors = GameObject.FindGameObjectsWithTag(TagToSearch);
        float sqrRadius = SearchRadius * SearchRadius;

        foreach (Vector3 centerPos in centerPoints)
        {
            foreach (var actor in actors)
            {
                if (Vector3.SqrMagnitude(actor.transform.position - centerPos) <= sqrRadius)
                {
                    // Check if we already added this actor (by ID) to avoid duplicates if multiple centers overlap
                    // This check is a bit slow with large lists, but correct for multi-center overlap
                    int actorID = actor.GetInstanceID();
                    bool exists = false;
                    for(int i=0; i<queryInstance.Items.Length; i++)
                    {
                        if(queryInstance.Items[i].ActorInstanceID == actorID)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        queryInstance.AddItem(actor.transform.position, actor);
                    }
                }
            }
        }
    }
}
