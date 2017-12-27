using System.Collections.Generic;
using UnityEngine;
namespace Bunny_TK.Localization
{
    [RequireComponent(typeof(AudioSource))]
    public class LocalizationAudio : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;

        public List<KeyAudio> audioDictionary;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            LocalizationManager.Instance.OnChangeLanguage += LocalizationManager_OnChangeLanguage;
        }

        private void OnDisable()
        {
            LocalizationManager.Instance.OnChangeLanguage -= LocalizationManager_OnChangeLanguage;
        }

        private void LocalizationManager_OnChangeLanguage(object sender, System.EventArgs e)
        {
            UpdateAudio(LocalizationManager.Instance.CurrentLanguage);
        }

        public void UpdateAudio(string keyLanguage)
        {
            var clip = audioDictionary.Find(a => a.keyLanguage == keyLanguage).audioSource;
            if (clip != null)
                _audioSource.clip = clip;
            else
                Debug.LogError("Key Language: " + keyLanguage + " Not found!");
        }

        [System.Serializable]
        public struct KeyAudio
        {
            public string keyLanguage;
            public AudioClip audioSource;
        }
    }
}