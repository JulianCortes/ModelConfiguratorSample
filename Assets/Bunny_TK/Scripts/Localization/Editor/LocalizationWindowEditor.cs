using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bunny_TK.Localization
{
    public class LocalizationWindowEditor : EditorWindow
    {
        private LocalizationScriptableObject targetScriptable;
        private List<string> wordKeys = new List<string>();
        private List<string> languageKeys = new List<string>();

        private List<LocalizationEntry> tempEntries;
        private LocalizationDictionary tempDictionary;

        private string newLanguage = "";
        private string newWordKey = "";
        private string newWordValue = "";

        private Vector2 scrollPos = new Vector2();

        private static bool loaded = false;

        public Color lightGrey = Color.grey;
        public Color grey = Color.blue;

        private LocalizationPopUp currentPopUpAdd;

        public static LocalizationWindowEditor Instance
        {
            get
            {
                return FindObjectOfType<LocalizationWindowEditor>();
            }
        }
        private string searchValue = "";

        private enum SearchType
        {
            WordKey,
            WordValue,
            All
        }
        private SearchType searchMask;
        [MenuItem("Bunny_TK/Localization/Localization Editor")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            LocalizationWindowEditor window = (LocalizationWindowEditor)EditorWindow.GetWindow(typeof(LocalizationWindowEditor));
            loaded = false;
            window.Show();
            window.titleContent = new GUIContent("Localization");
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            targetScriptable = (LocalizationScriptableObject)EditorGUILayout.ObjectField(targetScriptable, typeof(LocalizationScriptableObject), false);

            EditorGUILayout.BeginHorizontal(GUILayout.Height(25));
            {
                EditorGUI.BeginDisabledGroup(targetScriptable == null);
                if (GUILayout.Button(loaded ? "Reload" : "Load", GUILayout.Height(25)))
                    LoadDictionary(targetScriptable);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!loaded);
                if (GUILayout.Button("Save", GUILayout.Height(25)))
                {
                    tempDictionary.Clear();
                    tempDictionary.LoadEntries(tempEntries);
                    targetScriptable.dictionary = tempDictionary;
                    if (AssetDatabase.Contains(targetScriptable))
                    {
                        EditorUtility.SetDirty(targetScriptable);
                        AssetDatabase.SaveAssets();
                    }
                }

                if (GUILayout.Button("Languages", GUILayout.Height(25)))
                {

                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search by", GUILayout.Width(75));
            searchMask =(SearchType) EditorGUILayout.EnumPopup(searchMask,  GUILayout.Width(100));
            searchValue = EditorGUILayout.TextField(searchValue);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            if (loaded)
            {
                DisplayValues();

                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                {
                    //if (GUILayout.Button("Add Language"))
                    //{
                    //    if (!tempEntries.Exists(e => e.languageKey == newLanguage))
                    //    {
                    //        foreach (var w in wordKeys)
                    //        {
                    //            var temp = new LocalizationEntry();
                    //            temp.wordKey = w;
                    //            temp.languageKey = newLanguage;
                    //            tempEntries.Add(temp);
                    //        }

                    //        languageKeys.Add(newLanguage);
                    //    }
                    //}

                    if (GUILayout.Button("Add", GUILayout.Height(30)))
                    {
                        ShowAddPopup();
                    }
                }
                EditorGUILayout.EndHorizontal();

                //EditorGUILayout.BeginHorizontal();
                //{
                //    if (GUILayout.Button("Remove Language"))
                //    {
                //        tempEntries.RemoveAll(e => e.languageKey == newLanguage);
                //        languageKeys.Remove(newLanguage);
                //    }

                //    if (GUILayout.Button("Remove Word Key"))
                //    {
                //        tempEntries.RemoveAll(e => e.wordKey == newWordKey);
                //        wordKeys.Remove(newWordKey);
                //    }
                //}
                //EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
        }

        public void LoadDictionary(LocalizationScriptableObject newDictionary)
        {
            this.targetScriptable = newDictionary;
            tempDictionary = new LocalizationDictionary(targetScriptable.dictionary);
            wordKeys = tempDictionary.WordKeys;
            languageKeys = tempDictionary.LangKeys;
            //Copy
            tempEntries = new List<LocalizationEntry>(tempDictionary.Entries);
            loaded = true;
        }

        private void DisplayValues()
        {
            //Header
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("KEY", GUILayout.MinWidth(100), GUILayout.MaxWidth(200));

                foreach (var lang in languageKeys)
                {
                    EditorGUILayout.LabelField(lang, GUILayout.MinWidth(50), GUILayout.MaxWidth(500));
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //DisplayValues
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (string wk in wordKeys)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                if (GUILayout.Button("•", GUILayout.MinWidth(15), GUILayout.Height(23)))
                {
                    //Edit Entry
                    ShowEditPopup(wk);
                }

                EditorGUILayout.SelectableLabel(wk, GUILayout.Height(25));
                EditorGUILayout.EndHorizontal();

                foreach (string lk in languageKeys)
                {
                    var entry = tempEntries.Where(e => e.languageKey == lk && e.wordKey == wk).FirstOrDefault();
                    if (entry == null)
                    {
                        entry = new LocalizationEntry();
                        entry.wordKey = wk;
                        entry.languageKey = lk;
                    }
                    entry.wordValue = EditorGUILayout.TextField(entry.wordValue, GUILayout.MinWidth(50), GUILayout.MaxWidth(500), GUILayout.Height(25));
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
        }

        private void ShowEditPopup(string wk)
        {
            if (currentPopUpAdd == null)
                currentPopUpAdd = ScriptableObject.CreateInstance<LocalizationPopUp>();

            currentPopUpAdd.mainWindow = this;
            currentPopUpAdd.InitEditEntry(tempEntries.Where(e => e.wordKey == wk));
            currentPopUpAdd.ShowAuxWindow();
        }

        private void ShowAddPopup()
        {
            if (currentPopUpAdd == null)
                currentPopUpAdd = ScriptableObject.CreateInstance<LocalizationPopUp>();

            currentPopUpAdd.mainWindow = this;
            currentPopUpAdd.InitAddEntry(new List<string>(languageKeys));
            currentPopUpAdd.ShowAuxWindow();
        }

        public bool CheckWordKeyValidity(string wordKey)
        {
            if (wordKey == null) return false;
            if (wordKey.Trim() == string.Empty) return false;
            return !tempEntries.Exists(e => e.wordKey == wordKey);
        }

        public bool UpdateEntry(LocalizationEntry originalEntry, LocalizationEntry updatedEntry)
        {
            List<LocalizationEntry> targetEntries = tempEntries.Where(e => e.wordKey == originalEntry.wordKey && e.languageKey == originalEntry.languageKey).ToList();
            if (targetEntries.Count == 0) return false;

            for (int i = 0; i < targetEntries.Count; i++)
                targetEntries[i].Copy(updatedEntry);

            int index = wordKeys.IndexOf(originalEntry.wordKey);
            if (index >= 0)
                wordKeys[index] = updatedEntry.wordKey;

            return true;
        }

        public bool UpdateEntry(List<LocalizationEntry> originalEntries, List<LocalizationEntry> updatedEntries)
        {
            var oEntries = new List<LocalizationEntry>(originalEntries);
            var uEntries = new List<LocalizationEntry>(updatedEntries);

            if (oEntries.Count() != uEntries.Count()) return false;

            bool completeSuccess = true;
            for (int i = 0; i < oEntries.Count(); i++)
            {
                if (!UpdateEntry(oEntries[i], uEntries[i]))
                    completeSuccess = false;
            }

            return completeSuccess;
        }

        public void RemoveEntry(string targetWordkey)
        {
            tempEntries.RemoveAll(e => e.wordKey == targetWordkey);
            wordKeys.Remove(targetWordkey);
        }

        public bool AddEntry(LocalizationEntry newEntry)
        {
            if (tempEntries.Exists(e => e.wordKey == newEntry.wordKey && e.languageKey == newEntry.languageKey)) return false;

            tempEntries.Add(newEntry);
            if (!wordKeys.Contains(newEntry.wordKey))
                wordKeys.Add(newEntry.wordKey);
            if (!languageKeys.Contains(newEntry.languageKey))
                languageKeys.Add(newEntry.languageKey);

            return true;
        }
    }
}