using Bunny_TK.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bunny_TK.ModelConfigurator
{
    [RequireComponent(typeof(ConfigurationIDBase))]
    public class Configuration : MonoBehaviour
    {
        public List<GameObjectGroup> gameObjectGroups;
        public List<MaterialManager> materialGroups;

        private ConfigurationIDBase _id;
        public ConfigurationIDBase Id { get { return _id; } }

        void Reset()
        {
            _id = GetComponent<ConfigurationIDBase>();
        }

        public void ApplyConfiguration()
        {
            foreach (var g in gameObjectGroups)
                g.IsActive = true;

            foreach (var m in materialGroups)
                m.ApplyMaterial();
        }

        public void Remove()
        {
            foreach (var g in gameObjectGroups)
                g.IsActive = false;
        }
    }
}
