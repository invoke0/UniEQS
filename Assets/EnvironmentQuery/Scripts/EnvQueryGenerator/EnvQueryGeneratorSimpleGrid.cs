using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneratorSimpleGrid", menuName = "Environment Query/Generators/Simple Grid")]
public class EnvQueryGeneratorSimpleGrid : EnvQueryGenerator
{
	public float Radius = 4.0f;
	public float SpaceBetween = 1.0f;

    public override List<EnvQueryItem> GenerateItems(int numTests, Transform centerOfItems)
    {
        List<EnvQueryItem> items = new List<EnvQueryItem>();
		Vector3 position = Vector3.zero;
		items.Add(new EnvQueryItem(numTests, position, centerOfItems));

		int numOfSteps = (int)Mathf.Ceil(Radius / SpaceBetween);

		// First quadrant
		for(int xi = 0; xi < numOfSteps; xi++)
		{
			for(int zi = 0; zi < numOfSteps; zi++)
			{
				position.x = xi * SpaceBetween + SpaceBetween/2.0f;
				position.y = 0.0f;
				position.z = zi * SpaceBetween + SpaceBetween/2.0f;
				items.Add(new EnvQueryItem(numTests, position, centerOfItems));
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
				items.Add(new EnvQueryItem(numTests, position, centerOfItems));
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
				items.Add(new EnvQueryItem(numTests, position, centerOfItems));
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
				items.Add(new EnvQueryItem(numTests, position, centerOfItems));
			}
		}

        return items;
    }
}
