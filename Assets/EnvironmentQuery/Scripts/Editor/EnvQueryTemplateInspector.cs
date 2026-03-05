using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(EnvQueryTemplate))]
public class EnvQueryTemplateInspector : Editor
{
    private SerializedProperty optionsProp;
    private List<ReorderableList> testReorderableLists = new List<ReorderableList>();
    private float lineHeightSpace;

    void OnEnable()
    {
        optionsProp = serializedObject.FindProperty("Options");
        lineHeightSpace = EditorGUIUtility.singleLineHeight + 3;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("QueryName"));
        EditorGUILayout.Space();

        // Handle Options
        for (int i = 0; i < optionsProp.arraySize; i++)
        {
            SerializedProperty optionProp = optionsProp.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Draw Option Header (with Remove button)
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Option {i}", EditorStyles.boldLabel);
            if (GUILayout.Button("Remove Option", GUILayout.Width(100)))
            {
                optionsProp.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();

            // Draw Generator field
            SerializedProperty generatorProp = optionProp.FindPropertyRelative("Generator");
            EditorGUILayout.PropertyField(generatorProp);

            // Draw Tests ReorderableList for this option
            SerializedProperty testsProp = optionProp.FindPropertyRelative("Tests");
            
            // Each option needs its own reorderable list instance
            ReorderableList list = GetTestListForOption(i, testsProp);
            list.DoLayoutList();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add New Option"))
        {
            optionsProp.arraySize++;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private ReorderableList GetTestListForOption(int index, SerializedProperty testsProp)
    {
        // For simplicity, we can create it on the fly or cache it. 
        // Re-creating on the fly in OnInspectorGUI is generally fine for ReorderableList if we pass the right SerializedProperty.
        ReorderableList list = new ReorderableList(testsProp.serializedObject, testsProp, true, true, true, true);

        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Tests");
        };

        list.drawElementCallback = (Rect rect, int testIndex, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = testsProp.GetArrayElementAtIndex(testIndex);
            
            if (element.objectReferenceValue == null)
            {
                EditorGUI.LabelField(rect, "Null Test Object");
                return;
            }

            // Draw the test's inspector properties
            SerializedObject elementObj = new SerializedObject(element.objectReferenceValue);
            elementObj.Update();
            
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                                 element.objectReferenceValue.GetType().Name, EditorStyles.boldLabel);

            SerializedProperty propertyIterator = elementObj.GetIterator();
            int propCount = 1;
            while (propertyIterator.NextVisible(true))
            {
                // Skip script field
                if (propertyIterator.name == "m_Script") continue;
                
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * propCount), rect.width, EditorGUIUtility.singleLineHeight), 
                                        propertyIterator, true);
                propCount++;
            }

            elementObj.ApplyModifiedProperties();
        };

        list.elementHeightCallback = (int testIndex) =>
        {
            SerializedProperty element = testsProp.GetArrayElementAtIndex(testIndex);
            if (element.objectReferenceValue == null) return EditorGUIUtility.singleLineHeight;

            SerializedObject elementObj = new SerializedObject(element.objectReferenceValue);
            SerializedProperty propertyIterator = elementObj.GetIterator();
            int propCount = 1; // For the header
            while (propertyIterator.NextVisible(true))
            {
                if (propertyIterator.name == "m_Script") continue;
                propCount++;
            }
            return lineHeightSpace * propCount + 5;
        };

        list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Distance"), false, (t) => AddTest(t, testsProp), "Distance");
            menu.AddItem(new GUIContent("Dot"), false, (t) => AddTest(t, testsProp), "Dot");
            menu.AddItem(new GUIContent("PathFinding"), false, (t) => AddTest(t, testsProp), "PathFinding");
            menu.AddItem(new GUIContent("Trace"), false, (t) => AddTest(t, testsProp), "Trace");
            menu.DropDown(buttonRect);
        };

        list.onRemoveCallback = (l) => {
            SerializedProperty element = testsProp.GetArrayElementAtIndex(l.index);
            if (element.objectReferenceValue != null)
            {
                UnityEngine.Object.DestroyImmediate(element.objectReferenceValue, true);
                AssetDatabase.SaveAssets();
            }
            ReorderableList.defaultBehaviours.DoRemoveButton(l);
        };

        return list;
    }

    private void AddTest(object type, SerializedProperty testsProp)
    {
        string testType = (string)type;
        EnvQueryTest test = null;

        switch (testType)
        {
            case "Distance": test = ScriptableObject.CreateInstance<EnvQueryTestDistance>(); break;
            case "Dot": test = ScriptableObject.CreateInstance<EnvQueryTestDot>(); break;
            case "PathFinding": test = ScriptableObject.CreateInstance<EnvQueryTestPathFinding>(); break;
            case "Trace": test = ScriptableObject.CreateInstance<EnvQueryTestTrace>(); break;
        }

        if (test != null)
        {
            test.name = testType;
            // Add as sub-asset
            AssetDatabase.AddObjectToAsset(test, target);
            
            int index = testsProp.arraySize;
            testsProp.arraySize++;
            SerializedProperty element = testsProp.GetArrayElementAtIndex(index);
            element.objectReferenceValue = test;
            
            testsProp.serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }
    }
}
