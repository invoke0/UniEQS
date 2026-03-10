using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnvQueryGenerator_ProjectedPoints : EnvQueryGenerator
{
    [Header("Projection")]
    public EnvTraceData ProjectionData = new EnvTraceData(EnvQueryTrace.Navigation);

    /// <summary>
    /// Projects the raw points onto the NavMesh or Geometry and adds valid points to the QueryInstance.
    /// This mimics UE's ProjectAndFilterNavPoints + StoreNavPoints
    /// </summary>
    protected virtual void ProjectAndFilterPoints(List<Vector3> rawPoints, EnvQueryInstance queryInstance)
    {
        if (ProjectionData.TraceMode == EnvQueryTrace.None)
        {
            foreach (var pt in rawPoints)
            {
                queryInstance.AddItem(pt);
            }
            return;
        }

        if (ProjectionData.TraceMode == EnvQueryTrace.Navigation)
        {
            float maxDistance = Mathf.Max(ProjectionData.ProjectUp, ProjectionData.ProjectDown);
            // Unity's SamplePosition searches spherically. To simulate UE's vertical projection,
            // we apply a horizontal distance limit.
            float maxHorizontalSnap = 2.0f; 
            float maxHSqr = maxHorizontalSnap * maxHorizontalSnap;

            foreach (var pt in rawPoints)
            {
                if (NavMesh.SamplePosition(pt, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
                {
                    float yDiff = pt.y - hit.position.y;
                    Vector2 hDiff = new Vector2(pt.x - hit.position.x, pt.z - hit.position.z);
                    
                    // Check if the difference in height is within limits and horizontal drift is small
                    if (yDiff <= ProjectionData.ProjectDown && yDiff >= -ProjectionData.ProjectUp && hDiff.sqrMagnitude <= maxHSqr)
                    {
                        queryInstance.AddItem(hit.position);
                    }
                }
            }
        }
        else if (ProjectionData.TraceMode == EnvQueryTrace.Geometry)
        {
            foreach (var pt in rawPoints)
            {
                Vector3 origin = pt + Vector3.up * ProjectionData.ProjectUp;
                float dist = ProjectionData.ProjectUp + ProjectionData.ProjectDown;
                
                if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, dist, ProjectionData.GeometryLayer))
                {
                    queryInstance.AddItem(hit.point);
                }
            }
        }
    }
}
