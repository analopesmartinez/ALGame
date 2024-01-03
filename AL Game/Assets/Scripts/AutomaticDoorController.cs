using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoorController : MonoBehaviour
{
    [SerializeField] GameObject topRight, topLeft, bottomRight, bottomLeft;
    [SerializeField] float doorSpeed;
    private bool doorOpen;
    private bool doorOpening;
    private bool doorClosing;

    private void Awake()
    {
        doorOpen = false;
        doorClosing = false;
        doorOpening = false;
    }

    private void Update()
    {
        OpenDoor();
        CloseDoor();
    }

    private void CloseDoor()
    {
        if(doorClosing)
        {
            if (doorOpen)
            {
                SetDoorVisibility(true);

                doorOpen = false;
            }

            topRight.transform.localPosition -= Vector3.right * doorSpeed * Time.deltaTime;
            bottomLeft.transform.localPosition += Vector3.right * doorSpeed * Time.deltaTime;
            topLeft.transform.localPosition -= Vector3.up * doorSpeed * Time.deltaTime;
            bottomRight.transform.localPosition += Vector3.up * doorSpeed * Time.deltaTime;

            if (topRight.transform.localPosition.x <= 1.5)
            {
                doorClosing = false;
            }

        }
    }

    private void OpenDoor()
    {
        if(!doorOpen && doorOpening)
        {
            topRight.transform.localPosition += Vector3.right * doorSpeed * Time.deltaTime;
            bottomLeft.transform.localPosition -= Vector3.right * doorSpeed * Time.deltaTime;
            topLeft.transform.localPosition += Vector3.up * doorSpeed * Time.deltaTime;
            bottomRight.transform.localPosition -= Vector3.up * doorSpeed * Time.deltaTime;


            if (topRight.transform.localPosition.x >= 4)
            {
                SetDoorVisibility(false);
                doorOpen = true;
                doorOpening = false;
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Door opening activated");
        doorOpening = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Door closing activated");
        doorClosing = true;
    }

    private void SetDoorVisibility(bool visible)
    {
        topRight.SetActive(visible);
        bottomLeft.SetActive(visible);
        topLeft.SetActive(visible);
        bottomRight.SetActive(visible);
    }
}
