using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Bunny_TK.Utils
{
    public class LoadImageTester : MonoBehaviour
    {
        [SerializeField]
        SpriteRenderer targetSprite;

        [SerializeField]
        bool test = false;

        [SerializeField]
        string filePath;

        void Update()
        {
            if(test)
            {
                targetSprite.sprite = LoadImageRuntime.Get_Sprite(filePath);
                test = false;
            }
        }

    }
}
