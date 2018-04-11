using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bunny_TK.Utils
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MaterialManager))]
    public class MaterialManagerEditor : Editor
    {
        private static List<MeshRenderer> copyList = new List<MeshRenderer>();
        private GameObject go = null;
        private bool isVisible = true;
        private MaterialManager[] _Targets
        {
            get
            {
                List<MaterialManager> tmp = new List<MaterialManager>();
                foreach (var t in targets)
                    tmp.Add(t as MaterialManager);

                return tmp.ToArray();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Editor Only", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Preview");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply Material", EditorStyles.miniButtonLeft))
            {
                foreach (var t in _Targets)
                {
                    Undo.RecordObjects(t.GetTargetMeshes(), "Apply Material");
                    t.ApplyMaterial();
                }
            }
            if (GUILayout.Button("Select Meshes", EditorStyles.miniButtonRight))
            {
                SelectMeshes();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Add/Remove");
            go = EditorGUILayout.ObjectField("Target", go, typeof(GameObject), true) as GameObject;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Add Target Mesh", EditorStyles.miniButtonLeft))
            {
                Undo.RecordObjects(_Targets, "Add Mesh");
                AddMesh();
            }
            if (GUILayout.Button("Add Target Meshes Child", EditorStyles.miniButtonLeft))
            {
                Undo.RecordObjects(_Targets, "Add Meshes");
                AddMeshesChild();
            }
            if (GUILayout.Button("Add Meshes from MaterialManager", EditorStyles.miniButtonLeft))
            {
                Undo.RecordObjects(_Targets, "Add Meshes");
                AddMeshesFromMaterialManager();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Remove Target Mesh", EditorStyles.miniButtonRight))
            {
                Undo.RecordObjects(_Targets, "Remove Mesh");
                RemoveMesh();
            }
            if (GUILayout.Button("Remove Target Meshes Child", EditorStyles.miniButtonRight))
            {
                Undo.RecordObjects(_Targets, "Remove Meshes");
                RemoveMeshChild();
            }
            if (GUILayout.Button("Remove Meshes Selected", EditorStyles.miniButtonRight))
            {
                Undo.RecordObjects(_Targets, "Remove Meshes Selected");
                RemoveMeshesSelected();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Toggle Active GOs", EditorStyles.miniButton))
            {
                Undo.RecordObjects(_Targets, "Toggle Active");
                foreach (var t in _Targets)
                    foreach (var m in t.GetTargetMeshes())
                        m.gameObject.SetActive(!isVisible);
                isVisible = !isVisible;
            }
            if (GUILayout.Button("Remove Duplicates or Missing", EditorStyles.miniButton))
                RemoveDuplicateAndEmpty();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Substitute");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy", EditorStyles.miniButtonLeft))
                Copy();
            EditorGUI.BeginDisabledGroup(copyList == null || copyList.Count <= 0);
            if (GUILayout.Button("Paste", EditorStyles.miniButtonRight))
                Paste();
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Select Copied", EditorStyles.miniButton))
                SelectCopied();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private void SelectMeshes()
        {
            List<GameObject> tmp = new List<GameObject>();
            foreach (var matM in _Targets)
                foreach (var meshes in matM.GetTargetMeshes())
                    tmp.Add(meshes.gameObject);

            Selection.objects = tmp.ToArray();
        }

        private void RemoveDuplicateAndEmpty()
        {
            foreach (var m in _Targets)
            {
                m.RemoveEmptyEntries();
                m.RemoveDuplicates();
            }
        }

        private void AddMesh()
        {
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mr == null) return;

            foreach (var t in _Targets)
                t.AddMesh(mr);
        }

        private void AddMeshesChild()
        {
            foreach (var t in _Targets)
                t.AddChildMeshes(go.transform);
        }

        private void AddMeshesFromMaterialManager()
        {
            MaterialManager m = go.GetComponent<MaterialManager>();
            if (m == null) return;

            foreach (var t in _Targets)
            {
                t.AddMesh(m.GetTargetMeshes());
            }
        }

        private void RemoveMesh()
        {
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mr == null) return;

            foreach (var t in _Targets)
                t.RemoveMesh(mr);
        }

        private void RemoveMeshChild()
        {
            foreach (var t in _Targets)
                t.RemoveChildMeshes(go.transform);
        }

        private void RemoveMeshesSelected()
        {
            if (Selection.objects.Count() > 0)
            {
                MeshRenderer mr = null;
                List<MeshRenderer> tmp = new List<MeshRenderer>();

                foreach (var o in Selection.objects)
                {
                    mr = ((GameObject)o).GetComponent<MeshRenderer>();
                    if (mr != null)
                        tmp.Add(mr);
                }

                foreach (var t in _Targets)
                    t.RemoveMesh(tmp);
            }
        }

        private void Copy()
        {
            if (_Targets.Count() == 1)
                copyList = new List<MeshRenderer>(_Targets[0].GetTargetMeshes());
        }

        private void Paste()
        {
            if (copyList == null) return;
            if (copyList.Count <= 0) return;

            foreach (var t in _Targets)
                t.SetTargetMeshes(copyList);
        }

        private void SelectCopied()
        {
            List<GameObject> tmp = new List<GameObject>();
            foreach (var meshes in copyList)
                tmp.Add(meshes.gameObject);

            Selection.objects = tmp.ToArray();
        }
    }
}