using Bunny_TK.ModelConfigurator;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModelConfiguratorManager))]
public class ModelConfiguratorManagerEditor : Editor
{
    private static bool showAdvancedControls = true;
    private static bool configurationsGrouped = true;
    private static ConfigurationID targetConf;
    private ConfigurationDefinitions definitions;
    private List<GameObject> types;
    private int indexDef;
    private int indexConfigReferences;
    private bool sameDefinitionConfigReferences = true;

    private ModelConfiguratorManager Target
    {
        get { return target as ModelConfiguratorManager; }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        //Toggle button show controls
        if (GUILayout.Button(showAdvancedControls ? "Hide Advanced Controls" : "Show Advanced Controls", EditorStyles.miniButton))
            showAdvancedControls = !showAdvancedControls;

        EditorGUILayout.Space();

        if (showAdvancedControls)
        {
            DefinitionInitControls();
            EditorGUILayout.Space();
            ConfigurationControls();
        }
    }

    private void ConfigurationControls()
    {
        //No configurations
        if (Target.configurations == null || Target.configurations.Count <= 0)
            return;

        //No definitions
        if (Target.currentConfiguration.definition == null)
        {
            ChangeConfigControls();
            return;
        }

        //Start frame
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Preview");
        EditorGUILayout.Space();
        ChangeConfigControls();
        EditorGUILayout.Space();

        //Sort mode
        configurationsGrouped = EditorGUILayout.Toggle("Group by type", configurationsGrouped);

        //Group by type
        if (configurationsGrouped)
        {
            for (int i = 0; i < Target.currentConfiguration.definition.GetAllTypes().Count(); i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label(Target.currentConfiguration.definition.IndexToType(i));

                foreach (var c in Target.configurations.Where(c => c != null && (c.Id as ConfigurationID).configValues
                                                       .Exists(v => v.typeIndex == i && v.ValueIndex != ConfigValue.UNDEFINED_VALUE)))
                    if (c != null)
                        ShowConfigDetails(c);

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Previous", EditorStyles.miniButtonLeft))
                {
                    ConfigValue temp = (Target.currentConfiguration as ConfigurationID).configValues.Find(v => v.typeIndex == i);
                    temp.ValueIndex--;
                    if (temp.ValueIndex < 0)
                        temp.ValueIndex = Target.currentConfiguration.definition.GetAllValues(temp.typeIndex).Count() - 1;
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
        else //No grouping
        {
            foreach (var c in Target.configurations)
                ShowConfigDetails(c);
            EditorGUILayout.Space();
        }

        //Randomize button
        if (GUILayout.Button("Randomize", EditorStyles.miniButton))
        {
            foreach (var v in Target.currentConfiguration.configValues)
                v.ValueIndex = Random.Range(0, Target.currentConfiguration.definition.GetAllValues(v.typeIndex).Count());
            Target.ApplyConfiguration();
        }

        EditorGUILayout.EndVertical();
    }

    private void ShowConfigDetails(Configuration config)
    {
        Color defColor = GUI.backgroundColor;
        GUI.backgroundColor = config.LastStatus == Configuration.Status.Applied ? Color.green : defColor;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("      ");
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label(config.name);
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

    /// <summary>
    /// Apply configuration by reference.
    /// </summary>
    private void ChangeConfigControls()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(targetConf == null);
        if (GUILayout.Button("Apply", EditorStyles.miniButton))
            Target.ApplyConfiguration(targetConf);

        EditorGUI.EndDisabledGroup();
        targetConf = EditorGUILayout.ObjectField(targetConf, typeof(ConfigurationID), true) as ConfigurationID;
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Popup from Resources folder.
    /// </summary>
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

    /// <summary>
    /// Configurator definition and references controls.
    /// </summary>
    private void DefinitionInitControls()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        ShowAllDefinitions();

        if (definitions != null)
        {
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

    /// <summary>
    /// Search Configuration references.
    /// </summary>
    /// <param name="global"></param>
    /// <param name="sameDefinition">Same definition as CurrentConfiguration.</param>
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