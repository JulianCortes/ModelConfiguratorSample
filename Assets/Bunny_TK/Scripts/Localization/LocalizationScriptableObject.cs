using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Bunny_TK.Localization
{
    [Serializable]
    [CreateAssetMenu(fileName = "Dictionary", menuName = "Utilities/Localization Dictionary")]
    public class LocalizationScriptableObject : ScriptableObject
    {
        public LocalizationDictionary dictionary;
    }

}
