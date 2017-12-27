using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bunny_TK.Utils
{
    public class MaterialManagerGroup : MonoBehaviour
    {
        /// <summary>
        /// List of targets.
        /// </summary>
        [SerializeField]
        List<MaterialManager> materialManagers;

        /// <summary>
        /// Parent 
        /// </summary>
        [SerializeField]
        Transform parent;

        public void GetMaterialsManagersFromParent()
        {
            materialManagers = parent.GetComponentsInChildren<MaterialManager>().ToList();
        }

        public void VerifyMeshConflicts()
        {
            int diffCount = 0;
            int currentListCount = 0;

            foreach (var m1 in materialManagers)
            {
                foreach (var m2 in materialManagers)
                {
                    if (m1 == m2) continue;

                    currentListCount = m1.GetTargetMeshes().Length;
                    diffCount = m1.GetTargetMeshes().Except(m2.GetTargetMeshes()).Count();

                    if (currentListCount != diffCount)
                    {
                        print("Mesh Conflict: " + m1.name + " has " + diffCount + " meshes in common with " + m2.name);
                    }
                }
            }
        }

        public void ApplyMaterial()
        {
            foreach (var m in materialManagers)
                m.ApplyMaterial();
        }
    }
}
