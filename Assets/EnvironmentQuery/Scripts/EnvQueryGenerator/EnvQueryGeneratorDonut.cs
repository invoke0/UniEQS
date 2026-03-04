using System.Collections.Generic;
using UnityEngine;

/**
 * Donut Generator
 * Generates items in multiple rings around a center point.
 */
public class EnvQueryGeneratorDonut : EnvQueryGenerator
{
    private float _innerRadius;
    private float _outerRadius;
    private int _numberOfRings;
    private int _pointsPerRing;

    public EnvQueryGeneratorDonut(float innerRadius, float outerRadius, int numberOfRings, int pointsPerRing)
    {
        _innerRadius = innerRadius;
        _outerRadius = outerRadius;
        _numberOfRings = Mathf.Max(1, numberOfRings);
        _pointsPerRing = Mathf.Max(1, pointsPerRing);
    }

    public List<EnvQueryItem> GenerateItems(int numTests, Transform centerOfItems)
    {
        List<EnvQueryItem> items = new List<EnvQueryItem>();
        if (centerOfItems == null) return items;

        float radiusDelta = (_numberOfRings > 1) ? (_outerRadius - _innerRadius) / (_numberOfRings - 1) : 0;
        float angleDelta = (2 * Mathf.PI) / _pointsPerRing;

        for (int ringIdx = 0; ringIdx < _numberOfRings; ringIdx++)
        {
            float currentRadius = _innerRadius + (ringIdx * radiusDelta);
            for (int pointIdx = 0; pointIdx < _pointsPerRing; pointIdx++)
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
