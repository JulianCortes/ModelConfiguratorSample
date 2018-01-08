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
        public ConfigurationIDBase Id
        {
            get
            {
                if (_id == null)
                    _id = GetComponent<ConfigurationIDBase>();
                return _id;
            }
        }

        public enum Status
        {
            None,
            Applied,
            Removed
        }
        [SerializeField]
        private Status _lastStatus = Status.None;

        public Status LastStatus
        {
            get
            {
                return _lastStatus;
            }

            set
            {
                _lastStatus = value;
            }
        }

        private void Start()
        {
            _id = GetComponent<ConfigurationIDBase>();
            _lastStatus = Status.None;
        }

        void Reset()
        {
            _id = GetComponent<ConfigurationIDBase>();
        }

        public void ApplyConfiguration()
        {
            if (gameObjectGroups != null)
                foreach (var g in gameObjectGroups)
                    if (g != null)
                        g.IsActive = true;

            if (materialGroups != null)
                foreach (var m in materialGroups)
                    if (m != null)
                        m.ApplyMaterial();
            _lastStatus = Status.Applied;
        }

        public void Remove()
        {
            if (materialGroups != null)
                foreach (var g in gameObjectGroups)
                    if (g != null)
                        g.IsActive = false;
            _lastStatus = Status.Removed;
        }
    }
}
