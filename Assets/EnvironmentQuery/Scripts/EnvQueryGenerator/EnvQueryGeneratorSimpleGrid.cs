using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class EnvQueryGeneratorSimpleGrid : EnvQueryGenerator
{
    public EnvQueryContext SearchCenter;
    public float Radius = 10.0f;
    public float SpaceBetween = 2.0f;

    public override void GenerateItems(EnvQueryInstance queryInstance)
    {
        if (!queryInstance.PrepareContext(SearchCenter, out List<Vector3> centerPoints)) return;

        foreach (Vector3 centerPos in centerPoints)
        {
            Vector3 position = Vector3.zero;
            queryInstance.AddItem(centerPos);

            int numOfSteps = (int)Mathf.Ceil(Radius / SpaceBetween);

            // First quadrant
            for(int xi = 0; xi < numOfSteps; xi++)
            {
                for(int zi = 0; zi < numOfSteps; zi++)
                {
                    position.x = xi * SpaceBetween + SpaceBetween/2.0f;
                    position.y = 0.0f;
                    position.z = zi * SpaceBetween + SpaceBetween/2.0f;
                    queryInstance.AddItem(centerPos + position);
                }
            }
            // Second quadrant
            for(int xi = 0; xi < numOfSteps; xi++)
            {
                for(int zi = 0; zi < numOfSteps; zi++)
                {
                    position.x = -(xi * SpaceBetween + SpaceBetween/2.0f);
                    position.y =   0.0f;
                    position.z =   zi * SpaceBetween + SpaceBetween/2.0f;
                    queryInstance.AddItem(centerPos + position);
                }
            }
            // Third quadrant
            for(int xi = 0; xi < numOfSteps; xi++)
            {
                for(int zi = 0; zi < numOfSteps; zi++)
                {
                    position.x = -(xi * SpaceBetween + SpaceBetween/2.0f);
                    position.y =   0.0f;
                    position.z = -(zi * SpaceBetween + SpaceBetween/2.0f);
                    queryInstance.AddItem(centerPos + position);
                }
            }
            // Fourth quadrant
            for(int xi = 0; xi < numOfSteps; xi++)
            {
                for(int zi = 0; zi < numOfSteps; zi++)
                {
                    position.x =   xi * SpaceBetween + SpaceBetween/2.0f;
                    position.y =   0.0f;
                    position.z = -(zi * SpaceBetween + SpaceBetween/2.0f);
                    queryInstance.AddItem(centerPos + position);
                }
            }
        }
    }
}
