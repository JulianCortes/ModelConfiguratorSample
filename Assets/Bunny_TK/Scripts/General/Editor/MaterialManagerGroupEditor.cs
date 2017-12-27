using Bunny_TK.Utils;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bunny_TK.CarConfigurator
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MaterialManagerGroup))]
    public class MaterialManagerGroupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Get Child MaterialManagers"))
            {
                GetChildManagers();
            }

            if (GUILayout.Button("Apply Materials"))
            {
                ApplyMaterial();
            }
        }

        private void ApplyMaterial()
        {
            Undo.RecordObjects(targets, "Apply Material");
            foreach (var t in targets.Cast<MaterialManagerGroup>())
                t.ApplyMaterial();
        }

        private void GetChildManagers()
        {
            Undo.RecordObjects(targets, "Get Material Managers");
            foreach (var t in targets.Cast<MaterialManagerGroup>())
                t.GetMaterialsManagersFromParent();
        }
    }
}