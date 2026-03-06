using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EnvQueryContext : ScriptableObject
{
    public virtual void ProvideContext(EnvQueryInstance queryInstance, out List<Vector3> locations)
    {
        locations = null;
    }

    public virtual void ProvideContext(EnvQueryInstance queryInstance, out List<GameObject> actors)
    {
        actors = null;
    }
}

public class EnvQueryContext_Querier : EnvQueryContext
{
    public override void ProvideContext(EnvQueryInstance queryInstance, out List<Vector3> locations)
    {
        locations = new List<Vector3>();
        if (queryInstance.Owner != null)
        {
            locations.Add(queryInstance.Owner.transform.position);
        }
    }

    public override void ProvideContext(EnvQueryInstance queryInstance, out List<GameObject> actors)
    {
        actors = new List<GameObject>();
        if (queryInstance.Owner != null)
        {
            actors.Add(queryInstance.Owner);
        }
    }
}

public class EnvQueryContext_Item : EnvQueryContext
{
    // Items Context is handled internally by Tests
}
