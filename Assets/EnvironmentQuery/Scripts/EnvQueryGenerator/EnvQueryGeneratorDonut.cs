using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneratorDonut", menuName = "Environment Query/Generators/Donut")]
public class EnvQueryGeneratorDonut : EnvQueryGenerator
{
    public EnvQueryContext SearchCenter;
    public float InnerRadius = 1.0f;
    public float OuterRadius = 5.0f;
    public int NumberOfRings = 3;
    public int PointsPerRing = 8;

    public override List<EnvQueryItem> GenerateItems(EnvQueryInstance queryInstance)
    {
        List<EnvQueryItem> items = new List<EnvQueryItem>();
        
        if (!queryInstance.PrepareContext(SearchCenter, out List<Vector3> centerPoints))
        {
            return items;
        }

        int numTests = queryInstance.GetNumTests();

        float radiusDelta = (NumberOfRings > 1) ? (OuterRadius - InnerRadius) / (NumberOfRings - 1) : 0;
        float angleDelta = (2 * Mathf.PI) / PointsPerRing;

        foreach (Vector3 centerPos in centerPoints)
        {
            for (int ringIdx = 0; ringIdx < NumberOfRings; ringIdx++)
            {
                float currentRadius = InnerRadius + (ringIdx * radiusDelta);
                for (int pointIdx = 0; pointIdx < PointsPerRing; pointIdx++)
                {
                    float angle = pointIdx * angleDelta;
                    Vector3 offset = new Vector3(
                        currentRadius * Mathf.Cos(angle),
                        0,
                        currentRadius * Mathf.Sin(angle)
                    );
                    items.Add(new EnvQueryItem(numTests, centerPos + offset));
                }
            }
        }

        return items;
    }
}
