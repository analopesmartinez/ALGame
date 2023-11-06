using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] private float openSpeed = 0.1f;

    private ReceivePickup[] receivePickups;
    private bool allPickupsReceived = false;
    private bool isOpen = false;

    private void Awake()
    {
        receivePickups = FindObjectsOfType<ReceivePickup>();
    }

    private void Update()
    {
        if (allPickupsReceived)
        {
            Open();
        }
    }

    public void CheckPickups()
    {
        allPickupsReceived = true;
        foreach (ReceivePickup container in receivePickups)
        {
            if (!container.GetPickupReceived())
            {
                allPickupsReceived = false; break;
            }
        }
    }

    private void Open()
    {
        if (!isOpen && transform.position.y >= -transform.localScale.y / 2)
        {
            transform.position -= new Vector3(0, openSpeed * Time.deltaTime, 0);
        }
        else
        {
            isOpen = true;
        }
    }
}
