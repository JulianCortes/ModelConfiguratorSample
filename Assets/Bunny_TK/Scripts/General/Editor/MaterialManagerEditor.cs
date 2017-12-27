using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Bunny_TK.Utils
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MaterialManager))]
    public class MaterialManagerEditor : Editor
    {

        GameObject go = null;

        MaterialManager[] _Targets
        {
            get
            {
                List<MaterialManager> tmp = new List<MaterialManager>();
                foreach (var t in targets)
                    tmp.Add(t as MaterialManager);

                return tmp.ToArray();
            }
        }
        bool isVisible = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Only", EditorStyles.boldLabel);
            if (GUILayout.Button("Apply Material"))
            {
                foreach (var t in _Targets)
                {
                    Undo.RecordObjects(t.GetTargetMeshes(), "Apply Material");
                    t.ApplyMaterial();
                }
            }

            if (GUILayout.Button("Select Meshes"))
            {
                SelectMeshes();
            }
            if (GUILayout.Button("Remove Duplicate and Empty"))
            {
                RemoveDuplicateAndEmpty();
            }

            EditorGUILayout.Space();
            go = EditorGUILayout.ObjectField(go, typeof(GameObject), true) as GameObject;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Add Mesh"))
            {
                Undo.RecordObjects(_Targets, "Add Mesh");
                AddMesh();
            }
            if (GUILayout.Button("Add Meshes Child"))
            {
                Undo.RecordObjects(_Targets, "Add Meshes");
                AddMeshesChild();
            }
            if (GUILayout.Button("Add Meshes from MaterialManager"))
            {
                Undo.RecordObjects(_Targets, "Add Meshes");
                AddMeshesFromMaterialManager();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Remove Meshe"))
            {
                Undo.RecordObjects(_Targets, "Remove Mesh");
                RemoveMesh();
            }
            if (GUILayout.Button("Remove Meshes Child"))
            {
                Undo.RecordObjects(_Targets, "Remove Meshes");
                RemoveMeshChild();
            }
            if (GUILayout.Button("Remove Meshes Selected"))
            {
                Undo.RecordObjects(_Targets, "Remove Meshes Selected");
                RemoveMeshesSelected();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Toggle Active GOs"))
            {
                Undo.RecordObjects(_Targets, "Toggle Active");
                foreach (var t in _Targets)
                    foreach (var m in t.GetTargetMeshes())
                        m.gameObject.SetActive(!isVisible);
                isVisible = !isVisible;
            }

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
    }
}
