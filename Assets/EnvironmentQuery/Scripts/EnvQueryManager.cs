using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class EnvQueryManager : MonoBehaviour
{
    private static EnvQueryManager _instance;
    public static EnvQueryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnvQueryManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("EnvQueryManager");
                    _instance = go.AddComponent<EnvQueryManager>();
                }
            }
            return _instance;
        }
    }

    private List<EnvQueryInstance> _runningQueries = new List<EnvQueryInstance>();
    private int _nextQueryID = 0;

    public float TimeSliceLimit = 0.01f; // 10ms per frame

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else if (_instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private int GetNextQueryID()
    {
        return _nextQueryID++;
    }

    public EnvQueryInstance CreateQueryInstance(EnvQueryTemplate template, EnvQueryRunMode runMode, GameObject owner)
    {
        // Direct creation, handling NativeList lifecycle internally
        var instance = new EnvQueryInstance(
            template.QueryName,
            GetNextQueryID(),
            runMode,
            new List<EnvQueryOption>(template.Options),
            owner
        );
        return instance;
    }

    public int RunQuery(EnvQueryInstance instance, QueryFinishedSignature callback)
    {
        if (instance == null) return EnvQueryTypes.INDEX_NONE;

        instance.OnQueryFinished += callback;
        _runningQueries.Add(instance);

        return instance.QueryID;
    }

    public void AbortQuery(int requestID)
    {
        for (int i = _runningQueries.Count - 1; i >= 0; i--)
        {
            if (_runningQueries[i].QueryID == requestID)
            {
                var instance = _runningQueries[i];
                _runningQueries.RemoveAt(i);
                instance.Dispose(); // Clean up native memory
            }
        }
    }

    private void Update()
    {
        if (_runningQueries.Count == 0) return;

        float startTime = Time.realtimeSinceStartup;
        // Simple round-robin or priority queue could be used. 
        // For now, iterate all, but respect global time slice.
        
        // We process queries until time slice is used up
        int index = 0;
        while (index < _runningQueries.Count)
        {
            if ((Time.realtimeSinceStartup - startTime) > TimeSliceLimit)
            {
                break; // Stop processing for this frame
            }

            var instance = _runningQueries[index];
            
            // Execute a step
            // We give it the remaining time of the slice
            float remainingTime = TimeSliceLimit - (Time.realtimeSinceStartup - startTime);
            instance.ExecuteOneStep(remainingTime);

            if (instance.IsFinished())
            {
                _runningQueries.RemoveAt(index);
                // The callback is invoked inside FinalizeQuery/IsFinished logic usually, 
                // but here we ensure it's handled.
                // Instance invokes callback internally in FinalizeQuery.
                
                // IMPORTANT: Dispose after query is finished and processed by callback
                // The callback was already called inside instance.FinalizeQuery() or ExecuteOneStep
                // wait... FinalizeQuery calls OnQueryFinished.
                // If user accessed data in callback, it's fine.
                // Now we must Dispose.
                instance.Dispose();
            }
            else
            {
                index++;
            }
        }
    }
    
    private void OnDestroy()
    {
        // Clean up all pending queries
        foreach (var query in _runningQueries)
        {
            query.Dispose();
        }
        _runningQueries.Clear();
    }
}
