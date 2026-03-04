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

    private List<EnvQueryTest> tests;
    private EnvQueryGenerator generator;
    private Transform centerOfItems;
    private int currentTestIndex = -1; // -1 means generation step
    private bool isFinished = false;

    public EnvQueryInstance(string name, int id, EnvQueryRunMode mode, EnvQueryGenerator gen, List<EnvQueryTest> queryTests, GameObject owner)
    {
        QueryName = name;
        QueryID = id;
        RunMode = mode;
        generator = gen;
        tests = queryTests;
        Owner = owner;
        centerOfItems = owner.transform;
    }

    public bool IsFinished() => isFinished;

    public void ExecuteOneStep(float timeLimit)
    {
        if (isFinished) return;

        float startTime = Time.realtimeSinceStartup;

        if (currentTestIndex == -1)
        {
            // Generation step
            if (generator != null && centerOfItems != null)
            {
                Items = generator.GenerateItems(tests.Count, centerOfItems);
                foreach (var item in Items)
                {
                    item.UpdateNavMeshProjection();
                }
            }
            currentTestIndex = 0;
        }
        else if (currentTestIndex < tests.Count)
        {
            // Test step
            EnvQueryTest test = tests[currentTestIndex];
            if (test != null && test.IsActive)
            {
                test.RunTest(this, currentTestIndex);
                test.NormalizeItemScores(currentTestIndex, Items);
            }
            currentTestIndex++;
        }

        if (currentTestIndex >= tests.Count)
        {
            FinalizeQuery();
        }

        TotalExecutionTime += (Time.realtimeSinceStartup - startTime);
    }

    public void ExecuteFull()
    {
        while (!isFinished)
        {
            ExecuteOneStep(float.MaxValue);
        }
    }

    private void FinalizeQuery()
    {
        isFinished = true;
        
        var validItems = Items.Where(x => x.IsValid).ToList();
        if (validItems.Count == 0)
        {
            CurrentStatus = Status.Failed;
            BestResult = null;
            return;
        }

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

        // Notify delegate if set
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
