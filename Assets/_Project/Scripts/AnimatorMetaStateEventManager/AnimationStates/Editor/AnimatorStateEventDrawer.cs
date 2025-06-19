using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimatorStateEvent))]
public class AnimatorStateEventDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty stateNameProperty = property.FindPropertyRelative("StateName");
        SerializedProperty stateEventProperty = property.FindPropertyRelative("OnAnimatorStateEvent");

        Rect stateNameRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect stateEventRect = new(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUI.GetPropertyHeight(stateEventProperty));

        EditorGUI.PropertyField(stateNameRect, stateNameProperty);
        EditorGUI.PropertyField(stateEventRect, stateEventProperty, true);

        EditorGUI.EndProperty();

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty stateEventProperty = property.FindPropertyRelative("OnAnimatorStateEvent");
        return EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(stateEventProperty) + 4;
    }
}
