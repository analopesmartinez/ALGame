using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class electricityFxTest : MonoBehaviour
{
    public Transform startPosition;
    public Transform endPosition;
    [SerializeField] int divisions;
    [SerializeField]
    [Range(0f, 0.05f)] float width;
    [SerializeField]
    [Range(0f, 1f)] float amplitude;
    [SerializeField]
    [Range(1, 10)] int beamCount;
    public Material material;
    private List<Vector3> positions;
    private Vector3 startEndVector;
    private List<LineRenderer> lineRenderers;

    private void Awake()
    {
        positions = new List<Vector3>();
        lineRenderers = new List<LineRenderer>();
    }

    private void Start()
    {
        startEndVector = (endPosition.position - startPosition.position) / divisions;
        for (int i = 0; i < divisions + 1; i++)
        {
            positions.Add(startPosition.position + startEndVector * i);
        }
        for (int i = 0; i < beamCount; i++)
        {
            GameObject lineObject = new GameObject("LineRenderer");
            lineObject.transform.parent = transform;
            lineRenderers.Add(lineObject.AddComponent<LineRenderer>());

            lineRenderers[i].positionCount = positions.Count;
            lineRenderers[i].startWidth = width;
            lineRenderers[i].endWidth = width;
            lineRenderers[i].material = material;
        }
    }

    void Update()
    {
        positions = new List<Vector3>();
        startEndVector = (endPosition.position - startPosition.position) / divisions;
        for (int i = 0; i < divisions + 1; i++)
        {
            positions.Add(startPosition.position + startEndVector * i);
        }
        foreach (LineRenderer lineRenderer in lineRenderers)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                if (i == 0 || i == positions.Count - 1)
                {
                    lineRenderer.SetPosition(i, positions[i]);
                }
                else
                {
                    lineRenderer.SetPosition(i, positions[i] + new Vector3(Random.Range(-amplitude, amplitude), Random.Range(-amplitude, amplitude), Random.Range(-amplitude, amplitude)));
                }
            }
        }
    }
}
