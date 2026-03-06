using System.Collections.Generic;
using UnityEngine;

/**
 * Environment Query Generator (Definition)
 * Base class for all generators, similar to UEnvQueryGenerator in UE5.
 */
public abstract class EnvQueryGenerator : ScriptableObject
{
    public abstract List<EnvQueryItem> GenerateItems(EnvQueryInstance queryInstance);
}
