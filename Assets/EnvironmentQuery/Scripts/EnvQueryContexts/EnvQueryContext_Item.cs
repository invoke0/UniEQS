using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvQueryContext_Item", menuName = "Environment Query/Contexts/Item")]
public class EnvQueryContext_Item : EnvQueryContext
{
#if UNITY_EDITOR
    private static EnvQueryContext_Item _default;
    public static EnvQueryContext_Item Default
    {
        get
        {
            if (_default == null)
            {
                string dir = "Assets/EnvironmentQuery/Contexts";
                if (!UnityEditor.AssetDatabase.IsValidFolder(dir)) 
                    UnityEditor.AssetDatabase.CreateFolder("Assets/EnvironmentQuery", "Contexts");
                string path = dir + "/EnvQueryContext_Item_Default.asset";
                _default = UnityEditor.AssetDatabase.LoadAssetAtPath<EnvQueryContext_Item>(path);
                if (_default == null)
                {
                    _default = ScriptableObject.CreateInstance<EnvQueryContext_Item>();
                    UnityEditor.AssetDatabase.CreateAsset(_default, path);
                    UnityEditor.AssetDatabase.SaveAssets();
                }
            }
            return _default;
        }
    }
#endif

    // Items Context is handled internally by Tests
}
