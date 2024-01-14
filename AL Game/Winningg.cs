using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Winningg : MonoBehaviour
{
    public TMP_Text winText;

    private void OnTriggerEnter(Collider other)
    {
<<<<<<< HEAD:AL Game/Winningg.cs
=======
        Debug.Log("Triggered");
>>>>>>> Oscar:AL Game/Assets/Scripts/LevelComplete.cs
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("won");
            winText.text = "You win...\r\nfor now";
        }
        
    }
}
