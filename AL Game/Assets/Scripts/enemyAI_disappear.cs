using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class EnemyAI_disappear : MonoBehaviour
{
	public NavMeshAgent agent;

	public Transform[] patrolPoints;
	private int currentPatrolIndex;

	public Transform playerTransform; // Transform of the player's camera
	public float viewAngle = 110f; // The angle the AI can see in front
	public float viewRadius = 15f; // The range the AI can see in front
	public LayerMask targetMask; // Mask for the player
	public LayerMask obstacleMask; // Mask for the obstacles

	public float patrolSpeed = 3.5f;
	private bool isStopping = false;

	public float transparencyRate = 0.5f; // Rate at which enemy becomes transparent
	private float currentTransparency = 1.0f;
	private Material enemyMaterial;

	private Animator animator;


	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		currentPatrolIndex = 0;
		MoveToNextPatrolPoint();
		Renderer renderer = GetComponent<Renderer>();
		if (renderer != null)
		{
			enemyMaterial = renderer.material;
		}

		animator = GetComponent<Animator>();
        playerTransform = FindObjectOfType<CharacterController>().transform;

    }

	private void Update()
	{
		if (!isStopping)
		{
			Patrol();
			if (IsPlayerLookingAtEnemy())
			{
				StopAndFacePlayer();
				StartCoroutine(BecomeTransparent());
			}
		}

		float speed = agent.velocity.magnitude; // Get the speed from the NavMeshAgent
		animator.SetFloat("Speed", speed); // Set the Speed parameter in the Animator

	}

	void Patrol()
	{
		if (!agent.pathPending && agent.remainingDistance < 0.5f)
		{
			MoveToNextPatrolPoint();
		}
	}

	void MoveToNextPatrolPoint()
	{
		if (patrolPoints.Length == 0)
			return;
		agent.speed = patrolSpeed;
		agent.destination = patrolPoints[currentPatrolIndex].position;
		currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
		

	}

	bool IsPlayerLookingAtEnemy()
	{
		Vector3 directionToEnemy = transform.position - playerTransform.position;
		float angleToEnemy = Vector3.Angle(playerTransform.forward, directionToEnemy);

		if (angleToEnemy < viewAngle / 2)
		{
			RaycastHit hit;
			if (Physics.Raycast(playerTransform.position, directionToEnemy.normalized, out hit, viewRadius))
			{
				if (hit.collider.gameObject == this.gameObject)
				{
					return true;
				}
			}
		}
		return false;
	}

	void StopAndFacePlayer()
	{
		isStopping = true;
		agent.isStopped = true;
		Vector3 directionToPlayer = playerTransform.position - transform.position;
		directionToPlayer.y = 0; // Keep the enemy upright
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime * 5f);
	}


	IEnumerator BecomeTransparent()
	{
		Renderer[] allRenderers = GetComponentsInChildren<Renderer>(); // This will get all renderers in the GameObject and its children

		while (currentTransparency > -0.3)
		{
			currentTransparency -= Time.deltaTime * transparencyRate;
			foreach (Renderer renderer in allRenderers)
			{
				foreach (Material mat in renderer.materials)
				{
					Color newColor = mat.color;
					newColor.a = currentTransparency;
					mat.color = newColor;
				}
			}
			yield return null;
		}
	}

}

