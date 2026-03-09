using System.Collections.Generic;
using UnityEngine;

public class EnvQuery : MonoBehaviour
{
    public EnvQueryTemplate QueryTemplate; // Template asset
    public EnvQueryRunMode RunMode = EnvQueryRunMode.SingleResult;

    // Use struct, but this is a COPY
    public EnvQueryItem BestResult => _instance.BestResultItem;
    // For NativeList access, user should use _instance.Items
    // Or we copy it to List for convenience?
    // Copying defeats the purpose of NativeList. 
    // Let's expose the raw NativeList? Or provide List<EnvQueryItem> copy on demand?
    
    // For general usage, we probably just want the best position.
    public Vector3 BestResultLocation => _instance.BestResultItem.IsValid ? _instance.BestResultItem.GetWorldPosition() : Vector3.zero;

    public EnvQueryInstance _instance;
    private int _requestID = EnvQueryTypes.INDEX_NONE;

    // Start is called before the first frame update
    void Start()
    {
        // Example execution
        // ExecuteQuery();
    }

    // Update is called once per frame
    void Update()
    {
        ExecuteQuery();
    }

    public void ExecuteQuery()
    {
        if (QueryTemplate == null) return;
        EnvQueryRequest request = new EnvQueryRequest(QueryTemplate, gameObject);
        _requestID = request.Execute(RunMode, OnQueryFinished);
    }

    private void OnQueryFinished(EnvQueryInstance instance)
    {
        _instance = instance;
        // Handle result (e.g., move AI or update blackboard)
        
        if (instance.CurrentStatus == EnvQueryInstance.Status.Success)
        {
            Debug.Log($"Query Finished! Best Score: {instance.BestResultItem.Score} at {instance.BestResultItem.GetWorldPosition()}");
            
            // Example usage of Actor ref retrieval
            // GameObject bestActor = instance.GetActorFor(instance.BestResultItem);
        }
        else
        {
            Debug.LogWarning("Query Failed or No Result.");
        }
        
        // Note: instance will be Disposed immediately after this callback returns in EnvQueryManager.
        // If we need to keep data, we MUST copy it here.
        // For simple usage (fire and forget), this is fine.
    }

    private void OnDrawGizmos()
    {
        if (_instance != null && _instance.Items.IsCreated) // Check if valid
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < _instance.Items.Length; i++)
            {
                var item = _instance.Items[i];
                if (item.IsValid)
                {
                    Gizmos.DrawWireSphere(item.GetWorldPosition(), 0.5f);
                }
            }
            
            if (_instance.BestResultItem.IsValid)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_instance.BestResultItem.GetWorldPosition(), 0.6f);
            }
        }
    }
}
