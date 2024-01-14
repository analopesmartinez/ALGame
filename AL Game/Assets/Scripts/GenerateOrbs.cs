using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateOrbs : MonoBehaviour
{
    [SerializeField] GameObject[] glowingOrbs;
    [SerializeField] int orbCount;

    private void Start()
    {
        for (int i = 0; i < orbCount; i++)
        {
            int orbIdx = Random.Range(0, glowingOrbs.Length);
            Vector3 orbPosition = new Vector3(Random.Range(-300f, 300f), 155f + Random.Range(-145f, 145f), Random.Range(-300f, 300f));
            GameObject orb = Instantiate(glowingOrbs[orbIdx], orbPosition, Quaternion.identity, transform);
            orb.GetComponent<Light>().enabled = false;
            orb.GetComponent<Rigidbody>().useGravity = false;
            SphereCollider[] colliders = orb.GetComponents<SphereCollider>();
            foreach (SphereCollider collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }
}
