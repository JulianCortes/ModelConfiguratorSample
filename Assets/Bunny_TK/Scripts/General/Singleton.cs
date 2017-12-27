using UnityEngine;

namespace Bunny_TK.Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;

        public static T Instance
        {
            set
            {
                if (value == null)
                {
                    instance = null;
                }
                else
                {
                    Debug.LogError("Error, cannot assign a non-null value to singleton instance");
                }
            }

            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));
                    if (instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(T) +
                        " is needed in the scene or in resources, but there is none.");
                    }
                }
                return instance;
            }
        }

        public static bool InstanceExists
        {
            get
            {
                return instance != null;
            }
        }
    }
}