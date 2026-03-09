using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvQueryContext_Querier", menuName = "Environment Query/Contexts/Querier")]
public class EnvQueryContext_Querier : EnvQueryContext
{
#if UNITY_EDITOR
    private static EnvQueryContext_Querier _default;
    public static EnvQueryContext_Querier Default
    {
        get
        {
            if (_default == null)
            {
                string dir = "Assets/EnvironmentQuery/Contexts";
                if (!UnityEditor.AssetDatabase.IsValidFolder(dir)) 
                    UnityEditor.AssetDatabase.CreateFolder("Assets/EnvironmentQuery", "Contexts");
                string path = dir + "/EnvQueryContext_Querier_Default.asset";
                _default = UnityEditor.AssetDatabase.LoadAssetAtPath<EnvQueryContext_Querier>(path);
                if (_default == null)
                {
                    _default = ScriptableObject.CreateInstance<EnvQueryContext_Querier>();
                    UnityEditor.AssetDatabase.CreateAsset(_default, path);
                    UnityEditor.AssetDatabase.SaveAssets();
                }
            }
            return _default;
        }
    }
#endif

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
