using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigibodyMove : MonoBehaviour
{
    private float MoveSpeed = 6f;

    void Start()
    {

    }
    void Update()
    {
        //w or up arrow
        if (Input.GetKey(KeyCode.W))
        {
            //move forward
            this.transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            //move backward
            this.transform.Translate(Vector3.back * MoveSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //up arrow increasing speed
            MoveSpeed += 2;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //down arrow decreasing speed
            MoveSpeed -= 2;
        }

        //a or left arrow
        if (Input.GetKey(KeyCode.A))
        {
            //move left
            this.transform.Translate(Vector3.left * MoveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            //move right
            this.transform.Translate(Vector3.right * MoveSpeed * Time.deltaTime);
        }
    }
}
