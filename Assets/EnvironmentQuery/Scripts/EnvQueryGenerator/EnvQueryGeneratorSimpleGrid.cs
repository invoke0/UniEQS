using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneratorSimpleGrid", menuName = "Environment Query/Generators/Simple Grid")]
public class EnvQueryGeneratorSimpleGrid : EnvQueryGenerator
{
	public EnvQueryContext SearchCenter;
	public float Radius = 4.0f;
	public float SpaceBetween = 1.0f;

    public override List<EnvQueryItem> GenerateItems(EnvQueryInstance queryInstance)
    {
        List<EnvQueryItem> items = new List<EnvQueryItem>();
        
        if (!queryInstance.PrepareContext(SearchCenter, out List<Vector3> centerPoints))
        {
            return items;
        }

        int numTests = queryInstance.GetNumTests();

        foreach (Vector3 centerPos in centerPoints)
        {
            Vector3 position = Vector3.zero;
            items.Add(new EnvQueryItem(numTests, centerPos));

            int numOfSteps = (int)Mathf.Ceil(Radius / SpaceBetween);

            // First quadrant
            for(int xi = 0; xi < numOfSteps; xi++)
            {
                for(int zi = 0; zi < numOfSteps; zi++)
                {
                    position.x = xi * spaceBetween + spaceBetween/2.0f;
                    position.y = 0.0f;
                    position.z = zi * spaceBetween + spaceBetween/2.0f;
                    items.Add(new EnvQueryItem(numTests, centerPos + position));
                }
            }
            // Second quadrant
            for(int xi = 0; xi < numOfSteps; xi++)
            {
                for(int zi = 0; zi < numOfSteps; zi++)
                {
                    position.x = -(xi * spaceBetween + spaceBetween/2.0f);
                    position.y =   0.0f;
                    position.z =   zi * spaceBetween + spaceBetween/2.0f;
                    items.Add(new EnvQueryItem(numTests, centerPos + position));
                }
            }
            // Third quadrant
            for(int xi = 0; xi < numOfSteps; xi++)
            {
                for(int zi = 0; zi < numOfSteps; zi++)
                {
                    position.x = -(xi * spaceBetween + spaceBetween/2.0f);
                    position.y =   0.0f;
                    position.z = -(zi * spaceBetween + spaceBetween/2.0f);
                    items.Add(new EnvQueryItem(numTests, centerPos + position));
                }
            }
            // Fourth quadrant
            for(int xi = 0; xi < numOfSteps; xi++)
            {
                for(int zi = 0; zi < numOfSteps; zi++)
                {
                    position.x =   xi * spaceBetween + spaceBetween/2.0f;
                    position.y =   0.0f;
                    position.z = -(zi * spaceBetween + spaceBetween/2.0f);
                    items.Add(new EnvQueryItem(numTests, centerPos + position));
                }
            }
        }

        return items;
    }
}
