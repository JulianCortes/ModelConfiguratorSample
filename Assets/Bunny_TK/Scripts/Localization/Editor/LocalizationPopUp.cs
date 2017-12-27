using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bunny_TK.Localization
{
    public class LocalizationPopUp : EditorWindow
    {
        public LocalizationWindowEditor mainWindow;
        private List<LocalizationEntry> originalEntries;
        private List<LocalizationEntry> entries;

        private string originalWordKey;
        private string wordKey;
        private List<string> originalLangKeys;
        private List<string> langKeys;

        private enum PopUpType
        {
            None,
            Add,
            Edit
        }

        private PopUpType _currentType;

        private Vector2 langScroll = new Vector2(0, 0);

        public void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //EditorGUILayout.SelectableLabel(""+this.position.height);
            //EditorGUILayout.SelectableLabel(""+this.position.width);

            switch (_currentType)
            {
                case PopUpType.None:
                    break;

                case PopUpType.Add:
                    {
                        //Key Label
                        EditorGUILayout.BeginHorizontal();
                        {
                            //EditorGUILayout.LabelField("KEY", GUILayout.MinWidth(50), GUILayout.MaxWidth(75));
                            GUI.SetNextControlName("KEY");
                            wordKey = EditorGUILayout.TextField("KEY", wordKey);
                            entries.ForEach(e => e.wordKey = wordKey);
                        }
                        EditorGUILayout.EndHorizontal();
                        //if (originalWordKey != wordKey)
                        {
                            if (!mainWindow.CheckWordKeyValidity(wordKey))
                                EditorGUILayout.HelpBox("Invalid key", MessageType.Error);
                        }
                        EditorGUILayout.Separator();

                        //Langs
                        langScroll = EditorGUILayout.BeginScrollView(langScroll);
                        foreach (var lk in originalLangKeys)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                LocalizationEntry current = entries.Find(e => e.wordKey == wordKey && e.languageKey == lk);
                                EditorGUILayout.PrefixLabel(lk);
                                current.wordValue = EditorGUILayout.TextArea(current.wordValue, GUILayout.Height(50));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndScrollView();
                        if (GUILayout.Button("Add"))
                        {
                            if (!mainWindow.CheckWordKeyValidity(wordKey))
                            {
                                EditorGUILayout.HelpBox("Invalid key", MessageType.Error);
                            }
                            else
                            {
                                foreach (var e in entries)
                                    mainWindow.AddEntry(e);
                                Close();
                            }
                        }

                        break;
                    }

                case PopUpType.Edit:
                    {
                        //Key Label
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUI.SetNextControlName("KEY");
                            wordKey = EditorGUILayout.TextField("KEY", wordKey);
                            entries.ForEach(e => e.wordKey = wordKey);
                        }
                        EditorGUILayout.EndHorizontal();
                        if (originalWordKey != wordKey)
                        {
                            if (!mainWindow.CheckWordKeyValidity(wordKey))
                                EditorGUILayout.HelpBox("Invalid key", MessageType.Error);
                        }
                        EditorGUILayout.Separator();
                        //Langs
                        langScroll = EditorGUILayout.BeginScrollView(langScroll);
                        foreach (var lk in originalLangKeys)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                LocalizationEntry current = entries.Find(e => e.wordKey == wordKey && e.languageKey == lk);
                                EditorGUILayout.PrefixLabel(lk);
                                current.wordValue = EditorGUILayout.TextArea(current.wordValue, GUILayout.Height(50));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndScrollView();

                        if (GUILayout.Button("Update"))
                        {
                            if (originalWordKey != wordKey)
                            {
                                if (!mainWindow.CheckWordKeyValidity(wordKey))
                                {
                                    ShowNotification(new GUIContent("Key already present!"));
                                }
                                else
                                {
                                    Close();
                                    mainWindow.UpdateEntry(originalEntries, entries);
                                }
                            }
                            else
                            {
                                Close();
                                mainWindow.UpdateEntry(originalEntries, entries);
                            }
                        }

                        if(GUILayout.Button("Remove"))
                        {
                            mainWindow.RemoveEntry(originalWordKey);
                            ShowNotification(new GUIContent("Key Removed!"));
                            Close();
                        }

                        break;
                    }
                default:
                    break;
            }

            if (GUILayout.Button("Close"))
            {
                this.Close();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        public void InitEditEntry(IEnumerable<LocalizationEntry> targetEntries)
        {
            _currentType = PopUpType.Edit;
            originalEntries = new List<LocalizationEntry>();
            entries = new List<LocalizationEntry>();

            foreach (var e in targetEntries)
            {
                originalEntries.Add(new LocalizationEntry(e));
                entries.Add(new LocalizationEntry(e));
            }

            Init();
        }

        public void InitAddEntry(List<string> langKeys)
        {
            _currentType = PopUpType.Add;
            originalEntries = new List<LocalizationEntry>();

            for (int i = 0; i < langKeys.Count; i++)
                originalEntries.Add(new LocalizationEntry(langKeys[i], "newKey", ""));
            entries = new List<LocalizationEntry>(originalEntries);

            Init();
        }

        private void Init()
        {
            this.titleContent = new GUIContent("Localization");
            originalLangKeys = new List<string>();

            if (originalEntries == null) return;
            if (originalEntries.Count == 0) return;

            wordKey = originalWordKey = originalEntries[0].wordKey;
            originalLangKeys.Add(originalEntries[0].languageKey);

            if (originalEntries.Count == 1) return;
            if (originalEntries.Exists(e => e.wordKey != originalWordKey)) return;

            originalLangKeys = originalEntries.Select(e => e.languageKey).Distinct().ToList();
        }
    }
}