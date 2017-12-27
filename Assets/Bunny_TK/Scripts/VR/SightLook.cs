using Bunny_TK.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SightLook : Singleton<SightLook>
{
    [SerializeField]
    private Image sightImage;

    [SerializeField]
    private GameObject currentLookingObject;

    [SerializeField]
    private float currentFillTime = 0f;

    [SerializeField]
    private float currentFillLenght = 1.5f;

    public float FillValue
    {
        get { return sightImage.fillAmount; }
        set { sightImage.fillAmount = value; }
    }

    public void SetCurrentLookingObject(GameObject go, float fillLenght = 1.5f)
    {
        currentLookingObject = go;
        currentFillLenght = fillLenght;

        if (go == null)
        {
            currentFillTime = 0f;
            FillValue = 0f;
        }
    }

    private void Update()
    {
        if (currentLookingObject != null)
        {
            currentFillTime += Time.deltaTime;
            FillValue = currentFillTime / currentFillLenght;
        }
        else
        {
            currentFillTime = 0f;
            FillValue = 0f;
        }
    }

    private void Test()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }
}