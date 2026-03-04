using UnityEngine;

public enum EnvQueryTestPurpose
{
    Filter,
    Score,
    FilterAndScore
}

public enum EnvQueryTestFilterType
{
    Minimum,
    Maximum,
    Range,
    Match
}

public enum EnvQueryTestScoringEquation
{
    Linear,
    InverseLinear,
    Square,
    SquareRoot,
    Constant,
    HalfSine,
    InverseHalfSine,
    HalfSineSquared,
    InverseHalfSineSquared,
    SigmoidLike,
    InverseSigmoidLike
}

public enum EnvQueryRunMode
{
    SingleResult,
    RandomBest5Pct,
    RandomBest25Pct,
    AllMatching
}

public enum EnvQueryResultNormalizationOption
{
    Default,
    Unaltered,
    Normalized
}

public enum EnvQueryStatus
{
    Processing,
    Success,
    Failed,
    Aborted,
    OwnerLost,
    MissingTemplate
}

public static class EnvQueryTypes
{
    public const float SkippedItemValue = -float.MaxValue;
    public const float UnlimitedStepTime = -1.0f;
}
