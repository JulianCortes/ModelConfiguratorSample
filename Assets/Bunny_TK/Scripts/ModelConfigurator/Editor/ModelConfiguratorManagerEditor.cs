using Bunny_TK.ModelConfigurator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModelConfiguratorManager))]
public class ModelConfiguratorManagerEditor : Editor
{

    //Deve creare la gerarchia di oggetti nella scena
    //Inizializzazione del target
    ModelConfiguratorManager Target
    {
        get { return target as ModelConfiguratorManager; }
    }

    ConfigurationDefinitions definitions;
    List<GameObject> types;
    private int indexDef;
    private int indexConfigReferences;
    private bool sameDefinitionConfigReferences;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        ShowAllDefinitions();

        if (definitions != null)
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Update Configuration References Conditions");
            EditorGUI.indentLevel++;
            indexConfigReferences = EditorGUILayout.Popup("Hierarchy", indexConfigReferences, new string[] { "In children", "Global" });
            sameDefinitionConfigReferences = EditorGUILayout.Toggle("Same definition", sameDefinitionConfigReferences);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Initialize", EditorStyles.miniButtonLeft))
            {
                if (types == null)
                {
                    types = new List<GameObject>();
                }
                else
                {
                    types.ForEach(g => DestroyImmediate(g));
                    types.Clear();
                }

                Target.GetComponent<ConfigurationID>().definition = definitions;

                //Indice tipo
                int i = 0;
                foreach (var t in definitions.GetAllTypes())
                {
                    GameObject currentType = new GameObject();
                    currentType.transform.SetParent(Target.transform);
                    currentType.name = t;
                    types.Add(currentType);

                    //Indice valore
                    int j = 0;
                    foreach (var v in definitions.GetAllValues(t))
                    {
                        GameObject currentValue = new GameObject();
                        currentValue.transform.SetParent(currentType.transform);
                        currentValue.name = v;

                        var configID = currentValue.AddComponent<ConfigurationID>();
                        configID.UpdateConfigs(definitions);

                        currentValue.AddComponent<Configuration>();

                        //k tipo del configValues
                        for (int k = 0; k < configID.configValues.Count; k++)
                            configID.configValues[k].ValueIndex = i == k ? j : ConfigValue.UNDEFINED_VALUE;
                        j++;
                    }
                    i++;
                }

                Target.configurations = FindObjectsOfType<Configuration>()
                                       .Where(c => c.Id != null && c.Id.GetType() == typeof(ConfigurationID)).ToList();
            }

            if (GUILayout.Button("Update", EditorStyles.miniButtonRight))
                UpdateConfigurationReferences(indexConfigReferences == 1, sameDefinitionConfigReferences);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorUtility.SetDirty(Target.gameObject);
        }
        EditorGUILayout.EndVertical();

    }

    private void ShowAllDefinitions()
    {
        EditorGUILayout.Space();
        List<ConfigurationDefinitions> defs = new List<ConfigurationDefinitions>(Resources.LoadAll<ConfigurationDefinitions>(""));
        List<string> names = defs.Select(d => d.name).ToList();

        names.Insert(0, "None");

        if (Target.currentConfiguration.definition == null)
            indexDef = 0;
        else
            indexDef = names.IndexOf(Target.currentConfiguration.definition.name);

        EditorGUILayout.BeginHorizontal();
        indexDef = EditorGUILayout.Popup("Definition", indexDef, names.ToArray());

        if (indexDef <= 0) definitions = null;
        else definitions = defs[indexDef - 1];

        EditorGUI.BeginDisabledGroup(definitions == null);
        if (GUILayout.Button("Show", EditorStyles.miniButton, GUILayout.Width(50)))
            EditorGUIUtility.PingObject(definitions);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
    }

    private void UpdateConfigurationReferences(bool global, bool sameDefinition)
    {
        if (global)
        {
            if (sameDefinition)
                Target.configurations = FindObjectsOfType<Configuration>()
                                            .Where(c => c.Id.GetType() == typeof(ConfigurationID) && (c.Id as ConfigurationID).definition == definitions)
                                            .Select(c2 => c2.GetComponent<Configuration>()).ToList();
            else
                Target.configurations = FindObjectsOfType<ConfigurationID>()
                                            .Where(c => c.GetComponent<Configuration>() != null)
                                            .Select(c2 => c2.GetComponent<Configuration>()).ToList();
        }
        else
        {
            if (sameDefinition)
                Target.configurations = Target.GetComponentsInChildren<Configuration>()
                                            .Where(c => c.Id.GetType() == typeof(ConfigurationID) && (c.Id as ConfigurationID).definition == definitions)
                                            .Select(c2 => c2.GetComponent<Configuration>()).ToList();
            else
                Target.configurations = Target.GetComponentsInChildren<Configuration>().ToList();
        }
    }


}
