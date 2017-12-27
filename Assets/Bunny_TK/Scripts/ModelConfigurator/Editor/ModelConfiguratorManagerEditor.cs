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

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        definitions = (ConfigurationDefinitions)EditorGUILayout.ObjectField("Definitions", definitions, typeof(ConfigurationDefinitions), true);

        if (definitions != null)
        {
            if (GUILayout.Button("Initialize"))
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
            }
            Target.configurations = FindObjectsOfType<Configuration>()
                                   .Where(c =>c .Id != null && c.Id.GetType() == typeof(ConfigurationID)).ToList();
            EditorUtility.SetDirty(Target.gameObject);
        }
    }
}
