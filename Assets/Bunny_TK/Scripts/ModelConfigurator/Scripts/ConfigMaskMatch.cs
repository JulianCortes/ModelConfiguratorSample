namespace Bunny_TK.ModelConfigurator
{
    /// <summary>
    /// Defines a restriction between one or more value of a type with another one.
    /// </summary>
    [System.Serializable]
    public class ConfigMaskMatch
    {
        //Questa classe interseca uno o più valori di due tipi.
        //Utilizzato per definire restrizioni sulle configurazioni
        // es. Config A[tipo = 1, valore = 2] non è possibile con Config B[tipo = 2, valori = 0;2]
        //Inoltre definisce che valore si deve assumere nel caso di una restrizione: fallbackValue (Vedi Overlap in ConfigurationDefinition).
        //In particolare usa le operazioni di BitMask.

        /// <summary>
        /// indice del tipo
        /// </summary>
        public int indexType;
        /// <summary>
        /// Maschera di valori influenzati dalla restrizione.
        /// Vedi BitMask.
        /// </summary>
        public int maskIndex;
        /// <summary>
        /// Valore di fallback.
        /// </summary>
        public int fallbackIndexValue;

        public int otherIndexType;
        public int otherMaskIndex;
        public int otherFallbackIndexValue;

        public bool ValueIndexIsInMask(int valueIndex, int mask)
        {
            return (mask & (1 << valueIndex)) != 0;
        }

        /// <summary>
        /// Checks if the ConfigValues are in this match.
        /// </summary>
        /// <param name="typeIndex_a"></param>
        /// <param name="valueIndex_a"></param>
        /// <param name="typeIndex_b"></param>
        /// <param name="valueIndex_b"></param>
        /// <returns></returns>
        public bool CheckMatch(int typeIndex_a, int valueIndex_a, int typeIndex_b, int valueIndex_b)
        {
            if (typeIndex_a == indexType && typeIndex_b == otherIndexType)
            {
                if (!ValueIndexIsInMask(valueIndex_a, maskIndex)) return false;
                if (!ValueIndexIsInMask(valueIndex_b, otherMaskIndex)) return false;
                return true;
            }
            else if (typeIndex_a == otherIndexType && typeIndex_b == indexType)
            {
                if (!ValueIndexIsInMask(valueIndex_b, maskIndex)) return false;
                if (!ValueIndexIsInMask(valueIndex_a, otherMaskIndex)) return false;
                return true;
            }
            else
                return false;
        }

        public bool CheckMatch(ConfigValue a, ConfigValue b)
        {
            return CheckMatch(a.typeIndex, a.ValueIndex, b.typeIndex, b.ValueIndex);
        }

        public int GetFallbackindexValue(int indexType)
        {
            if (indexType == this.indexType) return fallbackIndexValue;
            if (indexType == otherIndexType) return otherFallbackIndexValue;
            return -1;
        }
    }
}