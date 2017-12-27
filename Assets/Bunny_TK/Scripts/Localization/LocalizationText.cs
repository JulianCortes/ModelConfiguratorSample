using UnityEngine;
using UnityEngine.UI;

namespace Bunny_TK.Localization
{
    [RequireComponent(typeof(Text))]
    public class LocalizationText : MonoBehaviour
    {
        [SerializeField]
        private string _id;

        [SerializeField]
        private Text _text;

        public string Id { get { return _id; } }

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        private void OnEnable()
        {
            LocalizationManager.Instance.OnChangeLanguage += LocalizationManager_OnChangeLanguage;
            UpdateText();
        }

        private void OnDisable()
        {
            LocalizationManager.Instance.OnChangeLanguage -= LocalizationManager_OnChangeLanguage;
        }

        private void UpdateText()
        {
            string value = LocalizationManager.Instance.GetValue(_id);
            if (value != null)
                _text.text = value;
        }

        private void LocalizationManager_OnChangeLanguage(object sender, System.EventArgs e)
        {
            UpdateText();
        }
    }
}