using Bunny_TK.ModelConfigurator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModelConfiguratorManager))]
public class ModelConfiguratorManagerEditor : Editor
{

    static bool showAdvancedControls;
    static bool configurationsGrouped = true;
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
    private bool sameDefinitionConfigReferences = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        ConfigurationControls();
        EditorGUILayout.Space();

        InitUpdate();

    }


    private void ConfigurationControls()
    {
        if (GUILayout.Button(showAdvancedControls ? "Hide Advanced Controls" : "Show Advanced Controls", EditorStyles.miniButton))
            showAdvancedControls = !showAdvancedControls;

        if (!showAdvancedControls) return;
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        configurationsGrouped = EditorGUILayout.Toggle("Group by type", configurationsGrouped);
        EditorGUILayout.Space();

        if (configurationsGrouped)
        {
            for (int i = 0; i < Target.currentConfiguration.definition.GetAllTypes().Count(); i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label(Target.currentConfiguration.definition.IndexToType(i));

                foreach (var c in Target.configurations.Where(c => c != null && (c.Id as ConfigurationID).configValues.Exists(v => v.typeIndex == i && v.ValueIndex != ConfigValue.UNDEFINED_VALUE)))
                    if (c != null)
                        ShowConfig(c);

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Previous", EditorStyles.miniButtonLeft))
                {
                    ConfigValue temp = (Target.currentConfiguration as ConfigurationID).configValues.Find(v => v.typeIndex == i);
                    temp.ValueIndex--;
                    if (temp.ValueIndex < 0)
                        temp.ValueIndex = Target.currentConfiguration.definition.GetAllValues(temp.typeIndex).Count() -1;
                    if (temp.ValueIndex >= Target.currentConfiguration.definition.GetAllValues(temp.typeIndex).Count())
                        temp.ValueIndex = 0;
                    Target.ApplyConfiguration();
                }
                if (GUILayout.Button("Next", EditorStyles.miniButtonRight))
                {
                    ConfigValue temp = (Target.currentConfiguration as ConfigurationID).configValues.Find(v => v.typeIndex == i);
                    temp.ValueIndex++;
                    if (temp.ValueIndex < 0)
                        temp.ValueIndex = Target.currentConfiguration.definition.GetAllValues(temp.typeIndex).Count() - 1;
                    if (temp.ValueIndex >= Target.currentConfiguration.definition.GetAllValues(temp.typeIndex).Count())
                        temp.ValueIndex = 0;
                    Target.ApplyConfiguration();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }
        else
        {
            foreach (var c in Target.configurations)
                ShowConfig(c);
            EditorGUILayout.Space();

        }

        if((Target.currentConfiguration as ConfigurationID).definition != null)
        {
            if(GUILayout.Button("Randomize",EditorStyles.miniButton))
            {
                foreach(var v in Target.currentConfiguration.configValues)
                {
                    v.ValueIndex = Random.Range(0, Target.currentConfiguration.definition.GetAllValues(v.typeIndex).Count());
                }
                Target.ApplyConfiguration();
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void ShowConfig(Configuration config)
    {
        Color defColor = GUI.backgroundColor;
        GUI.backgroundColor = config.LastStatus == Configuration.Status.Applied ? Color.green : defColor;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("      ");
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label(config.name);
        //GUILayout.Label(config.LastStatus.ToString());
        GUILayout.FlexibleSpace();
        GUI.backgroundColor = defColor;

        if (GUILayout.Button("Apply", EditorStyles.miniButtonLeft))
            Target.ApplyConfiguration(config.Id as ConfigurationID);
        if (GUILayout.Button("Ping", EditorStyles.miniButtonRight))
            EditorGUIUtility.PingObject(config);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = defColor;
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
        Target.currentConfiguration.definition = definitions;

        EditorGUI.BeginDisabledGroup(definitions == null);
        if (GUILayout.Button("Show", EditorStyles.miniButton, GUILayout.Width(50)))
            EditorGUIUtility.PingObject(definitions);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
    }

    private void InitUpdate()
    {
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
