using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class EQSTestingPawn : MonoBehaviour
{
    public EnvQuery QueryTemplate; // Template asset
    public EnvQueryRunMode RunMode = EnvQueryRunMode.AllMatching;

    [Header("Testing Options")]
    public bool AutoExecute = true; // Auto re-run when position changes
    public float UpdateInterval = 0.5f;

    [Header("Visualization")]
    public bool DrawFailedItems = true;
    public float ItemDrawSize = 0.5f;
    public Color BestColor = Color.blue;
    public Color PassedColor = Color.green;
    public Color FailedColor = Color.red;
    public bool ShowTextScores = true;

    // We store a copy of the results to draw after the instance is disposed
    private EnvQueryItem[] _debugItems;
    private EnvQueryItem _bestItem;

    private Vector3 _lastPosition;
    private float _lastUpdateTime;
    private int _requestID = EnvQueryTypes.INDEX_NONE;

    public EnvQueryItem bestItem=> _bestItem;

    void Update()
    {
        if (Application.isPlaying)
        {
            if (AutoExecute && Time.time - _lastUpdateTime > UpdateInterval)
            {
                if (Vector3.Distance(transform.position, _lastPosition) > 0.1f || Time.time - _lastUpdateTime > UpdateInterval * 2)
                {
                    ExecuteQueryRuntime();
                    _lastPosition = transform.position;
                    _lastUpdateTime = Time.time;
                }
            }
        }
    }

#if UNITY_EDITOR
    private double _lastEditorUpdateTime;

    private void OnEnable()
    {
        EditorApplication.update += EditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= EditorUpdate;
    }

    private void EditorUpdate()
    {
        if (Application.isPlaying) return;

        if (AutoExecute && QueryTemplate != null)
        {
            bool moved = Vector3.Distance(transform.position, _lastPosition) > 0.1f;
            bool timeElapsed = (EditorApplication.timeSinceStartup - _lastEditorUpdateTime) > UpdateInterval;

            if (moved || timeElapsed)
            {
                ExecuteQueryEditor();
                _lastPosition = transform.position;
                _lastEditorUpdateTime = EditorApplication.timeSinceStartup;
            }
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && AutoExecute)
        {
            // Use delayCall to prevent Unity warnings about updating during validation
            EditorApplication.delayCall += () => {
                if (this != null) ExecuteQueryEditor();
            };
        }
    }
#endif

    [ContextMenu("Execute Query (Editor)")]
    public void ExecuteQueryEditor()
    {
        if (QueryTemplate == null) return;

        // Run synchronously in editor
        var instance = new EnvQueryInstance(
            QueryTemplate.QueryName, 
            -1, 
            RunMode, 
            new List<EnvQueryOption>(QueryTemplate.Options), 
            gameObject
        );
        
        while (!instance.IsFinished())
        {
            instance.ExecuteOneStep(100f); // large time slice to finish immediately
        }

        StoreDebugData(instance);
        instance.Dispose();
        
#if UNITY_EDITOR
        // Make sure the scene view reflects the new data
        if (!Application.isPlaying)
        {
            SceneView.RepaintAll();
        }
#endif
    }

    public void ExecuteQueryRuntime()
    {
        if (QueryTemplate == null) return;
        EnvQueryRequest request = new EnvQueryRequest(QueryTemplate, gameObject);
        _requestID = request.Execute(RunMode, OnQueryFinished);
    }

    private void OnQueryFinished(EnvQueryInstance instance)
    {
        StoreDebugData(instance);
        // Note: instance will be Disposed immediately after this callback returns in EnvQueryManager.
    }

    private void StoreDebugData(EnvQueryInstance instance)
    {
        if (instance != null && instance.Items.IsCreated)
        {
            _debugItems = instance.Items.ToArray();
            _bestItem = instance.BestResultItem;
        }
        else
        {
            _debugItems = null;
            _bestItem = default;
        }
    }

    private void OnDrawGizmos()
    {
        if (_debugItems != null)
        {
            for (int i = 0; i < _debugItems.Length; i++)
            {
                var item = _debugItems[i];
                if (item.IsValid)
                {
                    Gizmos.color = PassedColor;
                    Gizmos.DrawWireSphere(item.GetWorldPosition(), ItemDrawSize);

#if UNITY_EDITOR
                    if (ShowTextScores)
                    {
                        GUIStyle style = new GUIStyle();
                        style.normal.textColor = Color.white;
                        style.alignment = TextAnchor.MiddleCenter;
                        Handles.Label(item.GetWorldPosition() + Vector3.up * ItemDrawSize, item.Score.ToString("F2"), style);
                    }
#endif
                }
                else if (DrawFailedItems)
                {
                    Gizmos.color = FailedColor;
                    Gizmos.DrawWireCube(item.GetWorldPosition(), Vector3.one * ItemDrawSize);
                }
            }

            if (_bestItem.IsValid)
            {
                Gizmos.color = BestColor;
                Gizmos.DrawSphere(_bestItem.GetWorldPosition(), ItemDrawSize * 1.2f);
            }
        }
    }
}
