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

    private List<EnvQuery> _runningQueries = new List<EnvQuery>();

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
            EnvQuery query = _runningQueries[index];

            if (query == null || !query.isActiveAndEnabled)
            {
                _runningQueries.RemoveAt(index);
                continue;
            }

            // In UE5, ExecuteOneStep handles the progression. 
            // Here, we can call ExecuteQuery and track how long it took.
            // If we want real time-slicing, we'd need to modify EnvQuery to support partial execution.
            query.ExecuteQuery();

            float stepDuration = Time.realtimeSinceStartup - stepStartTime;
            timeLeft -= stepDuration;

            // In this simplified version, we just remove it once it's done. 
            // In UE5, queries might take multiple frames.
            _runningQueries.RemoveAt(index);
        }
    }

    public void RunQuery(EnvQuery query)
    {
        if (!_runningQueries.Contains(query))
        {
            _runningQueries.Add(query);
        }
    }

    public void AbortQuery(EnvQuery query)
    {
        _runningQueries.Remove(query);
    }
}
