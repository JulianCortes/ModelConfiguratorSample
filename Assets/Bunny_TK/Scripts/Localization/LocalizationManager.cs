using Bunny_TK.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bunny_TK.Localization
{
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        public string CurrentLanguage {  get; private set; }

        public Dictionary<string, Dictionary<string, string>> _dictionary;

        public event EventHandler OnChangeLanguage;

        [SerializeField]
        LocalizationScriptableObject serializedDictionary;

        [SerializeField]
        bool useSerializedDictionary = false;

        [Header("Editor Only")]
        [SerializeField]
        string targetLang;

        [SerializeField]
        bool updateLang = false;

        private void Start()
        {
            _dictionary = new Dictionary<string, Dictionary<string, string>>();
            if(useSerializedDictionary)
            {
                _dictionary = serializedDictionary.dictionary.GetDictionary();
            }
        }

        void Update()
        {
            if(updateLang)
            {
                ChangeLanguage(targetLang);
                updateLang = false;
            }
        }
        public void ChangeLanguage(string newLanguageKey)
        {
            if (_dictionary.ContainsKey(newLanguageKey))
            {
                CurrentLanguage = newLanguageKey;
                if (OnChangeLanguage != null)
                    OnChangeLanguage(this, System.EventArgs.Empty);
            }
        }

        public string GetValue(string key)
        {
            if (_dictionary == null) return null;
            if (_dictionary[CurrentLanguage].ContainsKey(key))
            {
                return _dictionary[CurrentLanguage][key];
            }
            else
            {
                Debug.LogError("Cannot find KEY: " + key + " in current language " + CurrentLanguage);
                return null;
            }
        }

        public void AddLanguage(string keyLanguage)
        {
            if (_dictionary == null) _dictionary = new Dictionary<string, Dictionary<string, string>>();

            if (!_dictionary.ContainsKey(keyLanguage)) _dictionary.Add(keyLanguage, new Dictionary<string, string>());
        }

        public void AddValue(string keyLanguage, string keyWord, string value)
        {
            AddLanguage(keyLanguage);

            if (!_dictionary[keyLanguage].ContainsKey(keyWord))
                _dictionary[keyLanguage].Add(keyWord, value);
            else
                Debug.LogError("Trying to add: " + keyWord + " | " + value + " @ " + keyLanguage + ". But already keyWord present");
        }
    }
}