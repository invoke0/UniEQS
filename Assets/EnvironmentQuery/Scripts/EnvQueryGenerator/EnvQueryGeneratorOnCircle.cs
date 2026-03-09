using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class EnvQueryGeneratorOnCircle : EnvQueryGenerator
{
    public EnvQueryContext SearchCenter;
    public float Radius = 4.0f;
    public float SpaceBetween = 1.0f;

    public override void GenerateItems(EnvQueryInstance queryInstance)
    {
        if (!queryInstance.PrepareContext(SearchCenter, out List<Vector3> centerPoints)) return;

        foreach (Vector3 centerPos in centerPoints)
        {
            // Center point itself
            queryInstance.AddItem(centerPos);

            int numOfStepsForRadialDirection = (int)Mathf.Ceil(Radius / SpaceBetween);
            for(int ri = 1; ri <= numOfStepsForRadialDirection; ri++)
            {
                for(int k = 0; k < ri*8; k++)
                {
                    float theta = 1.0f/ri * k * Mathf.PI/4.0f;
                    Vector3 offset = new Vector3(
                        ri * SpaceBetween * Mathf.Sin(theta),
                        0.0f,
                        ri * SpaceBetween * Mathf.Cos(theta)
                    );
                    queryInstance.AddItem(centerPos + offset);
                }
            }
        }
    }
}
