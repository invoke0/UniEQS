using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneratorOnCircle", menuName = "Environment Query/Generators/On Circle")]
public class EnvQueryGeneratorOnCircle : EnvQueryGenerator
{
	public float Radius = 4.0f;
	public float SpaceBetween = 1.0f;

    public override List<EnvQueryItem> GenerateItems(int numTests, Transform centerOfItems)
    {
        List<EnvQueryItem> items = new List<EnvQueryItem>();
		Vector3 position = Vector3.zero;
		items.Add(new EnvQueryItem(numTests, position, centerOfItems));

		int numOfStepsForRadialDirection = (int)Mathf.Ceil(Radius / SpaceBetween);
		for(int ri = 1; ri <= numOfStepsForRadialDirection; ri++)
		{
			for(int k = 0; k < ri*8; k++)
			{
				float theta = 1.0f/ri * k * Mathf.PI/4.0f;
				position.x = ri * SpaceBetween * Mathf.Sin(theta);
				position.y = 0.0f;
				position.z = ri * SpaceBetween * Mathf.Cos(theta);
				items.Add(new EnvQueryItem(numTests, position, centerOfItems));
			}
		}

        return items;
    }
}
