using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bunny_TK.Utils
{
    [System.Serializable]
    public class MaterialManager : MonoBehaviour
    {
        public string Name;

        [SerializeField]
        public Material targetMaterial;

        [SerializeField]
        private List<MeshRenderer> targetMeshes;

        [SerializeField]
        private int indexMaterial = 0;

        public void ApplyMaterial()
        {
            ApplyMaterial(targetMaterial);
        }
        public void ApplyMaterial(Material targetMaterial)
        {
            foreach (var m in targetMeshes)
                if (m != null)
                {
                    var tmp = m.sharedMaterials;
                    tmp[indexMaterial] = targetMaterial;
                    m.sharedMaterials = tmp;
                }
        }

        public void AddMesh(MeshRenderer newMesh)
        {
            if (!targetMeshes.Contains(newMesh))
                targetMeshes.Add(newMesh);
        }
        public void AddMesh(IEnumerable<MeshRenderer> newMesh)
        {
            foreach (var m in newMesh)
                AddMesh(m);
        }

        public void AddChildMeshes(Transform parent)
        {
            AddMesh(parent.GetComponentsInChildren<MeshRenderer>());
        }
        public void AddChildMesh(IEnumerable<Transform> parents)
        {
            foreach (var t in parents)
                AddChildMeshes(t);
        }

        public void RemoveMesh(MeshRenderer meshToRemove)
        {
            if (targetMeshes.Contains(meshToRemove))
                targetMeshes.Remove(meshToRemove);
        }
        public void RemoveMesh(IEnumerable<MeshRenderer> meshesToRemove)
        {
            foreach (var m in meshesToRemove)
                RemoveMesh(m);
        }

        public void RemoveChildMeshes(Transform parent)
        {
            foreach (var m in parent.GetComponentsInChildren<MeshRenderer>())
                RemoveMesh(m);
        }
        public void RemoveChildMeshes(IEnumerable<Transform> parents)
        {
            foreach (var t in parents)
                RemoveChildMeshes(t);
        }

        public void RemoveDuplicates()
        {
            HashSet<MeshRenderer> h = new HashSet<MeshRenderer>(targetMeshes);
            targetMeshes = h.ToList();
        }
        public void RemoveEmptyEntries()
        {
            targetMeshes.RemoveAll(m => m == null);
        }
        public MeshRenderer[] GetTargetMeshes()
        {
            return targetMeshes.ToArray();
        }

        public void SetTargetMeshes(IEnumerable<MeshRenderer> meshes)
        {
            targetMeshes = new List<MeshRenderer>(meshes);
            RemoveEmptyEntries();
            RemoveDuplicates();
        }
    }
}
