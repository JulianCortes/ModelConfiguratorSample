using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Bunny_TK.Utils
{
    public class LoadImageRuntime : MonoBehaviour
    {
        public GameObject target;
        public string filePath;
        public bool loadOnStart = false;

        [SerializeField]
        private bool isLoaded = false;

        [SerializeField]
        private bool testLoad = false;

        public bool IsLoaded { get { return isLoaded; } }

        private void Start()
        {
            LoadTextureOnTarget();
        }

        private void Update()
        {
            if (testLoad)
            {
                LoadTextureOnTarget();
                testLoad = false;
            }
        }

        public void LoadTextureOnTarget()
        {
            if (target == null || string.IsNullOrEmpty(filePath)) return;
            if (!File.Exists(filePath)) return;

            _LoadTextureOnTarget(target, filePath);
            isLoaded = true;
        }

        public void LoadTextureOnTarget(string filePath)
        {
            this.filePath = filePath;
            LoadTextureOnTarget();
        }

        private void _LoadTextureOnTarget(GameObject target, string filePath)
        {
            Image image = target.GetComponent<Image>();
            SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
            MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();

            if (image != null)
                image.sprite = Get_Sprite(filePath);

            if (spriteRenderer != null)
                spriteRenderer.sprite = Get_Sprite(filePath);

            if (meshRenderer != null)
                meshRenderer.material.mainTexture = Get_Texture2D(filePath);
        }

        public static Texture2D Get_Texture2D(string filePath)
        {
            var texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(filePath)); //The image will be at original size.
            texture.wrapMode = TextureWrapMode.Clamp;       //WORKAROUND to remove the white border.
            return texture;
        }

        public static Sprite Get_Sprite(string filePath)
        {
            var texture = Get_Texture2D(filePath);
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100);
        }
    }
}