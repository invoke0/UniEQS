using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class EnvQueryInstance : System.IDisposable
{
    public string QueryName;
    public int QueryID;
    public EnvQueryRunMode RunMode;
    public GameObject Owner;

    public List<EnvQueryOption> options;
    public int currentOptionIndex = 0;
    public int currentTestIndex = -1;

    public EnvQueryInstance.Status CurrentStatus = Status.Processing;

    public enum Status
    {
        Processing,
        Success,
        Failed,
        Aborted,
        OwnerLost,
        MissingTemplate
    }

    // Native Arrays for Core Data
    public NativeList<EnvQueryItem> Items;
    public NativeList<float> AllTestResults; // Flattened array of all test results

    // Parallel arrays for reference types (since NativeList cannot store GameObjects)
    // Only populated if Generator produces Actors or Context items
    public List<GameObject> ItemActors; 

    public EnvQueryItem BestResultItem;
    public float TotalExecutionTime;

    public Dictionary<string, float> NamedParams = new Dictionary<string, float>();
    public QueryFinishedSignature OnQueryFinished;
    private bool isFinished = false;

    // Context Caching
    private Dictionary<EnvQueryContext, List<Vector3>> contextLocationCache = new Dictionary<EnvQueryContext, List<Vector3>>();
    private Dictionary<EnvQueryContext, List<GameObject>> contextActorCache = new Dictionary<EnvQueryContext, List<GameObject>>();

    public EnvQueryInstance(string name, int id, EnvQueryRunMode mode, List<EnvQueryOption> queryOptions, GameObject owner)
    {
        QueryName = name;
        QueryID = id;
        RunMode = mode;
        options = queryOptions;
        Owner = owner;

        Items = new NativeList<EnvQueryItem>(100, Allocator.Persistent);
        AllTestResults = new NativeList<float>(100, Allocator.Persistent);
        ItemActors = new List<GameObject>();
    }

    public void Dispose()
    {
        if (Items.IsCreated) Items.Dispose();
        if (AllTestResults.IsCreated) AllTestResults.Dispose();
        
        contextLocationCache.Clear();
        contextActorCache.Clear();
        ItemActors.Clear();
    }

    public bool IsFinished()
    {
        return isFinished;
    }

    public int GetNumTests()
    {
        if (options == null || options.Count == 0 || currentOptionIndex >= options.Count) return 0;
        return options[currentOptionIndex].Tests.Count;
    }

    public void ExecuteOneStep(float timeLimit)
    {
        if (CurrentStatus != Status.Processing) return;

        float startTime = Time.realtimeSinceStartup;

        // 1. Generate Items (if not already done for current option)
        if (Items.Length == 0 && currentOptionIndex < options.Count)
        {
            var currentOption = options[currentOptionIndex];
            if (currentOption.Generator != null)
            {
                currentOption.Generator.GenerateItems(this);
            }
            
            // Allocate space for test results
            int numTests = GetNumTests();
            if (numTests > 0 && Items.Length > 0)
            {
                AllTestResults.ResizeUninitialized(Items.Length * numTests);
                // Initialize with skipped value
                for (int i = 0; i < AllTestResults.Length; i++)
                {
                    AllTestResults[i] = EnvQueryTypes.SkippedItemValue;
                }
            }
            
            if (Items.Length == 0)
            {
                // No items generated, move to next option or fail
                currentOptionIndex++;
                if (currentOptionIndex >= options.Count)
                {
                    CurrentStatus = Status.Failed;
                    isFinished = true;
                    OnQueryFinished?.Invoke(this);
                    return;
                }
            }
        }

        // 2. Run Tests
        while (currentOptionIndex < options.Count)
        {
            var currentOption = options[currentOptionIndex];
            int numTests = currentOption.Tests.Count;

            // If we have finished all tests for this option
            if (currentTestIndex >= numTests - 1)
            {
                FinalizeQuery();
                return;
            }

            // Move to next test
            currentTestIndex++;
            var currentTest = currentOption.Tests[currentTestIndex];
            
            if (currentTest != null)
            {
                currentTest.RunTest(this);
            }

            // Check time budget
            if (timeLimit > 0 && (Time.realtimeSinceStartup - startTime) > timeLimit)
            {
                TotalExecutionTime += (Time.realtimeSinceStartup - startTime);
                return; // Yield execution
            }
        }
    }

    private void FinalizeQuery()
    {
        // Compute final scores and normalization
        // This is a simplified version, usually involves normalization steps
        // For now, let's just pick the best one based on raw scores if already computed by tests

        // Normalize and Score
        // TODO: Implement full normalization logic here if needed
        
        // Find Best Item
        float bestScore = -float.MaxValue;
        int bestIndex = -1;

        for (int i = 0; i < Items.Length; i++)
        {
            var item = Items[i];
            if (item.IsValid)
            {
                if (item.Score > bestScore)
                {
                    bestScore = item.Score;
                    bestIndex = i;
                }
            }
        }

        if (bestIndex != -1)
        {
            BestResultItem = Items[bestIndex];
            CurrentStatus = Status.Success;
        }
        else
        {
            CurrentStatus = Status.Failed;
        }

        isFinished = true;
        OnQueryFinished?.Invoke(this);
        Dispose(); // Clean up native memory immediately upon completion? Or let Manager do it?
        // Better to let Manager do it or caller, but here we assume single-fire usage
        // Actually, if we dispose here, the results are gone. 
        // We should NOT dispose here if the user wants to read results.
        // The Manager handles disposal after notifying listeners or if it's fire-and-forget.
        // Reverting Dispose() here. The owner of the instance is responsible for disposal.
    }

    public bool PrepareContext(EnvQueryContext context, out List<Vector3> locations)
    {
        if (context == null)
        {
            locations = new List<Vector3>();
            if(Owner != null) locations.Add(Owner.transform.position);
            return true;
        }

        if (contextLocationCache.TryGetValue(context, out locations))
        {
            return locations != null && locations.Count > 0;
        }

        context.ProvideContext(this, out locations);
        contextLocationCache[context] = locations;
        return locations != null && locations.Count > 0;
    }

    public bool PrepareContext(EnvQueryContext context, out List<GameObject> actors)
    {
         if (context == null)
        {
            actors = new List<GameObject>();
            if(Owner != null) actors.Add(Owner);
            return true;
        }

        if (contextActorCache.TryGetValue(context, out actors))
        {
            return actors != null && actors.Count > 0;
        }

        context.ProvideContext(this, out actors);
        contextActorCache[context] = actors;
        return actors != null && actors.Count > 0;
    }
    
    // Helper to set test result
    public void SetTestResult(int itemIndex, int testIndex, float score)
    {
        int numTests = GetNumTests();
        if (numTests == 0) return;
        
        int flatIndex = itemIndex * numTests + testIndex;
        if (flatIndex >= 0 && flatIndex < AllTestResults.Length)
        {
            AllTestResults[flatIndex] = score;
        }
    }
    
    public float GetTestResult(int itemIndex, int testIndex)
    {
        int numTests = GetNumTests();
        if (numTests == 0) return EnvQueryTypes.SkippedItemValue;
        
        int flatIndex = itemIndex * numTests + testIndex;
        if (flatIndex >= 0 && flatIndex < AllTestResults.Length)
        {
            return AllTestResults[flatIndex];
        }
        return EnvQueryTypes.SkippedItemValue;
    }

    // New helper to add items to NativeList
    public void AddItem(Vector3 position, GameObject actor = null)
    {
        int actorID = actor != null ? actor.GetInstanceID() : 0;
        Items.Add(new EnvQueryItem(position, actorID));
        if (actor != null)
        {
            // Ensure ItemActors list is in sync with Items list if we are using actors
            // If this is the first actor, fill previous slots with null
            while (ItemActors.Count < Items.Length - 1) ItemActors.Add(null);
            ItemActors.Add(actor);
        }
    }

    public GameObject GetActorFor(EnvQueryItem item)
    {
        // Simple search in parallel list
        // Or we could store index in item... but item struct is generic.
        // We can just iterate the parallel list and match ID?
        // Or assume indices match if we populated it correctly.
        
        // Correct way: Assume parallel list index matches Item index.
        // But we don't know the index of 'item' unless we pass it.
        // We can search by ID if we stored it.
        
        if (item.ActorInstanceID == 0) return null;
        
        // Linear search in cached actors
        foreach (var actor in ItemActors)
        {
            if (actor != null && actor.GetInstanceID() == item.ActorInstanceID)
                return actor;
        }
        return null;
    }
}
