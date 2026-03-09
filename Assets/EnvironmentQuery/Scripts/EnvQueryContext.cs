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
