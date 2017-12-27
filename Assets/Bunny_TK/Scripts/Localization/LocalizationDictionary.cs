using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bunny_TK.Localization
{
    [Serializable]
    public class LocalizationDictionary
    {
        public string name;

        [SerializeField]
        private List<LocalizationEntry> _entries;

        [SerializeField]
        private List<string> _langKeys;

        [SerializeField]
        private List<string> _wordKeys;

        #region Properties
        public List<string> LangKeys
        {
            get
            {
                return new List<string>(_langKeys = GetAllLanguageKeys());
            }
        }
        public List<string> WordKeys
        {
            get
            {
                return new List<string>(_wordKeys = GetAllWordKeys());
            }
        }
        public List<LocalizationEntry> Entries
        {
            get
            {
                return new List<LocalizationEntry>(_entries);
            }
        }
        #endregion Properties

        public LocalizationDictionary()
        {
            _entries = new List<LocalizationEntry>();
            _wordKeys = new List<string>();
            _langKeys = new List<string>();
        }
        public LocalizationDictionary(LocalizationDictionary localizationDictionary)
            : this()
        {
            LoadEntries(localizationDictionary.Entries);
        }

        #region LoadEntries
        public void LoadEntries(string langKey, string wordKey, string wordValue)
        {
            LoadEntries(new LocalizationEntry(langKey, wordKey, wordValue));
        }
        public void LoadEntries(LocalizationEntry entry)
        {
            if (!_entries.Contains(entry))
                _entries.Add(entry);

            if (!_langKeys.Contains(entry.languageKey))
                _langKeys.Add(entry.languageKey);

            if (!_wordKeys.Contains(entry.wordKey))
                _wordKeys.Add(entry.wordKey);
        }
        public void LoadEntries(IEnumerable<LocalizationEntry> entries)
        {
            foreach (var e in entries)
            {
                LoadEntries(e);
            }
        }
        public void LoadEntries(string langKey, Dictionary<string, string> dictionary)
        {
            foreach (var wordKey in dictionary.Keys)
            {
                LocalizationEntry current = new LocalizationEntry();

                current.languageKey = langKey;
                current.wordKey = wordKey;
                current.wordValue = dictionary[wordKey];

                LoadEntries(current);
            }
        }
        public void LoadEntries(Dictionary<string, Dictionary<string, string>> dictionary)
        {
            foreach (var l in dictionary.Keys)
                LoadEntries(l, dictionary[l]);
        }
        #endregion LoadEntries

        #region Contains
        public bool LangKeysContains(string langKey)
        {
            return _langKeys.Contains(langKey);
        }
        public bool WordKeysContains(string wordKey)
        {
            return _wordKeys.Contains(wordKey);
        }
        public bool EntriesContains(LocalizationEntry entry)
        {
            return _entries.Contains(entry);
        }
        #endregion Contains

        public void Clear()
        {
            _langKeys = new List<string>();
            _wordKeys = new List<string>();
            _entries = new List<LocalizationEntry>();
        }

        public Dictionary<string, Dictionary<string, string>> GetDictionary()
        {
            Dictionary<string, Dictionary<string, string>> resultDictionary = new Dictionary<string, Dictionary<string, string>>();
            foreach (var lKey in _langKeys)
            {
                resultDictionary.Add(lKey, new Dictionary<string, string>());
                foreach (var wKey in _wordKeys)
                {
                    if (!resultDictionary[lKey].ContainsKey(wKey))
                    {
                        string value = GetValue(lKey, wKey);
                        if (value == null) value = "";
                        resultDictionary[lKey].Add(wKey, value);
                    }
                }
            }
            return resultDictionary;
        }
        public string GetValue(string langKey, string wordKey)
        {
            var temp = GetEntry(langKey, wordKey);
            if (temp != null)
                return temp.wordValue;
            else
                return null;

        }

        public LocalizationEntry GetEntry(string langKey, string wordKey)
        {
            LocalizationEntry resultEntry = null;

            if (!LangKeysContains(langKey)) return resultEntry;
            if (!WordKeysContains(wordKey)) return resultEntry;
            var temp = _entries.Where(e => e.languageKey == langKey && e.wordKey == wordKey);
            if (temp.Count() != 1) return resultEntry;

            resultEntry = temp.First();

            return resultEntry;
        }

        public List<string> GetAllLanguageKeys()
        {
            HashSet<string> keys = new HashSet<string>();

            foreach (var e in _entries)
                keys.Add(e.languageKey);
            return keys.ToList();
        }

        public List<string> GetAllWordKeys()
        {
            HashSet<string> keys = new HashSet<string>();

            foreach (var e in _entries)
                keys.Add(e.wordKey);

            return keys.ToList();
        }

        public int RemoveAll(Predicate<LocalizationEntry> predicate)
        {
            int ret = -1;
            ret = _entries.RemoveAll(predicate);
            _wordKeys = GetAllWordKeys();
            _langKeys = GetAllLanguageKeys();
            return ret;
        }



    }
}