using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceivePickup : MonoBehaviour
{
    [SerializeField] private LayerMask pickupLayer;

    private Renderer[] renderers;
    private bool pickupReceived = false;
    private OpenDoor openDoor;

    public int pickupIdx = 0;

    public bool GetPickupReceived()
    {
        return pickupReceived;
    }

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        openDoor = FindFirstObjectByType<OpenDoor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Contert layer to bitmask
        int layerValue = 1 << other.gameObject.layer;
        // Use bitwise & operator to compare bits in layerValue and pickupLayer. If layerValue is contained within pickupLayer then only the layerValue bits will remain.
        if ((layerValue & pickupLayer) == layerValue && other.GetComponent<Pickup>().pickupIdx == pickupIdx)
        {
            Debug.Log("Pickup detected");
            other.attachedRigidbody.useGravity = false;
            other.attachedRigidbody.isKinematic = true;
            //other.transform.SetParent(null);
            other.transform.SetParent(this.transform);
            other.transform.localPosition = Vector3.zero;
            other.transform.localRotation = Quaternion.identity;
            other.gameObject.layer = LayerMask.NameToLayer("Default");
            // Iluminate
            other.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            foreach (Renderer r in renderers)
            {
                r.material.EnableKeyword("_EMISSION");
            }
            pickupReceived = true;
            openDoor.CheckPickups();
        }
    }
}
