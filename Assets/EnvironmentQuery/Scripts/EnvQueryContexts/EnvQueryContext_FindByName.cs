using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvQueryContext_FindByName", menuName = "Environment Query/Contexts/Find By Name")]
public class EnvQueryContext_FindByName : EnvQueryContext
{
    public string GameObjectName;

    public override void ProvideContext(EnvQueryInstance queryInstance, out List<Vector3> locations)
    {
        locations = new List<Vector3>();
        if (!string.IsNullOrEmpty(GameObjectName))
        {
            GameObject go = GameObject.Find(GameObjectName);
            if (go != null)
            {
                locations.Add(go.transform.position);
            }
        }
    }

    public override void ProvideContext(EnvQueryInstance queryInstance, out List<GameObject> actors)
    {
        actors = new List<GameObject>();
        if (!string.IsNullOrEmpty(GameObjectName))
        {
            GameObject go = GameObject.Find(GameObjectName);
            if (go != null)
            {
                actors.Add(go);
            }
        }
    }
}
