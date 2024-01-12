using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateExit : MonoBehaviour
{
    [SerializeField] int totalConditions;
    [SerializeField] GameObject electricityFxPrefab;
    [SerializeField] GameObject center;
    [SerializeField] GameObject topOfTeleport;
    [SerializeField] float riseSpeed = 0.1f;
    [SerializeField] Material blueMaterial;
    [SerializeField] Material ceilingEmissiveMaterial;
    private Collider exitCollider;
    private List<GameObject> orbPositions;
    private List<electricityFxTest> generatedFx;
    private int exitConditionCount = 0;
    private bool enableExit = false;
    private void Awake()
    {
        exitCollider = GetComponent<Collider>();
        orbPositions = new List<GameObject>();
        generatedFx = new List<electricityFxTest>();
    }
    void Start()
    {
        DisableExit();
    }

    private void Update()
    {
        if (enableExit)
        {
            EnableExit();
        } else
        {
            DisableExit();
        }
    }

    public void ExitCondition(GameObject anchor)
    {
        exitConditionCount++;
        orbPositions.Add(anchor);
        GenerateElectricityFX(orbPositions);
        if (exitConditionCount >= totalConditions)
        {
            enableExit = true;
            ceilingEmissiveMaterial.EnableKeyword("_EMISSION");
        }
    }

    public void VoidExitCondition(GameObject anchor)
    {
        exitConditionCount--;
        orbPositions.Remove(anchor);
        if (exitConditionCount < totalConditions)
        {
            enableExit = false;
            ceilingEmissiveMaterial.DisableKeyword("_EMISSION");
        }
    }

    private void EnableExit()
    {
        if (topOfTeleport.transform.position.y < 5)
        {
            topOfTeleport.transform.position += Vector3.up * riseSpeed;
        }
        else if(exitCollider.enabled == false)
        {
            exitCollider.enabled = true;
        }
    }

    private void DisableExit()
    {
        if (exitCollider.enabled == true)
        {
            exitCollider.enabled = false;
        }
        if (topOfTeleport.transform.position.y > 0)
        {
            topOfTeleport.transform.position -= Vector3.up * riseSpeed * 3;
        }
    }

    private void GenerateElectricityFX(List<GameObject> orbPositions)
    {
        if (orbPositions.Count == 2)
        {
            // FX Between first two pickups
            electricityFxTest electricityFx1 = Instantiate(electricityFxPrefab, this.transform.position, Quaternion.identity, orbPositions[0].transform).GetComponent<electricityFxTest>();
            electricityFxTest electricityFx2 = Instantiate(electricityFxPrefab, this.transform.position, Quaternion.identity, orbPositions[1].transform).GetComponent<electricityFxTest>();
            electricityFx1.startPosition = orbPositions[0].transform;
            electricityFx1.endPosition = orbPositions[1].transform;
            electricityFx1.material = orbPositions[0].GetComponent<Renderer>().material;
            generatedFx.Add(electricityFx1);
            electricityFx2.startPosition = orbPositions[1].transform;
            electricityFx2.endPosition = orbPositions[0].transform;
            electricityFx2.material = orbPositions[1].GetComponent<Renderer>().material;
            generatedFx.Add(electricityFx2);
        }
        if (orbPositions.Count == 3)
        {
            // FX Between Orb Pickups
            electricityFxTest electricityFx1 = Instantiate(electricityFxPrefab, this.transform.position, Quaternion.identity, orbPositions[0].transform).GetComponent<electricityFxTest>();
            electricityFxTest electricityFx2 = Instantiate(electricityFxPrefab, this.transform.position, Quaternion.identity, orbPositions[2].transform).GetComponent<electricityFxTest>();
            electricityFx1.startPosition = orbPositions[0].transform;
            electricityFx1.endPosition = orbPositions[2].transform;
            electricityFx1.material = orbPositions[0].GetComponent<Renderer>().material;
            generatedFx.Add(electricityFx1);
            electricityFx2.startPosition = orbPositions[2].transform;
            electricityFx2.endPosition = orbPositions[0].transform;
            electricityFx2.material = orbPositions[2].GetComponent<Renderer>().material;
            generatedFx.Add(electricityFx2);
            electricityFxTest electricityFx3 = Instantiate(electricityFxPrefab, this.transform.position, Quaternion.identity, orbPositions[2].transform).GetComponent<electricityFxTest>();
            electricityFxTest electricityFx4 = Instantiate(electricityFxPrefab, this.transform.position, Quaternion.identity, orbPositions[1].transform).GetComponent<electricityFxTest>();
            electricityFx3.startPosition = orbPositions[2].transform;
            electricityFx3.endPosition = orbPositions[1].transform;
            electricityFx3.material = orbPositions[2].GetComponent<Renderer>().material;
            generatedFx.Add(electricityFx3);
            electricityFx4.startPosition = orbPositions[1].transform;
            electricityFx4.endPosition = orbPositions[2].transform;
            electricityFx4.material = orbPositions[1].GetComponent<Renderer>().material;
            generatedFx.Add(electricityFx4);

            // FX Between Central Orb
            foreach (GameObject orbPosition in orbPositions)
            {
                electricityFxTest electricityFx5 = Instantiate(electricityFxPrefab, this.transform.position, Quaternion.identity, this.transform).GetComponent<electricityFxTest>();
                electricityFxTest electricityFx6 = Instantiate(electricityFxPrefab, this.transform.position, Quaternion.identity, this.transform).GetComponent<electricityFxTest>();
                electricityFxTest electricityFx7 = Instantiate(electricityFxPrefab, this.transform.position, Quaternion.identity, this.transform).GetComponent<electricityFxTest>();
                electricityFx5.startPosition = orbPositions[0].transform;
                electricityFx5.endPosition = center.transform;
                electricityFx5.material = blueMaterial;
                generatedFx.Add(electricityFx5);
                electricityFx6.startPosition = orbPositions[1].transform;
                electricityFx6.endPosition = center.transform;
                electricityFx6.material = blueMaterial;
                generatedFx.Add(electricityFx6);
                electricityFx7.startPosition = orbPositions[2].transform;
                electricityFx7.endPosition = center.transform;
                electricityFx7.material = blueMaterial;
                generatedFx.Add(electricityFx7);
            }
        }
    }
}
