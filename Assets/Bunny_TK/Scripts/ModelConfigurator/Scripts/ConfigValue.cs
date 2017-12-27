namespace Bunny_TK.ModelConfigurator
{
    [System.Serializable]
    public class ConfigValue
    {
        public static readonly int UNDEFINED_VALUE = -1;

        public int typeIndex;

        public int ValueIndex
        {
            get
            {
                return valueIndexForEditor - 1;
            }
            set
            {
                valueIndexForEditor = value + 1;
            }
        }

        /// <summary>
        /// Used in custom editor where index is shifted +1 and added undefined value at 0.
        /// </summary>
        public int valueIndexForEditor;

        public ConfigValue Overlap(ConfigValue overlapper, bool forceType = false)
        {
            if (!forceType)
                if (typeIndex != overlapper.typeIndex) return null;

            return new ConfigValue
            {
                typeIndex = overlapper.typeIndex,
                ValueIndex = overlapper.ValueIndex == UNDEFINED_VALUE ? this.ValueIndex : overlapper.ValueIndex
            };
        }

        /// <summary>
        /// Two ConfigValues are Similar if (with same type index) they have same value OR at least one of the values is UNDEFINED_VALUE.
        /// </summary>
        /// <param name="otherTypeIndex"></param>
        /// <param name="otherValueIndex"></param>
        /// <returns></returns>
        public bool Similar(int otherTypeIndex, int otherValueIndex)
        {
            if (typeIndex != otherTypeIndex) return false;
            if (ValueIndex != UNDEFINED_VALUE &&
                otherValueIndex != UNDEFINED_VALUE &&
                ValueIndex != otherValueIndex) return false;
            return true;
        }

        /// <summary>
        /// Two ConfigValues are Similar if (with same type index) they have same value OR at least one of the values is UNDEFINED_VALUE.
        /// </summary>
        /// <param name="otherTypeIndex"></param>
        /// <param name="otherValueIndex"></param>
        /// <returns></returns>
        public bool Similar(ConfigValue other)
        {
            return Similar(other.typeIndex, other.ValueIndex);
        }

        /// <summary>
        /// Two ConfigValues are Same if they have same type and same value.
        /// </summary>
        /// <param name="otherTypeIndex"></param>
        /// <param name="otherValueindex"></param>
        /// <returns></returns>
        public bool Same(int otherTypeIndex, int otherValueindex)
        {
            return typeIndex == otherTypeIndex && ValueIndex == otherValueindex;
        }

        /// <summary>
        /// Two ConfigValues are Same if they have same type and same value.
        /// </summary>
        /// <param name="otherTypeIndex"></param>
        /// <param name="otherValueindex"></param>
        /// <returns></returns>
        public bool Same(ConfigValue other)
        {
            return Same(other.typeIndex, other.ValueIndex);
        }
    }
}