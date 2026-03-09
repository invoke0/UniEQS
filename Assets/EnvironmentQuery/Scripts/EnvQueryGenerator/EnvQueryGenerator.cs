using System.Collections.Generic;
using UnityEngine;

public abstract class EnvQueryGenerator : ScriptableObject
{
    public abstract void GenerateItems(EnvQueryInstance queryInstance);
}
