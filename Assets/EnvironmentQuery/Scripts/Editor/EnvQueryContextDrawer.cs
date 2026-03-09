using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnvQueryContext), true)]
public class EnvQueryContextDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects
        Rect prefixRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        float buttonWidth = 60f;
        Rect objectFieldRect = new Rect(prefixRect.x, prefixRect.y, prefixRect.width - buttonWidth - 2, prefixRect.height);
        Rect buttonRect = new Rect(prefixRect.x + prefixRect.width - buttonWidth, prefixRect.y, buttonWidth, prefixRect.height);

        // Draw standard object field
        property.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, GUIContent.none, property.objectReferenceValue, typeof(EnvQueryContext), false);

        // Draw quick select button for defaults
        if (GUI.Button(buttonRect, "Default"))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Querier"), false, () =>
            {
                property.objectReferenceValue = EnvQueryContext_Querier.Default;
                property.serializedObject.ApplyModifiedProperties();
            });
            menu.AddItem(new GUIContent("Item"), false, () =>
            {
                property.objectReferenceValue = EnvQueryContext_Item.Default;
                property.serializedObject.ApplyModifiedProperties();
            });
            
            // Allow selecting null to clear it
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("None"), false, () =>
            {
                property.objectReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            });
            
            menu.ShowAsContext();
        }

        EditorGUI.EndProperty();
    }
}
