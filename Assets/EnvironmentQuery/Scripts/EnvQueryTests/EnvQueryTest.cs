using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public abstract class EnvQueryTest : ScriptableObject
{
    public EnvQueryTestPurpose TestPurpose = EnvQueryTestPurpose.FilterAndScore;
    public EnvQueryTestFilterType FilterType = EnvQueryTestFilterType.Range;

    // Filter params
    public float FloatValueMin = 0.0f;
    public float FloatValueMax = 100.0f;
    public bool BoolValue = true; // For 'Match' filter

    // Scoring params
    public EnvQueryTestScoringEquation ScoringEquation = EnvQueryTestScoringEquation.Linear;
    public float ScoringFactor = 1.0f; // Weight

    // Abstract method to be implemented by subclasses
    // They should populate the raw test results using queryInstance.SetTestResult
    public abstract void RunTest(EnvQueryInstance queryInstance);

    // Called after RunTest to normalize scores and filter items
    // This is generic logic that applies to all tests
    public void NormalizeItemScores(EnvQueryInstance queryInstance)
    {
        if (!queryInstance.Items.IsCreated || queryInstance.Items.Length < 1) return;

        int currentTestIndex = queryInstance.currentTestIndex;
        float minScore = float.MaxValue;
        float maxScore = -float.MaxValue;

        // 1. First Pass: Find Min/Max for normalization (and pre-filter check)
        // Also skip invalid items
        for (int i = 0; i < queryInstance.Items.Length; i++)
        {
            var item = queryInstance.Items[i];
            if (!item.IsValid) continue;

            float val = queryInstance.GetTestResult(i, currentTestIndex);
            if (val == EnvQueryTypes.SkippedItemValue) continue;

            // Apply filtering logic here?
            // If we filter here, we mark item as invalid and don't include in min/max
            if (TestPurpose != EnvQueryTestPurpose.Score)
            {
                if (!PassesFilter(val))
                {
                    item.IsValid = false;
                    queryInstance.Items[i] = item; // Write back struct
                    continue;
                }
            }

            if (TestPurpose == EnvQueryTestPurpose.Filter) continue; // If only filtering, we are done with this item

            if (val < minScore) minScore = val;
            if (val > maxScore) maxScore = val;
        }

        if (TestPurpose == EnvQueryTestPurpose.Filter) return; // Done if filter only

        // 2. Second Pass: Normalize and Apply Score
        float range = maxScore - minScore;
        // Avoid divide by zero
        if (range <= Mathf.Epsilon) range = 1.0f; 

        for (int i = 0; i < queryInstance.Items.Length; i++)
        {
            var item = queryInstance.Items[i];
            if (!item.IsValid) continue;

            float val = queryInstance.GetTestResult(i, currentTestIndex);
            if (val == EnvQueryTypes.SkippedItemValue) continue;

            float normalizedScore = 0f;
            if (Mathf.Abs(maxScore - minScore) < Mathf.Epsilon)
            {
                // All values are the same
                normalizedScore = 1.0f; 
            }
            else
            {
                // Normalize to 0..1
                // If ScoringFactor < 0 (inverse), we invert the normalization? 
                // Usually UE inverts the result, i.e. (Max - Val) / Range
                // But let's stick to standard normalization first: (Val - Min) / Range
                normalizedScore = (val - minScore) / range;
            }

            // Apply Scoring Equation
            // In UE, ReferenceValue is used for complex curves. Here we use simple equations.
            // But we should also handle the 'Inverse' nature of the test itself (handled in RunTest usually via 1-score)
            // But here we are normalizing raw values.

            // If ScoringFactor is negative, we want low raw values to have high score
            // So we invert the normalized score: 1 - norm
            // Wait, usually:
            // High Raw Value (e.g. Dist) -> Good? Or Bad?
            // If Test is "Distance", usually Closer is Better.
            // So we want Small Dist -> High Score.
            // This is handled by ScoringFactor sign OR by 'Inverse' property on specific test.
            // Let's assume standard behavior:
            // Positive Factor: High Raw -> High Score
            // Negative Factor: Low Raw -> High Score

            if (ScoringFactor < 0)
            {
                normalizedScore = 1.0f - normalizedScore;
            }

            float weightedScore = CalculateWeightedScore(normalizedScore, Mathf.Abs(ScoringFactor));
            
            // Accumulate score
            item.Score += weightedScore;
            queryInstance.Items[i] = item; // Write back
        }
    }

    private bool PassesFilter(float value)
    {
        switch (FilterType)
        {
            case EnvQueryTestFilterType.Minimum:
                return value >= FloatValueMin;
            case EnvQueryTestFilterType.Maximum:
                return value <= FloatValueMax;
            case EnvQueryTestFilterType.Range:
                return value >= FloatValueMin && value <= FloatValueMax;
            case EnvQueryTestFilterType.Match:
                // Match implies boolean mostly
                return (value != 0) == BoolValue;
            default:
                return true;
        }
    }

    private float CalculateWeightedScore(float normalizedScore, float weight)
    {
        // Clamp 0..1 just in case
        normalizedScore = Mathf.Clamp01(normalizedScore);

        switch (ScoringEquation)
        {
            case EnvQueryTestScoringEquation.Linear:
                return normalizedScore * weight;
            case EnvQueryTestScoringEquation.InverseLinear:
                return (1.0f - normalizedScore) * weight;
            case EnvQueryTestScoringEquation.Square:
                return normalizedScore * normalizedScore * weight;
            case EnvQueryTestScoringEquation.SquareRoot:
                return Mathf.Sqrt(normalizedScore) * weight;
            case EnvQueryTestScoringEquation.Constant:
                return weight;
            case EnvQueryTestScoringEquation.SigmoidLike:
                // Simple sigmoid approx
                float k = 4.0f;
                return (1.0f / (1.0f + Mathf.Exp(-k * (normalizedScore - 0.5f)))) * weight; 
            default:
                return normalizedScore * weight;
        }
    }
}
