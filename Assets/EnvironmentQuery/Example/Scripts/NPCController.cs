using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
	private NavMeshAgent agent;
	private EQSTestingPawn query;

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		query = GetComponent<EQSTestingPawn>();
	}

	void Update()
	{
		if (query != null && query.bestItem.IsValid)
		{
			agent.SetDestination(query.bestItem.GetWorldPosition());
		}
	}
}
