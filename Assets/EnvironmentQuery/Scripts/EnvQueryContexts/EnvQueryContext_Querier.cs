using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvQueryContext_Querier", menuName = "Environment Query/Contexts/Querier")]
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
