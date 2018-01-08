using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bunny_TK.ModelConfigurator
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Configuration))]
    public class ConfigurationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Apply", EditorStyles.miniButtonLeft))
                ApplyConfiguration();

            if (GUILayout.Button("Remove", EditorStyles.miniButtonRight))
                RemoveConfiguration();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

        }

        private void ApplyConfiguration()
        {
            foreach (var t in targets.Cast<Configuration>())
                t.ApplyConfiguration();
        }

        private void RemoveConfiguration()
        {
            foreach (var t in targets.Cast<Configuration>())
                t.Remove();
        }
    }

}
