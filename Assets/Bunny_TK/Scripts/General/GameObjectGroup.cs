using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bunny_TK.Utils
{
    public class GameObjectGroup : MonoBehaviour
    {
        [Header("Group")]
        [SerializeField]
        private List<GameObject> _gameObjects;

        [SerializeField]
        private bool _isActive;

        [Header("Disabled when active")]
        [SerializeField]
        private List<GameObject> _gameObjectsToDisable;

        [SerializeField]
        private List<GameObjectGroup> _groupsToDisable;

        public bool IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                _isActive = value;
                foreach (var go in _gameObjects)
                    go.SetActive(value);

                if (_isActive)
                {
                    foreach (var go in _gameObjectsToDisable)
                        go.SetActive(false);

                    foreach (var gr in _groupsToDisable)
                        gr.IsActive = false;
                }
            }
        }

        public List<GameObject> GetAllGameObjects()
        {
            return new List<GameObject>(_gameObjects);
        }

        public List<GameObject> GetAllDisabledGameObjects()
        {
            List<GameObject> t = new List<GameObject>(_gameObjectsToDisable);

            foreach (var gr in _groupsToDisable)
                t.AddRange(gr.GetAllGameObjects());

            return t;
        }

        public void RemoveDuplicatesOrMissing()
        {
            if (_gameObjects != null && _gameObjects.Count > 1)
            {
                _gameObjects.RemoveAll(g => g == null);
                HashSet<GameObject> t = new HashSet<GameObject>(_gameObjects);
                _gameObjects = t.ToList();
            }
        }
    }
}