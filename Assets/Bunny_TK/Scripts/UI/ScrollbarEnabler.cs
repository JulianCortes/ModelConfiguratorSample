using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Classe che abilita/disabilita la scrollbar in base al contenuto
/// </summary>
public class ScrollbarEnabler : MonoBehaviour
{
    /// <summary>
    /// ScrollView che fa da contenitore.
    /// </summary>
    [SerializeField]
    RectTransform
        container;

    /// <summary>
    /// Panel le cui dimensioni variano in base al contenuto.
    /// </summary>
	[SerializeField]
    RectTransform
        content;

    /// <summary>
    /// La scrollbar da abilitare/disabilitare.
    /// </summary>
    [SerializeField]
    Scrollbar
        scrollbar;

    /// <summary>
    /// Specifica se la scrollbar è posta in orizzontale.
    /// </summary>
    [SerializeField]
    bool
        horizontal = false;

    private int prevChildCount = 0;

    private bool enableScrollbar = false;

    void Start()
    {
        prevChildCount = content.transform.childCount;
    }

    void Update()
    {
        OnContentSizeChanged();
        CheckDimensions();
        if (enableScrollbar != scrollbar.gameObject.activeSelf)
            scrollbar.gameObject.SetActive(enableScrollbar);
    }

    void CheckDimensions()
    {
        if (!horizontal)
        {
            enableScrollbar = container.rect.height < content.rect.height;//container.sizeDelta.y <= content.sizeDelta.y;
        }
        else
        {
            enableScrollbar = container.rect.width < content.rect.width;//container.sizeDelta.y <= content.sizeDelta.y;
        }
    }

    void OnContentSizeChanged()
    {
        if (prevChildCount != content.transform.childCount)
        {
            prevChildCount = content.transform.childCount;

            //Hack perchè la posizione del rect non viene aggiornata quando le dimensioni cambiano
            float temp = scrollbar.value;
            scrollbar.value = 0;
            Canvas.ForceUpdateCanvases();
            scrollbar.value = 1;
            Canvas.ForceUpdateCanvases();
            scrollbar.value = temp;
        }
    }
}