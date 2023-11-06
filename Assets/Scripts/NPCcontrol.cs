using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCcontrol : MonoBehaviour
{
    private bool Ismove=true;
    public GameObject Player;
    private bool Isattack = false;
    public Animator Ani;
    void Start()
    {
        InvokeRepeating("Aset", 5, 5);
        Ani.SetBool("run", Ismove);
        //Animator for future use
    }
    public void Aset()
    {
        Ismove = !Ismove;
        Ani.SetBool("run", Ismove);
    }
    void Update()
    {
        if (!Isattack)
        {
            if (Ismove)
            {
                transform.Translate(Vector3.forward * 3 * Time.deltaTime); //change the number to modify the speed of enemy moving forward
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                {
                    Isattack = true;
                    Ani.SetBool("run", true);
                    CancelInvoke("Aset");
                    gameObject.transform.LookAt(Player.transform.localPosition);
                    
                }
            }
        }
        else
        {
            gameObject.transform.LookAt(Player.transform.localPosition);
            transform.Translate(Vector3.forward * 10 * Time.deltaTime); //change the number to modify the speed of enemy chasing
        }
        
    }
}
