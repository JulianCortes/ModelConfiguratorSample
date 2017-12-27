using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bunny_TK.Utils
{
    public class MaterialsUtilities
    {
        [MenuItem("Bunny_TK/Mesh \x8B& Materials/Materials/Select all Materials")]
        public static void SelectAllMaterialsUsed()
        {
            var transforms = Selection.transforms;

            var mats = new List<Material>();

            foreach (var transform in transforms)
            {
                var renderers = transform.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    var sharedMats = renderer.sharedMaterials;

                    foreach (var sharedMat in sharedMats)
                    {
                        mats.Add(sharedMat);
                    }
                }
            }
            Selection.objects = mats.ToArray();
        }

        [MenuItem("Bunny_TK/Mesh \x8B& Materials/Select all Child Renderer")]
        public static void SelectAllChildRenderers()
        {
            var transforms = Selection.transforms;
            var renderers = new List<Renderer>();

            foreach (var transform in transforms)
            {
                renderers.AddRange(transform.GetComponentsInChildren<Renderer>());
            }
            Selection.objects = renderers.Select(r => r.gameObject).ToArray();
        }
    }
}