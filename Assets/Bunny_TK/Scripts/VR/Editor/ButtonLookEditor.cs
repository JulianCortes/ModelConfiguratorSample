using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ButtonLook), true)]
public class ButtonLookEditor : UnityEditor.UI.ButtonEditor
{

    SerializedProperty fillLengthProp;
    

    protected override void OnEnable()
    {
        base.OnEnable();
        serializedObject.FindProperty("fillLenght");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fillLenght"), new GUIContent("Fill Length"));
        base.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();
        //DrawDefaultInspector();

    }

}
