using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneratorDonut", menuName = "Environment Query/Generators/Donut")]
public class EnvQueryGeneratorDonut : EnvQueryGenerator
{
    public float InnerRadius = 1.0f;
    public float OuterRadius = 5.0f;
    public int NumberOfRings = 3;
    public int PointsPerRing = 8;

    public override List<EnvQueryItem> GenerateItems(int numTests, Transform centerOfItems)
    {
        List<EnvQueryItem> items = new List<EnvQueryItem>();
        if (centerOfItems == null) return items;

        float radiusDelta = (NumberOfRings > 1) ? (OuterRadius - InnerRadius) / (NumberOfRings - 1) : 0;
        float angleDelta = (2 * Mathf.PI) / PointsPerRing;

        for (int ringIdx = 0; ringIdx < NumberOfRings; ringIdx++)
        {
            float currentRadius = InnerRadius + (ringIdx * radiusDelta);
            for (int pointIdx = 0; pointIdx < PointsPerRing; pointIdx++)
            {
                float angle = pointIdx * angleDelta;
                Vector3 position = new Vector3(
                    currentRadius * Mathf.Cos(angle),
                    0,
                    currentRadius * Mathf.Sin(angle)
                );
                items.Add(new EnvQueryItem(numTests, position, centerOfItems));
            }
        }

        return items;
    }
}
