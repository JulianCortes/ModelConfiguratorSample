using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bunny_TK.ModelConfigurator
{
    /// <summary>
    /// Contains: all possible types, all possible values per type, restrictions/validity of a configuration.
    /// </summary>
    [CreateAssetMenu(fileName = "ModelConfiguratorDefinitions", menuName = "Utilities/ConfigurationDefinitions ")]
    public class ConfigurationDefinitions : ScriptableObject
    {
        //This class has a custom inspector to make it easy to use.
        //TODO: Make this ScriptableObject so that is unified across scenes.

        public string definitionName = "";

        /// <summary>
        /// Support class to make ConfigurationGeneric human-readable.
        /// </summary>
        [System.Serializable]
        public class ConfigTypeValues
        {
            public string type;
            public List<string> values = new List<string>();
        }

        /// <summary>
        /// Definition of types and possible values per type.
        /// </summary>
        public List<ConfigTypeValues> definitions = new List<ConfigTypeValues>();

        /// <summary>
        /// Definition of restrictions.
        /// es: type: A value 1 is not allowed with type: B value 0.
        /// </summary>
        public List<ConfigMaskMatch> restrictions = new List<ConfigMaskMatch>();

        public IEnumerable<string> GetAllTypes()
        {
            return definitions.Select(c => c.type);
        }

        public IEnumerable<string> GetAllValues(string type)
        {
            return definitions.Find(c => c.type == type).values;
        }

        public IEnumerable<string> GetAllValues(int indexType)
        {
            if (indexType >= definitions.Count) return new List<string> { "" };
            return definitions[indexType].values;
        }

        public string IndexToType(int index)
        {
            if (index >= definitions.Count) return "NULL";
            return definitions[index].type;
        }

        public string IndexToValue(int typeIndex, int valueIndex)
        {
            if (valueIndex == ConfigValue.UNDEFINED_VALUE) return "UNDEFINED";
            return definitions[typeIndex].values[valueIndex];
        }

        public int ValueToIndex(int typeIndex, string value)
        {
            if (value == "UNDEFINED") return ConfigValue.UNDEFINED_VALUE;
            return definitions[typeIndex].values.IndexOf(value);
        }


        public bool CheckRestriction(int typeIndex, int valueIndex, int otherTypeIndex, int otherValueIndex)
        {
            //Verifica se le due configurazioni si incontrano in una restrizione
            return restrictions.Exists(r => r.CheckMatch(typeIndex, valueIndex, otherTypeIndex, otherValueIndex));
        }

        public bool CheckRestriction(ConfigValue a, ConfigValue b)
        {
            return CheckRestriction(a.typeIndex, a.ValueIndex, b.typeIndex, b.ValueIndex);
        }

        public bool CheckValidity(List<ConfigValue> values)
        {
            if (definitions == null) return true;

            foreach (var pair in GetInvalidConfigs(values))
            {
                Debug.Log("Is invalid");
                return false;
            }
            return true;
        }

        public int GetFirstIndexRestrictionViolated(IEnumerable<ConfigValue> values)
        {
            int index = -1;
            foreach (var pair in GetPermutations(values, 2))
            {
                index = GetIndexRestrictionViolated(pair.First(), pair.Last());
                if (index >= 0) return index;
            }
            return index;
        }

        public int GetIndexRestrictionViolated(ConfigValue a, ConfigValue b)
        {
            return GetIndexRestrictionViolated(a.typeIndex, a.ValueIndex, b.typeIndex, b.ValueIndex);
        }

        public int GetIndexRestrictionViolated(int typeIndex, int valueIndex, int otherTypeIndex, int otherValueIndex)
        {
            return restrictions.FindIndex(r => r.CheckMatch(typeIndex, valueIndex, otherTypeIndex, otherValueIndex));
        }

        public ConfigMaskMatch GetConfigMask(ConfigValue a, ConfigValue b)
        {
            return restrictions.Find(r => r.CheckMatch(a, b));
        }

        public IEnumerable<ConfigValue> GetInvalidConfigs(IEnumerable<ConfigValue> configValues)
        {
            if (definitions == null) yield return null;

            foreach (var pairs in GetPermutations<ConfigValue>(configValues, 2))
                if (CheckRestriction(pairs.First(), pairs.Last()))
                {
                    yield return pairs.First();
                    yield return pairs.Last();
                }
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count)
        {
            int i = 0;
            foreach (var item in items)
            {
                if (count == 1)
                    yield return new T[] { item };
                else
                {
                    foreach (var result in GetPermutations(items.Skip(i + 1), count - 1))
                        yield return new T[] { item }.Concat(result);
                }
                ++i;
            }
        }

        /// <summary>
        /// Returns the overlap result of baseConfigs << overlapperConfigs.
        /// </summary>
        /// <param name="baseConfigs"></param>
        /// <param name="overlapperConfigs"></param>
        /// <returns></returns>
        public IEnumerable<ConfigValue> OverlapAndApplyRestrictions(List<ConfigValue> baseConfigs, List<ConfigValue> overlapperConfigs)
        {
            List<ConfigValue> result = new List<ConfigValue>();
            List<ConfigValue> aConfigs = new List<ConfigValue>(baseConfigs);
            List<ConfigValue> bConfigs = new List<ConfigValue>(overlapperConfigs);

            aConfigs = aConfigs.OrderBy(c => c.typeIndex).ToList();
            bConfigs = bConfigs.OrderBy(c => c.typeIndex).ToList();

            //Adding diff
            if (aConfigs.Count < bConfigs.Count)
                aConfigs.AddRange(bConfigs.GetRange(aConfigs.Count, bConfigs.Count - aConfigs.Count));
            else if (bConfigs.Count < aConfigs.Count)
                bConfigs.AddRange(aConfigs.GetRange(aConfigs.Count, aConfigs.Count - bConfigs.Count));

            for (int i = 0; i < aConfigs.Count; i++)
                result.Add(aConfigs[i].Overlap(bConfigs[i], true)); //<-- add with force type

            return ApplyRestrictions(result, bConfigs);
        }

        /// <summary>
        /// Returns a list of ConfigValues with restrictions applied.
        /// </summary>
        /// <param name="configs">ConfigValues to apply the restriction.</param>
        /// <param name="overlapperConfigs">ConfigValues that has been overlapped on <paramref name="configs"/></param>
        /// <returns></returns>
        public IEnumerable<ConfigValue> ApplyRestrictions(List<ConfigValue> configs, List<ConfigValue> overlapperConfigs)
        {
            List<ConfigValue> result = new List<ConfigValue>(configs);
            int lastIndex = 0;
            int indexCurrentConfig = 0;
            int changesCount = 0;
            //I need a HashSet of the restrictions applied.
            //If a restriction is applied more than once, it means that there is a sequence of restrictions that loops.
            //In that case the function will interrupt and will return an unsolvable invalid list of configValues, plus an error message.
            HashSet<int> violatedEncountered = new HashSet<int>();
            while (lastIndex >= 0 && indexCurrentConfig < result.Count)
            {
                lastIndex = GetFirstIndexRestrictionViolated(result);
                if (lastIndex >= 0)
                {
                    if (violatedEncountered.Add(lastIndex))
                    {
                        //Assuming that the overlapper and base are valid configurations,
                        //in case of violation between two configValues,
                        //the config that will "fallback" is the config that remains immutated after the overlap action.
                        //The configurations that are changing are in the overlapperConfigs.

                        //Check if current config (result[indexCurrentConfig]) is changing,
                        //if not, currentConfig will "fallback"
                        //else the other config will "fallback"
                        int tempIndex = overlapperConfigs.FindIndex(c => c.Same(result[indexCurrentConfig]));
                        if (tempIndex < 0)
                        {
                            result[indexCurrentConfig].ValueIndex = restrictions[lastIndex].GetFallbackindexValue(result[indexCurrentConfig].typeIndex);
                        }
                        else
                        {
                            //Get the other opposite indexType in the mask
                            int indexTypeToChange = restrictions[lastIndex].indexType == result[indexCurrentConfig].typeIndex ?
                                                    restrictions[lastIndex].otherIndexType :
                                                    restrictions[lastIndex].indexType;

                            result[indexTypeToChange].ValueIndex = restrictions[lastIndex].GetFallbackindexValue(indexTypeToChange);
                        }

                        changesCount++;
                        indexCurrentConfig = 0;
                    }
                    else
                    {
                        string loopError = "Restriction loop!\nList of restrictions applied: ";
                        foreach (var i in violatedEncountered)
                            loopError += i + " ";
                        loopError += ". Applying restriction #" + lastIndex + " again.";
                        Debug.LogError(loopError);
                        return result;
                    }
                }
                else
                    indexCurrentConfig++;

                //Just to get sure we're not looping..
                if (restrictions.Count > 0 && changesCount >= GetAllTypes().Count() * restrictions.Count())
                {
                    Debug.LogError("Possible restriction loop!");
                    return result;
                }
            }
            return result;
        }
    }
}