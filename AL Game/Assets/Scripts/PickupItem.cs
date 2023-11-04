using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float hitDistance;

    private Camera _camera;
    private GameObject itemPickedUp;
    private void Start()
    {
        _camera = Camera.main;
    }
    private void OnPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, hitDistance, layerMask))
        {
            Debug.Log("Raycast hit");
            if (hit.rigidbody != null)
            {
                hit.rigidbody.useGravity = false;
                hit.rigidbody.isKinematic = true;
            }
            hit.transform.SetParent(_camera.transform);
            itemPickedUp = hit.transform.gameObject;
        }
    }

    private void OnRelease()
    {
        if (itemPickedUp != null)
        {
            Rigidbody _rigidbody = itemPickedUp.GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _rigidbody.useGravity = true;
                _rigidbody.isKinematic = false;
            }
            if (itemPickedUp.transform.parent == _camera.transform)
            {
                itemPickedUp.transform.SetParent(null);
            }
        }
    }
}
