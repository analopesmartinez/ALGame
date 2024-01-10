using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceivePickup : MonoBehaviour
{
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private Transform pickupAnchor;

    [Header("Related to Pickup Function (leave blank if not applicable)")]
    [SerializeField] private Material[] emissiveMaterials;

    private Renderer[] renderers;
    private bool pickupReceived = false;
    private OpenDoor openDoor;
    private Light[] lights;

    public int pickupIdx = 0;
    private bool objectContained;

    public bool GetPickupReceived()
    {
        return pickupReceived;
    }

    private void Awake()
    {
        lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        //renderers = GetComponentsInChildren<Renderer>();
        //openDoor = FindFirstObjectByType<OpenDoor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Contert layer to bitmask
        int layerValue = 1 << other.gameObject.layer;
        // Use bitwise & operator to compare bits in layerValue and pickupLayer. If layerValue is contained within pickupLayer then only the layerValue bits will remain.
        if ((layerValue & pickupLayer) == layerValue && other.GetComponent<Pickup>().pickupIdx == pickupIdx && !pickupReceived)
        {
            Debug.Log("Pickup detected");
            other.attachedRigidbody.useGravity = false;
            other.attachedRigidbody.isKinematic = true;
            other.transform.SetParent(null);
            //other.transform.SetParent(this.transform);
            other.transform.position = pickupAnchor.position;
            other.transform.rotation = pickupAnchor.rotation;
            
            //other.gameObject.layer = LayerMask.NameToLayer("Default");
            // Iluminate
            /*other.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            foreach (Renderer r in renderers)
            {
                r.material.EnableKeyword("_EMISSION");
            }*/
            ExecutePickupFunction();
            other.GetComponent<Pickup>().pickupPlaced = true;
            pickupReceived = true;
            //openDoor.CheckPickups();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Pickup Dropped: " + other.name);
        // Contert layer to bitmask
        int layerValue = 1 << other.gameObject.layer;
        // Use bitwise & operator to compare bits in layerValue and pickupLayer. If layerValue is contained within pickupLayer then only the layerValue bits will remain.
        if ((layerValue & pickupLayer) == layerValue && other.GetComponent<Pickup>().pickupIdx == pickupIdx)
        {
            pickupReceived = false;
            TerminatePickupFunction();
            other.GetComponent<Pickup>().pickupPlaced = false;
        }
    }

    private void ExecutePickupFunction()
    {
        switch (this.pickupIdx)
        {
            case 0:
                StartCoroutine(LightsOn());
                break;
            default: break;
        }
    }

    private void TerminatePickupFunction()
    {
        switch (this.pickupIdx)
        {
            case 0:
                LightsOff();
                break;
            default: break;
        }
    }


    private IEnumerator LightsOn()
    {
        float delayTime = 1f;
        while (delayTime > 0f)
        {
            //Lights Off
            foreach (Material mat in emissiveMaterials)
            {
                mat.DisableKeyword("_EMISSION");
            }
            foreach (Light light in lights)
            {
                light.enabled = false;
            }
            yield return new WaitForSeconds(delayTime);
            //Lights On
            foreach (Material mat in emissiveMaterials)
            {
                mat.EnableKeyword("_EMISSION");
            }
            foreach (Light light in lights)
            {
                light.enabled = true;
            }
            yield return new WaitForSeconds(delayTime / 2f);
            delayTime -= 0.25f;
            Debug.Log("Delay Time: " + delayTime);
        }
    }

    private void LightsOff()
    {
        //Lights Off
        foreach (Material mat in emissiveMaterials)
        {
            mat.DisableKeyword("_EMISSION");
        }
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
    }
}
