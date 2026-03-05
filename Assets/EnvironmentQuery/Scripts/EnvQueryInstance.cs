using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * Environment Query Instance
 * Represents a single execution of an environment query.
 * Supports step-by-step execution for time-slicing.
 */
public class EnvQueryInstance
{
    public enum Status
    {
        Processing,
        Success,
        Failed,
        Aborted
    }

    public string QueryName;
    public int QueryID;
    public Status CurrentStatus = Status.Processing;
    public EnvQueryRunMode RunMode;
    public QueryFinishedSignature OnQueryFinished;
    public Dictionary<string, float> NamedParams = new Dictionary<string, float>();
    public List<EnvQueryItem> Items = new List<EnvQueryItem>();
    public EnvQueryItem BestResult { get; private set; }
    public float TotalExecutionTime { get; private set; }
    public GameObject Owner { get; private set; }

    private List<EnvQueryOption> options;
    private int currentOptionIndex = 0;
    private int currentTestIndex = -1; // -1 means generation step
    private bool isFinished = false;

    public EnvQueryInstance(string name, int id, EnvQueryRunMode mode, List<EnvQueryOption> queryOptions, GameObject owner)
    {
        QueryName = name;
        QueryID = id;
        RunMode = mode;
        options = queryOptions;
        Owner = owner;
    }

    public bool IsFinished() => isFinished;

    public void ExecuteOneStep(float timeLimit)
    {
        if (isFinished) return;

        float startTime = Time.realtimeSinceStartup;

        if (options == null || options.Count == 0)
        {
            CurrentStatus = Status.Failed;
            isFinished = true;
            return;
        }

        EnvQueryOption currentOption = options[currentOptionIndex];

        if (currentTestIndex == -1)
        {
            // Generation step
            if (currentOption.Generator != null && Owner != null)
            {
                Items = currentOption.Generator.GenerateItems(currentOption.Tests.Count, Owner.transform);
                foreach (var item in Items)
                {
                    item.UpdateNavMeshProjection();
                }
            }
            currentTestIndex = 0;
        }
        else if (currentTestIndex < currentOption.Tests.Count)
        {
            // Test step
            EnvQueryTest test = currentOption.Tests[currentTestIndex];
            if (test != null && test.IsActive)
            {
                test.RunTest(this, currentTestIndex);
                test.NormalizeItemScores(currentTestIndex, Items);
            }
            currentTestIndex++;
        }

        // Check if current option is finished
        if (currentTestIndex >= currentOption.Tests.Count)
        {
            FinalizeOption();
        }

        TotalExecutionTime += (Time.realtimeSinceStartup - startTime);
    }

    private void FinalizeOption()
    {
        var validItems = Items.Where(x => x.IsValid).ToList();
        
        if (validItems.Count > 0)
        {
            // Found results in current option, finalize query
            FinalizeQuery(validItems);
        }
        else
        {
            // No results in current option, try next option
            currentOptionIndex++;
            currentTestIndex = -1;
            
            if (currentOptionIndex >= options.Count)
            {
                // All options failed
                CurrentStatus = Status.Failed;
                BestResult = null;
                isFinished = true;
                OnQueryFinished?.Invoke(this);
            }
        }
    }

    public void ExecuteFull()
    {
        while (!isFinished)
        {
            ExecuteOneStep(float.MaxValue);
        }
    }

    private void FinalizeQuery(List<EnvQueryItem> validItems)
    {
        isFinished = true;
        
        // Sort by score
        validItems.Sort((a, b) => b.Score.CompareTo(a.Score));

        switch (RunMode)
        {
            case EnvQueryRunMode.SingleResult:
                BestResult = validItems[0];
                break;
            case EnvQueryRunMode.RandomBest5Pct:
                BestResult = PickRandomItemOfScoreAtLeast(validItems, validItems[0].Score * 0.95f);
                break;
            case EnvQueryRunMode.RandomBest25Pct:
                BestResult = PickRandomItemOfScoreAtLeast(validItems, validItems[0].Score * 0.75f);
                break;
            case EnvQueryRunMode.AllMatching:
                BestResult = validItems[0];
                break;
        }

        CurrentStatus = Status.Success;
        OnQueryFinished?.Invoke(this);
    }

    private EnvQueryItem PickRandomItemOfScoreAtLeast(List<EnvQueryItem> sortedValidItems, float minScore)
    {
        int count = 0;
        while (count < sortedValidItems.Count && sortedValidItems[count].Score >= minScore)
        {
            count++;
        }
        return sortedValidItems[Random.Range(0, count)];
    }
}
