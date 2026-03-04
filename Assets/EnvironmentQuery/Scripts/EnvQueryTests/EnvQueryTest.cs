using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EnvQueryTest : ScriptableObject
{
    public bool IsActive = true;
    public EnvQueryTestPurpose TestPurpose = EnvQueryTestPurpose.FilterAndScore;
    public EnvQueryTestFilterType FilterType = EnvQueryTestFilterType.Range;
    public float FloatValueMin = 0.0f;
    public float FloatValueMax = 100.0f;
    public bool BoolValue = true;
    public EnvQueryTestScoringEquation ScoringEquation = EnvQueryTestScoringEquation.Linear;
    public float ScoringFactor = 1.0f;

    public virtual void RunTest(EnvQueryInstance queryInstance, int currentTest) { }

    public void NormalizeItemScores(int currentTest, List<EnvQueryItem> envQueryItems)
    {
        if (envQueryItems == null || envQueryItems.Count < 1) return;
        if (TestPurpose == EnvQueryTestPurpose.Filter) return;

        float maxScore = -float.MaxValue;
        float minScore = float.MaxValue;

        // Collect min/max for normalization
        foreach (var item in envQueryItems)
        {
            if (!item.IsValid) continue;
            float val = item.TestResults[currentTest];
            if (val == EnvQueryTypes.SkippedItemValue) continue;

            if (val > maxScore) maxScore = val;
            if (val < minScore) minScore = val;
        }

        if (maxScore == minScore)
        {
            float scoreToApply = (minScore == 0.0f) ? 0.0f : ScoringFactor;
            foreach (var item in envQueryItems)
            {
                if (item.IsValid) item.Score += scoreToApply;
            }
            return;
        }

        float range = maxScore - minScore;
        float absWeight = Mathf.Abs(ScoringFactor);

        foreach (var item in envQueryItems)
        {
            if (!item.IsValid) continue;
            float val = item.TestResults[currentTest];
            if (val == EnvQueryTypes.SkippedItemValue)
            {
                // In UE5, skipped items get 0 weighted score
                continue;
            }

            float normalizedScore = (ScoringFactor >= 0)
                ? (val - minScore) / range
                : 1.0f - (val - minScore) / range;

            float weightedScore = 0.0f;
            switch (ScoringEquation)
            {
                case EnvQueryTestScoringEquation.Linear:
                    weightedScore = absWeight * normalizedScore;
                    break;
                case EnvQueryTestScoringEquation.InverseLinear:
                    weightedScore = absWeight * (1.0f - normalizedScore);
                    break;
                case EnvQueryTestScoringEquation.Square:
                    weightedScore = absWeight * (normalizedScore * normalizedScore);
                    break;
                case EnvQueryTestScoringEquation.SquareRoot:
                    weightedScore = absWeight * Mathf.Sqrt(normalizedScore);
                    break;
                case EnvQueryTestScoringEquation.Constant:
                    weightedScore = (normalizedScore > 0) ? absWeight : 0.0f;
                    break;
                // Keeping original half-sine etc. for compatibility if needed
                case EnvQueryTestScoringEquation.HalfSine:
                    weightedScore = absWeight * Mathf.Sin(Mathf.PI * normalizedScore);
                    break;
                case EnvQueryTestScoringEquation.SigmoidLike:
                    weightedScore = absWeight * (((float)Math.Tanh(4.0f * (normalizedScore - 0.5f)) + 1.0f) / 2.0f);
                    break;
            }

            item.Score += weightedScore;
        }
    }

    protected void FilterItem(EnvQueryItem item, float value)
    {
        if (TestPurpose == EnvQueryTestPurpose.Score) return;

        bool passed = false;
        switch (FilterType)
        {
            case EnvQueryTestFilterType.Minimum:
                passed = value >= FloatValueMin;
                break;
            case EnvQueryTestFilterType.Maximum:
                passed = value <= FloatValueMax;
                break;
            case EnvQueryTestFilterType.Range:
                passed = value >= FloatValueMin && value <= FloatValueMax;
                break;
            case EnvQueryTestFilterType.Match:
                passed = (value != 0) == BoolValue;
                break;
        }

        if (!passed)
        {
            item.Discard();
        }
    }
}
