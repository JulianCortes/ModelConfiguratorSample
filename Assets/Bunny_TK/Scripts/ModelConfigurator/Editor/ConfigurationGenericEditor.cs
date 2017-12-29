using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bunny_TK.ModelConfigurator
{
    [CustomEditor(typeof(ConfigurationID))]
    public class ConfigurationGenericEditor : Editor
    {
        private ConfigurationID TargetConfig
        {
            get
            {
                return target as ConfigurationID;
            }
        }

        private bool isValid = true;
        private int indexDef = 0;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            ShowAllDefinitions();
            ConfigurationDefinitions def = TargetConfig.definition;

            if (TargetConfig.definition == null)
            {
                EditorGUILayout.HelpBox("Definition is NULL", MessageType.Error);
                base.OnInspectorGUI();
                EditorGUI.EndChangeCheck();
                return;
            }

            //Rimuovi i superflui
            int diff = TargetConfig.configValues.Count() - def.GetAllTypes().Count();
            if (diff > 0) TargetConfig.configValues.RemoveRange(def.GetAllTypes().Count(), diff);

            int i = 0;
            foreach (var type in def.GetAllTypes())
            {
                //Aggiungi se necessario
                if (i >= TargetConfig.configValues.Count)
                    TargetConfig.configValues.Add(new ConfigValue());
                TargetConfig.configValues[i].typeIndex = i;

                //Aggiungi valore Undefined
                var values = def.GetAllValues(i).ToList();
                values.Insert(0, "UNDEFINED");

                TargetConfig.configValues[i].valueIndexForEditor = EditorGUILayout.Popup(type, TargetConfig.configValues[i].valueIndexForEditor, values.ToArray());
                i++;
            }

            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
                isValid = TargetConfig.CheckValidity();

            if (!isValid)
                EditorGUILayout.HelpBox(string.Format("Configuration violates restriction rule #{0}.", TargetConfig.GetFirstIndexRestrictionViolated()), MessageType.Error);
        }

        private void ShowAllDefinitions()
        {
            EditorGUILayout.Space();
            List<ConfigurationDefinitions> defs = new List<ConfigurationDefinitions>(Resources.LoadAll<ConfigurationDefinitions>(""));
            List<string> names;

            //Get names
            if (defs == null) names = new List<string>();
            else names = defs.Select(d => d.name).ToList();

            //For null
            names.Insert(0, "None");

            //Update from target
            if (TargetConfig.definition == null) indexDef = 0;
            else indexDef = names.IndexOf(TargetConfig.definition.name);

            EditorGUILayout.BeginHorizontal();
            indexDef = EditorGUILayout.Popup("Definition", indexDef, names.ToArray());

            //Set to target
            if (indexDef <= 0) TargetConfig.definition = null;
            else TargetConfig.definition = defs[indexDef - 1];

            EditorGUI.BeginDisabledGroup(TargetConfig.definition == null);
            if (GUILayout.Button("Show", EditorStyles.miniButton, GUILayout.Width(50)))
                EditorGUIUtility.PingObject(TargetConfig.definition);
            EditorGUI.EndDisabledGroup();

            //TODO:
            // - Add default definition to get when adding new ConfigIDs
            // - En masse modification for ConfigIDs
            // - Maybe make this custom drawer?

            EditorGUILayout.EndHorizontal();
        }
    }
}