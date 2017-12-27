using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bunny_TK.ModelConfigurator
{
    /// <summary>
    /// Base functions of a configuration ID.
    /// </summary>
    public abstract class ConfigurationIDBase : MonoBehaviour
    {
        /// <summary>
        /// By pairing the values with same type with the other configuration, this should return TRUE if all paired values are either the same or at least one of them is UNDEFINED.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool Similar(ConfigurationIDBase other);
        /// <summary>
        /// Should return TRUE if this and other have the same values.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool Same(ConfigurationIDBase other);
        /// <summary>
        /// How this and the other will merge in one. The result should be applied in this.
        /// </summary>
        /// <param name="other"></param>
        public abstract void Overlap(ConfigurationIDBase other);

        public abstract IEnumerable<string> GetAllTypes();
        public abstract IEnumerable<string> GetAllValues(int typeIndex);
    }
}
