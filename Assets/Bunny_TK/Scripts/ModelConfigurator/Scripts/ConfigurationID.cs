using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bunny_TK.ModelConfigurator
{
    /// <summary>
    /// Contains the definitions of a configuration.
    /// Note: Requires a ConfigurationDefinitions to check it's validity and apply restrictions on overlaps.
    /// </summary>
    public class ConfigurationID : ConfigurationIDBase
    {
        //This class has a custom inspector to make it easy to read and use, but this requires a definition.
        [HideInInspector]
        public ConfigurationDefinitions definition;

        /// <summary>
        /// Note: configValues uses indexes.
        /// </summary>
        [SerializeField]
        public List<ConfigValue> configValues = new List<ConfigValue>();

        private void Reset()
        {
            //Find the first ConfigurationDefinition in scene
            configValues = new List<ConfigValue>();
            definition = Resources.LoadAll<ConfigurationDefinitions>("").FirstOrDefault();
        }

        /// <summary>
        /// Returns TRUE if all ConfigValues are similar.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Similar(ConfigurationID other)
        {
            return Similar(other.configValues);
        }

        /// <summary>
        /// Returns TRUE if all ConfigValues are similar.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Similar(List<ConfigValue> values)
        {
            if (values.Count != configValues.Count) return false;
            if (configValues.Where(c => !values.Exists(c2 => c2.Similar(c))).Count() > 0) return false;
            return true;
        }

        /// <summary>
        /// Returns TRUE if all ConfigValues are similar.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Similar(ConfigurationIDBase other)
        {
            if (other == null) return false;
            if (other.GetType() != typeof(ConfigurationID)) return false;
            if (other == this) return true;
            return Similar(other as ConfigurationID);
        }

        // OVERLAP SAMPLE
        // RESULT CONFIG      BASE CONFIG        OVERLAPPER CONFIG
        //   A2        =        A1         <-     A2
        //   B2        =        UNDEF      <-     B2
        //   C1        =        C1         <-     UNDEF
        //   UNDEF     =        UNDEF      <-     UNDEF
        /// <summary>
        /// This configValues will be the overlap of <paramref name="baseConfig"/>'s configValues with <paramref name="overlapperConfig"/>'s configValues.
        /// Note: if defintion is not NULL restrictions will be applied.
        /// </summary>
        /// <param name="baseConfig"></param>
        /// <param name="overlapperConfig"></param>
        public void Overlap(ConfigurationID baseConfig, ConfigurationID overlapperConfig)
        {
            List<ConfigValue> result = new List<ConfigValue>();
            List<ConfigValue> aConfigs = new List<ConfigValue>(baseConfig.configValues);
            List<ConfigValue> bConfigs = new List<ConfigValue>(overlapperConfig.configValues);

            aConfigs = aConfigs.OrderBy(c => c.typeIndex).ToList();
            bConfigs = bConfigs.OrderBy(c => c.typeIndex).ToList();

            //Make them the same length
            if (aConfigs.Count < bConfigs.Count)
                aConfigs.AddRange(bConfigs.GetRange(aConfigs.Count, bConfigs.Count - aConfigs.Count));
            else if (bConfigs.Count < aConfigs.Count)
                bConfigs.AddRange(aConfigs.GetRange(aConfigs.Count, aConfigs.Count - bConfigs.Count));

            for (int i = 0; i < aConfigs.Count; i++)
                result.Add(aConfigs[i].Overlap(bConfigs[i], true)); //<-- add with force type

            if (definition != null)
                result = definition.ApplyRestrictions(result, bConfigs).ToList();

            configValues = result;
        }

        /// <summary>
        /// Overlaps this configValues with other.configValues.
        /// </summary>
        /// <param name="other"></param>
        public void Overlap(ConfigurationID other)
        {
            Overlap(this, other);
        }

        /// <summary>
        /// Overlaps this configValues with other.configValues.
        /// </summary>
        /// <param name="other"></param>
        public override void Overlap(ConfigurationIDBase other)
        {
            Overlap(other as ConfigurationID);
        }

        /// <summary>
        /// Returns TRUE if all configValues doesn't violate any restriction in this.definition.
        /// </summary>
        /// <returns></returns>
        public bool CheckValidity()
        {
            if (definition == null) return true;
            return definition.CheckValidity(configValues);
        }

        public int GetFirstIndexRestrictionViolated()
        {
            if (definition == null) return -1;
            return definition.GetFirstIndexRestrictionViolated(configValues);
        }

        public IEnumerable<ConfigValue> GetInvalidConfigs()
        {
            if (definition == null) return new ConfigValue[] { };
            return definition.GetInvalidConfigs(configValues);
        }

        /// <summary>
        /// Make this configuration complaint to <paramref name="definitions"/>.
        /// </summary>
        /// <param name="definitions"></param>
        public void UpdateConfigs(ConfigurationDefinitions definitions)
        {
            this.definition = definitions;
            if (definitions == null || definitions.definitions == null || definitions.definitions.Count() == 0)
            {
                //if no definition is available
                configValues.Clear();
                return;
            }

            //Rimozione configValues di troppo
            int diff = configValues.Count - definition.GetAllTypes().Count();
            if (diff > 0) configValues.RemoveRange(definition.GetAllTypes().Count() - 1, diff);

            //Aggiunta di nuovi configValue se necessario
            //Clamp dei ValueIndex dei configValue
            int i = 0;
            foreach (var type in definition.GetAllTypes())
            {
                if (i >= configValues.Count)
                    configValues.Add(new ConfigValue());

                configValues[i].typeIndex = i;
                configValues[i].ValueIndex = Mathf.Clamp(configValues[i].ValueIndex, 0, definition.GetAllValues(i).Count());
                i++;
            }
        }

        public override bool Same(ConfigurationIDBase other)
        {
            if (!Similar(other)) return false;

            ConfigurationID otherID = other as ConfigurationID;
            if (configValues.Count != otherID.configValues.Count) return false;

            if (configValues.Exists(v => !otherID.configValues.Exists(v2 => v2.typeIndex != v.typeIndex))) return false;

            if (configValues.Exists(v =>
                otherID.configValues.Exists(v2 =>
                v2.typeIndex == v.typeIndex && v2.ValueIndex != v.ValueIndex)))
                return false;

            return true;
        }

        public override IEnumerable<string> GetAllTypes()
        {
            if (definition != null)
                return definition.GetAllTypes();
            else
            {
                Debug.LogWarning("Definition is null, returning indexes");
                return configValues.Select(c => c.typeIndex.ToString());
            }
        }

        public override IEnumerable<string> GetAllValues(int typeIndex)
        {
            if (definition != null)
                return definition.GetAllValues(typeIndex);
            else
            {
                Debug.LogError("Definition is null, cannot return possible values");
                return new string[] { };
            }
        }
    }
}