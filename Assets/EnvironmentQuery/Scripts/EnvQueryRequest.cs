using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Environment Query Request
 * Prepares a query execution. Similar to FEnvQueryRequest in UE5.
 */
public class EnvQueryRequest
{
    public EnvQueryTemplate QueryTemplate;
    public GameObject Owner;
    public Dictionary<string, float> NamedParams = new Dictionary<string, float>();

    public EnvQueryRequest(EnvQueryTemplate template, GameObject owner)
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

        // 1. Resolve Generator
        EnvQueryGenerator generator = null;
        switch (QueryTemplate.GeneratorType)
        {
            case EnvQuery.EnvQueryGeneratorType.OnCircle:
                generator = new EnvQueryGeneratorOnCircle(QueryTemplate.Radius, QueryTemplate.SpaceBetween);
                break;
            case EnvQuery.EnvQueryGeneratorType.SimpleGrid:
                generator = new EnvQueryGeneratorSimpleGrid(QueryTemplate.Radius, QueryTemplate.SpaceBetween);
                break;
            case EnvQuery.EnvQueryGeneratorType.Donut:
                generator = new EnvQueryGeneratorDonut(QueryTemplate.InnerRadius, QueryTemplate.OuterRadius, QueryTemplate.NumberOfRings, QueryTemplate.PointsPerRing);
                break;
            case EnvQuery.EnvQueryGeneratorType.ActorsOfClass:
                generator = new EnvQueryGeneratorActorsOfClass(QueryTemplate.SearchedTag, QueryTemplate.SearchRadiusForActors, QueryTemplate.UseRadiusForActors);
                break;
        }

        if (generator == null) return EnvQueryTypes.INDEX_NONE;

        // 2. Create Query Instance
        int queryID = EnvQueryManager.Instance.GetNextQueryID();
        EnvQueryInstance instance = new EnvQueryInstance(
            QueryTemplate.QueryName,
            queryID,
            runMode,
            generator,
            new List<EnvQueryTest>(QueryTemplate.EnvQueryTests), // Copy tests to instance
            Owner
        );

        // 3. Set Named Params
        foreach (var param in NamedParams)
        {
            instance.NamedParams[param.Key] = param.Value;
        }

        // 4. Register and Run in Manager
        return EnvQueryManager.Instance.RunQuery(instance, callback);
    }
}
