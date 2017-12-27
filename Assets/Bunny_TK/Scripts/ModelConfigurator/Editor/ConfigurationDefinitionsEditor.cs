using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bunny_TK.ModelConfigurator
{
    [CustomEditor(typeof(ConfigurationDefinitions))]
    public class ConfigurationDefinitionsEditor : Editor
    {
        //Custom inspector for ConfigurationDefinitions.
        //Allows easy management of definitions:
        // - Add, Edit, Remove values
        // - Auto updates all ConfigurationGeneric
        // - Human-readable restrisctions

        private ConfigurationDefinitions Def
        {
            get
            {
                return (ConfigurationDefinitions)target;
            }
        }

        private Color defaultColor;
        private bool showNotes = true;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            defaultColor = GUI.color;
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("definitionName"));
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Types & Values", EditorStyles.boldLabel);
                //Notes
                EditorGUI.indentLevel++;
                showNotes = EditorGUILayout.Foldout(showNotes, "Notes");
                if (showNotes)
                {
                    EditorGUILayout.LabelField("- Values[" + ConfigValue.UNDEFINED_VALUE + "] is always UNDEFINED_VALUE.", EditorStyles.miniLabel);
                    EditorGUILayout.LabelField("- All configurations with this definition will be updated on change.", EditorStyles.miniLabel);
                    EditorGUILayout.LabelField("- This script DOESN'T SUPPORT UNDO functionalities yet.", EditorStyles.miniLabel);
                }
                EditorGUI.indentLevel--;

                //Definitions
                for (int i = 0; i < Def.definitions.Count; i++)
                    ShowTypeValues(Def.definitions[i]);

                //Controls
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add", EditorStyles.miniButtonLeft))
                        Def.definitions.Add(new ConfigurationDefinitions.ConfigTypeValues());
                    if (GUILayout.Button("Clear", EditorStyles.miniButtonRight))
                        Def.definitions.Clear();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Restriction rules", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("A not allowed with B and viceversa.", EditorStyles.miniLabel);

                var restrictions = new List<ConfigMaskMatch>(Def.restrictions);
                if (restrictions.Count > 0)
                {
                    //Header
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Type", EditorStyles.centeredGreyMiniLabel);
                        GUILayout.Label("Values", EditorStyles.centeredGreyMiniLabel);
                        GUILayout.Label("Fallback", EditorStyles.centeredGreyMiniLabel);
                        GUILayout.Label(" ", GUILayout.Width(15));
                    }
                    GUILayout.EndHorizontal();

                    //Content
                    GUI.color = Color.black;
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.color = defaultColor;

                    for (int i = 0; i < restrictions.Count; i++)
                        ShowRestriction(restrictions[i], i);
                    GUILayout.EndVertical();
                }

                //Controls
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add", EditorStyles.miniButtonLeft, GUILayout.Width(50)))
                        Def.restrictions.Add(new ConfigMaskMatch());
                    if (GUILayout.Button("Clear", EditorStyles.miniButtonRight, GUILayout.Width(50)))
                        Def.restrictions.Clear();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            //Update all configs if modified
            if (EditorGUI.EndChangeCheck())
                ForceUpdateAllConfigurations();

            serializedObject.ApplyModifiedProperties();
        }

        public void ShowRestriction(ConfigMaskMatch match, int index)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField(index + ".", GUILayout.Width(15));
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("A", GUILayout.Width(12));
                        match.indexType = EditorGUILayout.Popup(match.indexType, Def.GetAllTypes().ToArray());
                        match.maskIndex = EditorGUILayout.MaskField(match.maskIndex, Def.GetAllValues(match.indexType).ToArray());
                        List<string> fallbackValues = Def.GetAllValues(match.indexType).ToList();
                        match.fallbackIndexValue = EditorGUILayout.Popup(match.fallbackIndexValue, fallbackValues.ToArray());
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("B", GUILayout.Width(12));
                        match.otherIndexType = EditorGUILayout.Popup(match.otherIndexType, Def.GetAllTypes().ToArray());
                        match.otherMaskIndex = EditorGUILayout.MaskField(match.otherMaskIndex, Def.GetAllValues(match.otherIndexType).ToArray());
                        List<string> otherFallbackValues = Def.GetAllValues(match.otherIndexType).ToList();
                        match.otherFallbackIndexValue = EditorGUILayout.Popup(match.otherFallbackIndexValue, otherFallbackValues.ToArray());
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(15)))
                    Def.restrictions.Remove(match);
            }
            GUILayout.EndHorizontal();
        }

        public void ShowTypeValues(ConfigurationDefinitions.ConfigTypeValues configTypeValues)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            configTypeValues.type = EditorGUILayout.TextField("Type", configTypeValues.type);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PrefixLabel("Values");
                EditorGUI.indentLevel += 2;
                if (configTypeValues.values == null)
                    configTypeValues.values = new List<string> { "" };
                for (int i = 0; i < configTypeValues.values.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    configTypeValues.values[i] = EditorGUILayout.TextField("[" + i + "]", configTypeValues.values[i]);
                    if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(15)))
                    {
                        configTypeValues.values.RemoveAt(i);
                        if (configTypeValues.values == null)
                            configTypeValues.values = new List<string> { "" };
                    }
                    GUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel -= 2;

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add", EditorStyles.miniButtonLeft))
                        configTypeValues.values.Add("");
                    if (GUILayout.Button("Clear", EditorStyles.miniButtonMid))
                        configTypeValues.values.Clear();
                    if (GUILayout.Button("Remove", EditorStyles.miniButtonRight))
                        Def.definitions.Remove(configTypeValues);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        public void ForceUpdateAllConfigurations()
        {
            foreach (var c in FindObjectsOfType<ConfigurationID>().Where(c => c.definition == Def))
            {
                c.UpdateConfigs(Def);
                EditorUtility.SetDirty(c.gameObject);
            }
        }
    }
}