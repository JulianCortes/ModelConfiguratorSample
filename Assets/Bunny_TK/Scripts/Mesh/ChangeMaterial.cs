using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ChangeMaterial : MonoBehaviour
{
    [SerializeField]
    List<Material> originalMaterials;

    [SerializeField]
    List<ChangeMaterial> materialChangers;

    [SerializeField]
    MeshRenderer _mRenderer;

    public bool searchInChildren = false;

    void Start()
    {
        _mRenderer = GetComponent<MeshRenderer>();

        if (_mRenderer != null)
        {
            originalMaterials = new List<Material>(_mRenderer.materials);
        }
        else
        {
            if(searchInChildren)
            {
                AutoAttachComponent();
            }
        }
    }


    private void AutoAttachComponent()
    {
        List<MeshRenderer> temp = GetComponentsInChildren<MeshRenderer>().ToList();

        ChangeMaterial cm = null;
        materialChangers = new List<ChangeMaterial>();
        for (int i = 0; i < temp.Count; i++)
        {
            cm = temp[i].gameObject.GetComponent<ChangeMaterial>();
            if (cm == null)
                cm = temp[i].gameObject.AddComponent<ChangeMaterial>();

            materialChangers.Add(cm);
        }
    }

    public void SwapMaterials(Material newMaterial)
    {

        if (_mRenderer != null)
        {
            Material[] temp = new Material[originalMaterials.Count];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = newMaterial;
            _mRenderer.materials = temp;
        }
        else
        {
            materialChangers.ForEach(m => m.SwapMaterials(newMaterial));
        }
    }

    public void ResetMaterials()
    {

        if (_mRenderer != null)
        {
            List<Material> temp = new List<Material>(originalMaterials);
            _mRenderer.materials = temp.ToArray();
        }
        else
        {
            materialChangers.ForEach(m => m.ResetMaterials());

        }
    }
}
