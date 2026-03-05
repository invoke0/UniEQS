using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Environment Query Manager
 * Handles multiple query instances, allowing for time-sliced execution (asynchronous queries).
 */
public class EnvQueryManager : MonoBehaviour
{
    private static EnvQueryManager _instance;
    public static EnvQueryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<EnvQueryManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("EnvQueryManager");
                    _instance = go.AddComponent<EnvQueryManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    [Header("Configuration")]
    [Tooltip("Maximum allowed time per frame for executing queries (in seconds).")]
    public float MaxAllowedTestingTime = 0.01f;

    private List<EnvQueryInstance> _runningQueries = new List<EnvQueryInstance>();
    private int _nextQueryID = 0;

    void Update()
    {
        Tick(Time.deltaTime);
    }

    public void Tick(float deltaTime)
    {
        if (_runningQueries.Count == 0) return;

        float timeLeft = MaxAllowedTestingTime;
        int index = 0;

        while (timeLeft > 0.0f && index < _runningQueries.Count)
        {
            float stepStartTime = Time.realtimeSinceStartup;
            EnvQueryInstance instance = _runningQueries[index];

            if (instance == null || instance.Owner == null)
            {
                _runningQueries.RemoveAt(index);
                continue;
            }

            instance.ExecuteOneStep(timeLeft);

            if (instance.IsFinished())
            {
                _runningQueries.RemoveAt(index);
            }
            else
            {
                index++;
            }

            float stepDuration = Time.realtimeSinceStartup - stepStartTime;
            timeLeft -= stepDuration;
        }
    }

    public int RunQuery(EnvQueryInstance instance, QueryFinishedSignature callback)
    {
        instance.OnQueryFinished = callback;
        _runningQueries.Add(instance);
        return instance.QueryID;
    }

    public EnvQueryInstance CreateQueryInstance(EnvQueryTemplate template, EnvQueryRunMode runMode, GameObject owner)
    {
        return new EnvQueryInstance(
            template.QueryName,
            GetNextQueryID(),
            runMode,
            new List<EnvQueryOption>(template.Options),
            owner
        );
    }

    public int GetNextQueryID() => _nextQueryID++;

    public void AbortQuery(int requestID)
    {
        _runningQueries.RemoveAll(q => q.QueryID == requestID);
    }
}
