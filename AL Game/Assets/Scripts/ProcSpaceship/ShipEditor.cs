using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShipCreator))]
public class ShipEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ShipCreator shipCreator = (ShipCreator)target;
        if(GUILayout.Button("Create New Ship"))
        {
            shipCreator.CreateShip();
        }
    }
}
