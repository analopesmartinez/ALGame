using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PremadeRoom : MonoBehaviour
{
    [Header("Dimensions")]
    [SerializeField] float x;
    [SerializeField] float z;
    
    public Vector2 GetDimensions() { return new Vector2(x, z);}
}
