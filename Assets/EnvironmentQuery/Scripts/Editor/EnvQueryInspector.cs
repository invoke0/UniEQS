using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnvQuery))]
[CanEditMultipleObjects]
public class EnvQueryInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
	}
}
