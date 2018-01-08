using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Bunny_TK.ModelConfigurator
{
    [RequireComponent(typeof(ConfigurationIDBase))]
    public class ModelConfiguratorBase<T> : MonoBehaviour
                                   where T : ConfigurationIDBase
    {
        //This component manages all possible configurations appliable to the model.

        /// <summary>
        /// CurrentConfiguration will contain the results of ApplyConfiguration.
        /// </summary>
        public T currentConfiguration;
        /// <summary>
        /// References of all configurations.
        /// </summary>
        public List<Configuration> configurations;

        /// <summary>
        /// Event is sent when ApplyConfiguration is called.
        /// </summary>
        public Action OnAppliedConfiguration;

        private void Start()
        {
            ApplyConfiguration();
        }
        private void Reset()
        {
            currentConfiguration = GetComponent<ConfigurationIDBase>() as T;
        }

        /// <summary>
        /// Overlaps <paramref name="targetConfiguration"/> to current configurations and applies all configuration similar to the result.
        /// The overlap result is applied on CurrentConfiguration.
        /// </summary>
        /// <param name="targetConfiguration"></param>
        public virtual void ApplyConfiguration(T targetConfiguration)
        {
            configurations.ForEach(c => c.Remove());

            currentConfiguration.Overlap(targetConfiguration);
            configurations.Where(configuration => configuration.Id.Similar(currentConfiguration)).ToList()
                          .ForEach(similar => similar.ApplyConfiguration());

            if (OnAppliedConfiguration != null)
                OnAppliedConfiguration();
        }
        /// <summary>
        /// Overlaps the Id of <paramref name="targetConfiguration"/> to currentConfiguration and applies all configuration similar to the result.
        /// </summary>
        /// <param name="targetConfiguration"></param>
        public void ApplyConfiguration(Configuration targetConfiguration)
        {
            ApplyConfiguration(targetConfiguration.Id as T);
        }
        /// <summary>
        /// Applies CurrentConfiguration.
        /// </summary>
        public void ApplyConfiguration()
        {
            ApplyConfiguration(currentConfiguration);
        }
    }
}
