using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EnvQueryContext : ScriptableObject
{
    public abstract void ProvideContext(EnvQueryInstance queryInstance, out List<Vector3> locations);
    public abstract void ProvideContext(EnvQueryInstance queryInstance, out List<GameObject> actors);

    public virtual bool ProvidesLocation() => true;
    public virtual bool ProvidesActor() => false;
}

public class EnvQueryContext_Querier : EnvQueryContext
{
    public override void ProvideContext(EnvQueryInstance queryInstance, out List<Vector3> locations)
    {
        locations = new List<Vector3> { queryInstance.Owner.transform.position };
    }

    public override void ProvideContext(EnvQueryInstance queryInstance, out List<GameObject> actors)
    {
        actors = new List<GameObject> { queryInstance.Owner };
    }

    public override bool ProvidesActor() => true;
}

public class EnvQueryContext_Item : EnvQueryContext
{
    public override void ProvideContext(EnvQueryInstance queryInstance, out List<Vector3> locations)
    {
        // This is usually handled inside the test/generator loop
        locations = new List<Vector3>();
    }

    public override void ProvideContext(EnvQueryInstance queryInstance, out List<GameObject> actors)
    {
        actors = new List<GameObject>();
    }
}
