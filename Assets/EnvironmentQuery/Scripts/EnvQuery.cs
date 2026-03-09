using System.Collections.Generic;
using UnityEngine;

/**
 * Environment Query Option
 * Bundles a generator and its associated tests. Similar to FEnvQueryOption in UE5.
 */
[System.Serializable]
public class EnvQueryOption
{
    public EnvQueryGenerator Generator;
    public List<EnvQueryTest> Tests = new List<EnvQueryTest>();
}

/**
 * Environment Query Template
 * A ScriptableObject that defines a query template, similar to UEnvQuery in UE5.
 */
[CreateAssetMenu(fileName = "NewEnvQuery", menuName = "Environment Query/EnvQuery")]
public class EnvQuery : ScriptableObject
{
    public string QueryName;
    public List<EnvQueryOption> Options = new List<EnvQueryOption>();
}
