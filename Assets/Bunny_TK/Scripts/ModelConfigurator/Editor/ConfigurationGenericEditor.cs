using System.Linq;
using UnityEditor;

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

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (TargetConfig.definition == null)
            {
                base.OnInspectorGUI();
                EditorGUILayout.HelpBox("Definition is NULL", MessageType.Error);
                return;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("definition"));
            ConfigurationDefinitions def = TargetConfig.definition;
            EditorGUI.BeginChangeCheck();
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
    }
}