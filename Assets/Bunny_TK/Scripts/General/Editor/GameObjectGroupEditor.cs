using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bunny_TK.Utils
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(GameObjectGroup))]
    public class GameObjectGroupEditor : Editor
    {
        GameObjectGroup[] _Targets
        {
            get
            {
                return targets.Cast<GameObjectGroup>().ToArray();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Only", EditorStyles.boldLabel);

            if (GUILayout.Button("Remove duplicates and missing"))
            {
                RemoveDuplicatesOrMissing();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select target GOs"))
            {
                SelectAllTargetGameObjects();
            }

            if (GUILayout.Button("Select GOs to disable"))
            {
                SelectAllDisabledGameObjects();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Toggle"))
            {
                foreach (var t in _Targets)
                    t.IsActive = !t.IsActive;
            }

        }


        void RemoveDuplicatesOrMissing()
        {
            Undo.RecordObjects(targets, "Remove Duplicates");
            foreach (var t in _Targets)
                t.RemoveDuplicatesOrMissing();
        }

        void SelectAllTargetGameObjects()
        {
            List<GameObject> goTargets = new List<GameObject>();
            foreach (var t in _Targets)
                goTargets.AddRange(t.GetAllGameObjects());

            Selection.objects = goTargets.ToArray();
        }

        void SelectAllDisabledGameObjects()
        {
            List<GameObject> goTargets = new List<GameObject>();
            foreach (var t in _Targets)
                goTargets.AddRange(t.GetAllDisabledGameObjects());

            Selection.objects = goTargets.ToArray();
        }
    }
}
