using System.Collections.Generic;
using UnityEngine;

/**
 * Environment Query Template
 * A ScriptableObject that defines a query template, similar to UEnvQuery in UE5.
 */
[CreateAssetMenu(fileName = "NewEnvQuery", menuName = "Environment Query/Query Template")]
public class EnvQueryTemplate : ScriptableObject
{
    public string QueryName;
    public EnvQuery.EnvQueryGeneratorType GeneratorType = EnvQuery.EnvQueryGeneratorType.OnCircle;

    [Header("Generator Parameters")]
    public float Radius = 4.0f;
    public float SpaceBetween = 1.0f;
    public float InnerRadius = 1.0f;
    public float OuterRadius = 5.0f;
    public int NumberOfRings = 3;
    public int PointsPerRing = 8;
    public string SearchedTag = "Enemy";
    public bool UseRadiusForActors = true;
    public float SearchRadiusForActors = 50.0f;

    public List<EnvQueryTest> EnvQueryTests = new List<EnvQueryTest>();
}
