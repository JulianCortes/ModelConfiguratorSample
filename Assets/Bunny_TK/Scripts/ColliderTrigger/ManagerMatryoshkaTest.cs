using Bunny_TK.Utils;
using UnityEngine;

public class ManagerMatryoshkaTest : MonoBehaviour
{
    [SerializeField]
    private MatryoshkaColliders matryoshka;

    private void Start()
    {
        matryoshka.TriggerEnter += Matryoshka_TriggerEnter;
        matryoshka.TriggerExit += Matryoshka_TriggerExit;
        matryoshka.TriggerStay += Matryoshka_TriggerStay;
    }

    private void Matryoshka_TriggerStay(object sender, ColliderTriggerEvent e)
    {
        Debug.Log("Stay: " + e.triggeredCollider.name);
    }

    private void Matryoshka_TriggerExit(object sender, ColliderTriggerEvent e)
    {
        Debug.Log("Exit: " + e.triggeredCollider.name);
    }

    private void Matryoshka_TriggerEnter(object sender, ColliderTriggerEvent e)
    {
        Debug.Log("Enter: " + e.triggeredCollider.name);
    }
}