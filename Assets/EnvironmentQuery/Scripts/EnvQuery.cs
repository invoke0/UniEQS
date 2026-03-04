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

	public EnvQueryItem BestResult => _instance?.BestResult;
	public List<EnvQueryItem> AllResults => _instance?.Items;

	public EnvQueryRunMode RunMode = EnvQueryRunMode.SingleResult;
	public EnvQueryGeneratorType GeneratorType = EnvQueryGeneratorType.OnCircle;
	
	[Header("Generator Parameters")]
	public float Radius = 4.0f;
	public float SpaceBetween = 1.0f;
	public float InnerRadius = 1.0f;
	public float OuterRadius = 5.0f;
	public int NumberOfRings = 3;
	public int PointsPerRing = 8;
	public string SearchedTag = "Enemy";
	public bool UseRadiusForActors = true;
	public float SearchRadiusForActors = 50.0f;
	
	public List<EnvQueryTest> EnvQueryTests = new List<EnvQueryTest>();

	private EnvQueryInstance _instance;

	void Start()
	{
		// Optional: Auto-run if needed, or wait for manual trigger
	}

	void Update()
	{
		// For backward compatibility, we can auto-trigger every frame if needed
		// but it's better to use ExecuteQuery() manually.
		ExecuteQuery();
	}

	public void ExecuteQuery()
	{
		EnvQueryGenerator generator = null;
		switch (GeneratorType)
		{
			case EnvQueryGeneratorType.OnCircle:
				generator = new EnvQueryGeneratorOnCircle(Radius, SpaceBetween);
				break;
			case EnvQueryGeneratorType.SimpleGrid:
				generator = new EnvQueryGeneratorSimpleGrid(Radius, SpaceBetween);
				break;
			case EnvQueryGeneratorType.Donut:
				generator = new EnvQueryGeneratorDonut(InnerRadius, OuterRadius, NumberOfRings, PointsPerRing);
				break;
			case EnvQueryGeneratorType.ActorsOfClass:
				generator = new EnvQueryGeneratorActorsOfClass(SearchedTag, SearchRadiusForActors, UseRadiusForActors);
				break;
		}

		if (generator != null)
		{
			_instance = new EnvQueryInstance(gameObject.name, RunMode, generator, EnvQueryTests, gameObject);
			_instance.ExecuteFull();
		}
	}

	// Async version using EnvQueryManager
	public void ExecuteQueryAsync()
	{
		EnvQueryGenerator generator = null;
		if (GeneratorType == EnvQueryGeneratorType.OnCircle)
		{
			generator = new EnvQueryGeneratorOnCircle(Radius, SpaceBetween);
		}
		else if (GeneratorType == EnvQueryGeneratorType.SimpleGrid)
		{
			generator = new EnvQueryGeneratorSimpleGrid(Radius, SpaceBetween);
		}

		if (generator != null)
		{
			// This part would need EnvQueryManager to support adding EnvQueryInstance directly
			// For now, we'll keep it simple.
		}
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
