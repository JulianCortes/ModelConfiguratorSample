using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class HotkeysTools : MonoBehaviour
{
    //% ctrl
    //# shift
    //& alt
    static Transform _currentParent;

    #region GameObject Utilities
    [MenuItem("Bunny_TK/GameObject/Toggle active %q")]
    static void ToggleActiveGameObject()
    {
        if (Selection.transforms != null && Selection.transforms.Length > 0)
        {
            foreach (Transform t in Selection.transforms)
            {
                Undo.RegisterCompleteObjectUndo(t.gameObject, "Toggle Active");
                t.gameObject.SetActive(!t.gameObject.activeSelf);
            }
        }
    }

    [MenuItem("Bunny_TK/GameObject/Toggle active %q", true)]
    static bool ValidateToggleActiveGameObject()
    {
        return Selection.transforms != null && Selection.transforms.Length > 0;
    }


    [MenuItem("Bunny_TK/GameObject/Set Current Parent %w")]
    static void SetCurrentParent()
    {
        if (Selection.transforms != null && Selection.transforms.Length == 1)
        {
            _currentParent = Selection.transforms[0];
            EditorGUIUtility.PingObject(_currentParent.gameObject);
        }
    }

    [MenuItem("Bunny_TK/GameObject/Set child to Current Parent %e")]
    static void SetParentSelection()
    {
        if (_currentParent != null && Selection.transforms != null && Selection.transforms.Length > 0)
        {
            foreach (Transform t in Selection.transforms)
                Undo.SetTransformParent(t, _currentParent, "UtilSetChild");
            EditorGUIUtility.PingObject(_currentParent.gameObject);
        }
        else
        {
            Debug.LogWarning("No current selected parent!");
        }
    }

    [MenuItem("Bunny_TK/GameObject/Create Empty Child %#q")]
    static void CreateEmpty()
    {
        if (Selection.activeTransform != null)
        {

            GameObject g = new GameObject();
            g.name = "GameObject_";

            g.transform.SetParent(Selection.activeTransform);
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            g.transform.localScale = Vector3.one;

            Selection.activeTransform = g.transform;
            EditorGUIUtility.PingObject(g);
        }
    }

    #endregion GameObject Utilities



}
