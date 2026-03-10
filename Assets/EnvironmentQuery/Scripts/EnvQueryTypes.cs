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

public enum EnvQueryTrace
{
    None,
    Navigation,
    Geometry
}

[System.Serializable]
public struct EnvTraceData
{
    public EnvQueryTrace TraceMode;
    public float ProjectDown;
    public float ProjectUp;
    public LayerMask GeometryLayer;

    public EnvTraceData(EnvQueryTrace mode = EnvQueryTrace.None)
    {
        TraceMode = mode;
        ProjectDown = 10.0f;
        ProjectUp = 10.0f;
        GeometryLayer = Physics.DefaultRaycastLayers;
    }
}

public delegate void QueryFinishedSignature(EnvQueryInstance queryInstance);

public static class EnvQueryTypes
{
    public const int INDEX_NONE = -1;
    public const float SkippedItemValue = -float.MaxValue;
    public const float UnlimitedStepTime = -1.0f;
}
