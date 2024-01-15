using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCcontrol : MonoBehaviour
{
	private bool Ismove = true;
	public GameObject Player;
	private bool Isattack = false;
	public Animator animator;
	private float moveSpeed = 3f; // Speed when moving normally
	private float chaseSpeed = 10f; // Speed when chasing the player


	void Start()
	{
		InvokeRepeating("ToggleMovement", 5, 5);
		// Initial setting of the speed in the Animator
		animator.SetFloat("Speed", moveSpeed);
	}

	public void ToggleMovement()
	{
		Ismove = !Ismove;
	}

	void Update()
	{
		if (!Isattack)
		{
			if (Ismove)
			{
				transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
				animator.SetFloat("Speed", moveSpeed);
			}
			else
			{
				animator.SetFloat("Speed", 0f); // Set speed to 0 when not moving
			}

			if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
			{
				Isattack = true;
				gameObject.transform.LookAt(Player.transform.localPosition);
				CancelInvoke("ToggleMovement");
			}
		}
		else
		{
			gameObject.transform.LookAt(Player.transform.localPosition);
			transform.Translate(Vector3.forward * chaseSpeed * Time.deltaTime);
			animator.SetFloat("Speed", chaseSpeed);
		}
	}
}