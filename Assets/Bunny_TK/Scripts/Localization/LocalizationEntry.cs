using System;
using System.Collections.Generic;

namespace Bunny_TK.Localization
{
    [Serializable]
    public class LocalizationEntry : IEqualityComparer<LocalizationEntry>
    {
        public string languageKey;
        public string wordKey;
        public string wordValue;

        public LocalizationEntry() : base()
        {
        }

        public LocalizationEntry(LocalizationEntry e) :
            this(e.languageKey, e.wordKey, e.wordValue)
        {
        }

        public LocalizationEntry(string languageKey, string wordKey, string wordValue)
        {
            this.languageKey = languageKey;
            this.wordKey = wordKey;
            this.wordValue = wordValue;
        }

        public bool Equals(LocalizationEntry x, LocalizationEntry y)
        {
            if (x.wordKey != y.wordKey) return false;
            if (x.wordValue != y.wordValue) return false;
            if (x.languageKey != y.languageKey) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(LocalizationEntry)) return false;
            return Equals(((LocalizationEntry)obj), this);
        }

        public int GetHashCode(LocalizationEntry obj)
        {
            return wordKey.GetHashCode() + languageKey.GetHashCode() + wordValue.GetHashCode() * 17;
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public override string ToString()
        {
            return "Entry[" + languageKey + "][" + wordKey + "] " + wordValue;
        }

        public void Copy(LocalizationEntry targetEntry)
        {
            languageKey = targetEntry.languageKey;
            wordKey = targetEntry.wordKey;
            wordValue = targetEntry.wordValue;

        }
    }
}