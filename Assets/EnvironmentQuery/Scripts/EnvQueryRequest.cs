using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Environment Query Request
 * Prepares a query execution. Similar to FEnvQueryRequest in UE5.
 */
public class EnvQueryRequest
{
    public EnvQuery QueryTemplate;
    public GameObject Owner;
    public Dictionary<string, float> NamedParams = new Dictionary<string, float>();

    public EnvQueryRequest(EnvQuery template, GameObject owner)
    {
        QueryTemplate = template;
        Owner = owner;
    }

    public EnvQueryRequest SetNamedParam(string name, float value)
    {
        NamedParams[name] = value;
        return this;
    }

    public int Execute(EnvQueryRunMode runMode, QueryFinishedSignature callback)
    {
        if (QueryTemplate == null || Owner == null)
        {
            Debug.LogError("EnvQueryRequest: Missing template or owner!");
            return EnvQueryTypes.INDEX_NONE;
        }

        // 1. Create Query Instance via Manager
        EnvQueryInstance instance = EnvQueryManager.Instance.CreateQueryInstance(QueryTemplate, runMode, Owner);

        // 2. Set Named Params
        foreach (var param in NamedParams)
        {
            instance.NamedParams[param.Key] = param.Value;
        }

        // 3. Register and Run in Manager
        return EnvQueryManager.Instance.RunQuery(instance, callback);
    }
}
