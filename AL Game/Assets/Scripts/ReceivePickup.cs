using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReceivePickup : MonoBehaviour
{
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private Transform pickupAnchor;

    [Header("Related to Pickup Function (leave blank if not applicable)")]
    [SerializeField] private Material[] emissiveMaterials;
    [SerializeField] private GameObject FX;
    [SerializeField] private ActivateExit activateExit;

    private bool pickupReceived = false;
    private List<Light> lights;
    private AudioSource audioSource;

    public int pickupIdx = 0;
    public int plinthIdx = 0;

    public bool GetPickupReceived()
    {
        return pickupReceived;
    }

    private void Awake()
    {
        lights = new List<Light>();
        audioSource = GetComponent<AudioSource>();
        //renderers = GetComponentsInChildren<Renderer>();
        //openDoor = FindFirstObjectByType<OpenDoor>();
    }

    private void Start()
    {
        foreach (Light light in FindObjectsByType<Light>(FindObjectsSortMode.None))
        {
            if (light.tag == "Lights")
            {
                lights.Add(light);
            }
        }
        foreach (Material mat in emissiveMaterials)
        {
            mat.DisableKeyword("_EMISSION");
        }
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
            ExecutePickupFunction(other.gameObject);
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
            TerminatePickupFunction(other.gameObject);
            other.GetComponent<Pickup>().pickupPlaced = false;
        }
    }

    private void ExecutePickupFunction(GameObject orb)
    {
        bool playAnimation = plinthIdx != 1;
        switch (this.pickupIdx)
        {
            case 0:
                StartCoroutine(LightsOn(true, true));
                break;
            case 1:
                StartCoroutine(LightsOn(false, playAnimation));
                break;
            case 2:
                StartCoroutine(LightsOn(false, playAnimation));
                break;
            case 3:
                StartCoroutine(LightsOn(false, playAnimation));
                break;
            default: break;
        }
        if (plinthIdx == 1) activateExit.ExitCondition(orb);
    }

    private void TerminatePickupFunction(GameObject orb)
    {
        switch (this.pickupIdx)
        {
            case 0:
                LightsOff(true);
                break;
            case 1:
                LightsOff(false);
                break;
            case 2:
                LightsOff(false);
                break;
            case 3:
                LightsOff(false);
                break;
            default: break;
        }
        if (plinthIdx == 1) activateExit.VoidExitCondition(orb);
    }


    private IEnumerator LightsOn(bool environmentLights, bool playAnimation)
    {
        float delayTime = 0.5f;
        //Lights On
        FX.SetActive(true);
        foreach (Material mat in emissiveMaterials)
        {
            mat.EnableKeyword("_EMISSION");
        }
        if (environmentLights)
        {
            foreach (Light light in lights)
            {
                light.enabled = true;
            }
        }
        audioSource.Play();
        if (playAnimation)
        {
            yield return new WaitForSeconds(delayTime);
            while (delayTime > 0f)
            {
                //Lights Off
                LightsOff(environmentLights);
                yield return new WaitForSeconds(delayTime);
                //Lights On
                FX.SetActive(true);
                foreach (Material mat in emissiveMaterials)
                {
                    mat.EnableKeyword("_EMISSION");
                }
                if (environmentLights)
                {
                    foreach (Light light in lights)
                    {
                        light.enabled = true;
                    }
                }
                audioSource.Play();
                yield return new WaitForSeconds(delayTime / 2f);
                if (delayTime > 0.1)
                {
                    delayTime -= 0.15f;
                }
                else
                {
                    delayTime -= 0.025f;
                }
                Debug.Log("Delay Time: " + delayTime);
            }
        }
        yield return null;
    }

    private void LightsOff(bool environmentLights)
    {
        //Lights Off
        FX.SetActive(false);
        foreach (Material mat in emissiveMaterials)
        {
            mat.DisableKeyword("_EMISSION");
        }
        if (environmentLights)
        {
            foreach (Light light in lights)
            {
                light.enabled = false;
            }
        }
        audioSource.Stop();
    }
}
