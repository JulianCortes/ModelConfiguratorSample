using Bunny_TK.Utils;
using UnityEngine;

public class ManagerColliderTest : MonoBehaviour
{
    [SerializeField]
    private CompoundColliderTrigger mulitpleCollider;

    private void Start()
    {
        mulitpleCollider.TriggerEnter += MulitpleCollider_TriggerEnter;
        mulitpleCollider.TriggerExit += MulitpleCollider_TriggerExit;
        mulitpleCollider.TriggerStay += MulitpleCollider_TriggerStay;
    }

    private void MulitpleCollider_TriggerStay(object sender, ColliderTriggerEvent e)
    {
        Debug.Log("Stay: " + e.triggeredCollider.name);
    }

    private void MulitpleCollider_TriggerExit(object sender, ColliderTriggerEvent e)
    {
        Debug.Log("Exit: " + e.triggeredCollider.name);
    }

    private void MulitpleCollider_TriggerEnter(object sender, ColliderTriggerEvent e)
    {
        Debug.Log("Enter: " + e.triggeredCollider.name);
    }
}