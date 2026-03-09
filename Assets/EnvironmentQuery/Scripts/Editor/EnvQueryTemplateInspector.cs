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
    // Cache ReorderableLists to avoid recreating them every frame if possible, 
    // but since they depend on index which can change, recreating in OnInspectorGUI is safer/easier for this structure.
    private float lineHeightSpace;

    void OnEnable()
    {
        optionsProp = serializedObject.FindProperty("Options");
        lineHeightSpace = EditorGUIUtility.singleLineHeight + 2;
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
                RemoveOption(i);
                break; // Exit loop since array changed
            }
            EditorGUILayout.EndHorizontal();

            // Draw Generator field
            SerializedProperty generatorProp = optionProp.FindPropertyRelative("Generator");
            DrawGeneratorInspector(generatorProp);

            EditorGUILayout.Space();

            // Draw Tests ReorderableList for this option
            SerializedProperty testsProp = optionProp.FindPropertyRelative("Tests");
            
            // Create list on the fly
            ReorderableList list = GetTestListForOption(testsProp);
            list.DoLayoutList();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add New Option"))
        {
            optionsProp.arraySize++;
            SerializedProperty newOption = optionsProp.GetArrayElementAtIndex(optionsProp.arraySize - 1);
            
            // Clear Generator reference
            newOption.FindPropertyRelative("Generator").objectReferenceValue = null;
            
            // Clear Tests list
            newOption.FindPropertyRelative("Tests").ClearArray();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void RemoveOption(int index)
    {
        SerializedProperty optionProp = optionsProp.GetArrayElementAtIndex(index);
        
        // Remove Generator
        SerializedProperty generatorProp = optionProp.FindPropertyRelative("Generator");
        if (generatorProp.objectReferenceValue != null)
        {
            Undo.DestroyObjectImmediate(generatorProp.objectReferenceValue);
        }

        // Remove Tests
        SerializedProperty testsProp = optionProp.FindPropertyRelative("Tests");
        for (int i = testsProp.arraySize - 1; i >= 0; i--)
        {
            SerializedProperty testProp = testsProp.GetArrayElementAtIndex(i);
            if (testProp.objectReferenceValue != null)
            {
                Undo.DestroyObjectImmediate(testProp.objectReferenceValue);
            }
        }

        optionsProp.DeleteArrayElementAtIndex(index);
        AssetDatabase.SaveAssets();
    }

    private void DrawGeneratorInspector(SerializedProperty generatorProp)
    {
        EditorGUILayout.LabelField("Generator", EditorStyles.boldLabel);
        
        if (generatorProp.objectReferenceValue == null)
        {
             if (EditorGUILayout.DropdownButton(new GUIContent("Create Generator"), FocusType.Keyboard))
             {
                 GenericMenu menu = new GenericMenu();
                 menu.AddItem(new GUIContent("On Circle"), false, () => CreateGenerator(generatorProp, typeof(EnvQueryGeneratorOnCircle)));
                 menu.AddItem(new GUIContent("Simple Grid"), false, () => CreateGenerator(generatorProp, typeof(EnvQueryGeneratorSimpleGrid)));
                 menu.AddItem(new GUIContent("Donut"), false, () => CreateGenerator(generatorProp, typeof(EnvQueryGeneratorDonut)));
                 menu.AddItem(new GUIContent("Actors Of Class"), false, () => CreateGenerator(generatorProp, typeof(EnvQueryGeneratorActorsOfClass)));
                 menu.ShowAsContext();
             }
        }
        else
        {
            // Draw Generator Inspector
            EnvQueryGenerator generator = (EnvQueryGenerator)generatorProp.objectReferenceValue;
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(generator.GetType().Name, EditorStyles.boldLabel);
            
            if (GUILayout.Button("Ping", GUILayout.Width(45)))
            {
                EditorGUIUtility.PingObject(generator);
            }

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                RemoveGenerator(generatorProp);
                // Return early as the object is destroyed
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                return;
            }
            EditorGUILayout.EndHorizontal();
            
            SerializedObject generatorObj = new SerializedObject(generator);
            generatorObj.Update();
            
            SerializedProperty prop = generatorObj.GetIterator();
            bool enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (prop.name == "m_Script") continue;
                EditorGUILayout.PropertyField(prop, true);
            }
            
            generatorObj.ApplyModifiedProperties();
            
            EditorGUILayout.EndVertical();
        }
    }

    private void CreateGenerator(SerializedProperty generatorProp, Type type)
    {
        EnvQueryGenerator generator = (EnvQueryGenerator)ScriptableObject.CreateInstance(type);
        generator.name = type.Name;
        
        AssetDatabase.AddObjectToAsset(generator, target);
        generatorProp.objectReferenceValue = generator;
        
        generatorProp.serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
    }

    private void RemoveGenerator(SerializedProperty generatorProp)
    {
        if (generatorProp.objectReferenceValue != null)
        {
            Undo.DestroyObjectImmediate(generatorProp.objectReferenceValue);
            generatorProp.objectReferenceValue = null;
            generatorProp.serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }
    }

    private ReorderableList GetTestListForOption(SerializedProperty testsProp)
    {
        ReorderableList list = new ReorderableList(testsProp.serializedObject, testsProp, true, true, true, true);

        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Tests");
        };

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            if (index >= testsProp.arraySize) return;
            
            SerializedProperty element = testsProp.GetArrayElementAtIndex(index);
            
            if (element.objectReferenceValue == null)
            {
                EditorGUI.LabelField(rect, "Null Test Object");
                return;
            }

            // Draw the test's inspector properties
            SerializedObject elementObj = new SerializedObject(element.objectReferenceValue);
            elementObj.Update();
            
            // Header for the test item
            float buttonWidth = 45f;
            Rect headerRect = new Rect(rect.x, rect.y, rect.width - buttonWidth - 5, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(headerRect, element.objectReferenceValue.GetType().Name, EditorStyles.boldLabel);

            Rect pingRect = new Rect(rect.x + rect.width - buttonWidth, rect.y, buttonWidth, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(pingRect, "Ping"))
            {
                EditorGUIUtility.PingObject(element.objectReferenceValue);
            }

            SerializedProperty propertyIterator = elementObj.GetIterator();
            int propCount = 1;
            while (propertyIterator.NextVisible(true))
            {
                // Skip script field
                if (propertyIterator.name == "m_Script") continue;
                
                float yPos = rect.y + (lineHeightSpace * propCount);
                EditorGUI.PropertyField(new Rect(rect.x, yPos, rect.width, EditorGUIUtility.singleLineHeight), 
                                        propertyIterator, true);
                propCount++;
            }

            elementObj.ApplyModifiedProperties();
        };

        list.elementHeightCallback = (int index) =>
        {
            if (index >= testsProp.arraySize) return EditorGUIUtility.singleLineHeight;
            
            SerializedProperty element = testsProp.GetArrayElementAtIndex(index);
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
            menu.AddItem(new GUIContent("Distance"), false, () => CreateTest(testsProp, typeof(EnvQueryTestDistance)));
            menu.AddItem(new GUIContent("Dot"), false, () => CreateTest(testsProp, typeof(EnvQueryTestDot)));
            menu.AddItem(new GUIContent("PathFinding"), false, () => CreateTest(testsProp, typeof(EnvQueryTestPathFinding)));
            menu.AddItem(new GUIContent("Trace"), false, () => CreateTest(testsProp, typeof(EnvQueryTestTrace)));
            menu.DropDown(buttonRect);
        };

        list.onRemoveCallback = (l) => {
            SerializedProperty element = testsProp.GetArrayElementAtIndex(l.index);
            if (element.objectReferenceValue != null)
            {
                Undo.DestroyObjectImmediate(element.objectReferenceValue);
                // Also need to remove the array element reference
                // But ReorderableList.defaultBehaviours.DoRemoveButton(l) handles array removal.
                // However, we destroyed the object, so the reference is now null/missing.
            }
            ReorderableList.defaultBehaviours.DoRemoveButton(l);
            AssetDatabase.SaveAssets();
        };

        return list;
    }

    private void CreateTest(SerializedProperty testsProp, Type type)
    {
        EnvQueryTest test = (EnvQueryTest)ScriptableObject.CreateInstance(type);
        test.name = type.Name;
        
        AssetDatabase.AddObjectToAsset(test, target);
        
        int index = testsProp.arraySize;
        testsProp.arraySize++;
        SerializedProperty element = testsProp.GetArrayElementAtIndex(index);
        element.objectReferenceValue = test;
        
        testsProp.serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
    }
}
