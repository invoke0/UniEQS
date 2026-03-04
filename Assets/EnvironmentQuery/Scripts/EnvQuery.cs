using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * Environment Query Component
 * Defines a query and can trigger its execution.
 */
public class EnvQuery : MonoBehaviour
{
	public enum EnvQueryGeneratorType
	{
		OnCircle,
		SimpleGrid,
		Donut,
		ActorsOfClass
	}

	public EnvQueryTemplate QueryTemplate; // Template asset
	public EnvQueryRunMode RunMode = EnvQueryRunMode.SingleResult;

	public EnvQueryItem BestResult => _instance?.BestResult;
	public List<EnvQueryItem> AllResults => _instance?.Items;

	private EnvQueryInstance _instance;
	private int _requestID = EnvQueryTypes.INDEX_NONE;

	void Start()
	{
		// Manual execution example
		// ExecuteQuery();
	}

	void Update()
	{
		// For backward compatibility or testing, can be called per frame
		// though real use should be event-driven or from AI behavior nodes.
		ExecuteQuery();
	}

	public void ExecuteQuery()
	{
		if (QueryTemplate == null) return;

		// 1. Create a request (similar to UE5 code)
		EnvQueryRequest request = new EnvQueryRequest(QueryTemplate, gameObject);

		// 2. Execute and store ID (like MyMemory->RequestID = EQSRequest.Execute...)
		_requestID = request.Execute(RunMode, OnQueryFinished);
	}

	private void OnQueryFinished(EnvQueryInstance instance)
	{
		_instance = instance;
		// Handle result (e.g., move AI or update blackboard)
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if (isActiveAndEnabled && _instance != null && _instance.Items != null)
		{
			foreach (EnvQueryItem item in _instance.Items)
			{
				if (item.IsValid)
				{
					Gizmos.color = Color.HSVToRGB((Mathf.Clamp01(item.Score) / 2.0f), 1.0f, 1.0f);
					Gizmos.DrawWireSphere(item.GetWorldPosition(), 0.25f);
				}
			}

			if (_instance.BestResult != null)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(_instance.BestResult.GetWorldPosition(), 0.25f);
			}
		}
	}
#endif
}
