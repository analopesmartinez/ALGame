using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Winningg : MonoBehaviour
{
    public TMP_Text winText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("won");
            winText.text = "You win...\r\nfor now";
        }
        
    }
}
