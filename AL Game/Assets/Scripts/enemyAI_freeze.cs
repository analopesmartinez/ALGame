using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI_freeze : MonoBehaviour
{
	private enum EnemyState { Patrolling, Chasing, Stopping }
	private EnemyState currentState = EnemyState.Patrolling;

	public NavMeshAgent agent;
	public Transform[] patrolPoints;
	private int currentPatrolIndex;

	[Range(0, 360)]
	public float frontViewAngle;
	public float frontViewRadius;

	[Range(0, 360)]
	public float backViewAngle;
	public float backViewRadius;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	public float chaseSpeed = 5f;
	public float patrolSpeed = 3.5f;
	public float chaseDuration = 10f;
	private float chaseTimer;

	private bool isChasing;
	public Transform playerTransform;
	public float viewAngle = 110f; // The angle the AI can see in front
	public float viewRadius = 15f; // The range the AI can see in front

	private Animator animator;
	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		currentPatrolIndex = 0;
		MoveToNextPatrolPoint();
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		switch (currentState)
		{
			case EnemyState.Patrolling:
				Patrol();
				CheckForPlayer();
				break;
			case EnemyState.Chasing:
				if (IsPlayerLookingAtEnemy())
				{
					currentState = EnemyState.Stopping;
				}
				else
				{
					ChasePlayer();
				}
				break;
			case EnemyState.Stopping:
				StopChasing();
				if (!IsPlayerLookingAtEnemy() && IsPlayerInRange())
				{
					currentState = EnemyState.Chasing;
				}
				break;
		}

		float speed = agent.velocity.magnitude; // Get the speed from the NavMeshAgent
		animator.SetFloat("Speed", speed); // Set the Speed parameter in the Animator
		Debug.Log("Current speed: " + speed);
	}


	void Patrol()
	{
		agent.isStopped = false;
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

	void CheckForPlayer()
	{
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, frontViewRadius, targetMask);
		foreach (var targetCollider in targetsInViewRadius)
		{
			Transform target = targetCollider.transform;
			playerTransform = target;
			if (CanSeePlayer() && !IsPlayerLookingAtEnemy())
			{
				currentState = EnemyState.Chasing;
				return;
			}
		}

		if (currentState != EnemyState.Patrolling && !isChasing)
		{
			currentState = EnemyState.Patrolling;
			MoveToNextPatrolPoint();
		}
	}


	bool IsPlayerInRange()
	{
		return Vector3.Distance(transform.position, playerTransform.position) <= viewRadius;
	}

	void ChasePlayer()
	{
		agent.isStopped = false;
		agent.speed = chaseSpeed;
		agent.SetDestination(playerTransform.position);
	}

	void StopChasing()
	{
		agent.isStopped = true;
		Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}

	bool CanSeePlayer()
	{
		Vector3 dirToTarget = (playerTransform.position - transform.position).normalized;
		float angleToTarget = Vector3.Angle(transform.forward, dirToTarget);
		float distanceToTarget = Vector3.Distance(transform.position, playerTransform.position);

		// If the player is within the full view radius and within the front view angle
		if (angleToTarget <= frontViewAngle / 2 && distanceToTarget <= frontViewRadius)
		{
			// Raycast to check if the player is visible in front without any obstacles
			if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
			{
				return true; // Player is visible in front
			}
		}
		// If the player is behind and within the back view angle
		else if (angleToTarget > 180 - (backViewAngle / 2) && distanceToTarget <= backViewRadius)
		{
			// Raycast to check if the player is visible behind without any obstacles
			if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
			{
				return true; // Player is visible behind
			}
		}

		return false; // Player is not visible
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

	// For visualizing the field of view in the editor
	void OnDrawGizmosSelected()
	{
		// Draw the front view
		Gizmos.color = Color.red;
		DrawFieldOfView(frontViewRadius, frontViewAngle, transform.forward);

		// Draw the back view with a different color
		Gizmos.color = Color.blue;
		DrawFieldOfView(backViewRadius, backViewAngle, -transform.forward); // Notice the -transform.forward for the back view
	}

	void DrawFieldOfView(float radius, float angle, Vector3 facingDirection)
	{
		Vector3 fovLine1 = Quaternion.AngleAxis(angle / 2, transform.up) * facingDirection * radius;
		Vector3 fovLine2 = Quaternion.AngleAxis(-angle / 2, transform.up) * facingDirection * radius;

		Gizmos.DrawRay(transform.position, fovLine1);
		Gizmos.DrawRay(transform.position, fovLine2);

		if (angle < 360)
		{
			Gizmos.DrawLine(transform.position, transform.position + fovLine1);
			Gizmos.DrawLine(transform.position, transform.position + fovLine2);
		}

		DrawFieldOfViewArc(radius, angle, facingDirection);
	}

	void DrawFieldOfViewArc(float radius, float angle, Vector3 facingDirection)
	{
		float totalFOV = angle;
		float rayRange = radius;
		float halfFOV = totalFOV / 2.0f;
		Quaternion facingRotation = Quaternion.LookRotation(facingDirection, Vector3.up);
		float stepSize = totalFOV / 20; // Determines the resolution of the arc
		Vector3 previousDirection = facingRotation * Quaternion.AngleAxis(-halfFOV, Vector3.up) * Vector3.forward;

		for (int i = 1; i <= 20; i++)
		{
			Quaternion rotation = facingRotation * Quaternion.AngleAxis(stepSize * i - halfFOV, Vector3.up);
			Vector3 direction = rotation * Vector3.forward * rayRange;
			Gizmos.DrawLine(transform.position + previousDirection * rayRange, transform.position + direction);
			previousDirection = direction.normalized;
		}
	}

	public void SetPatrolPoints(Transform[] newPatrolPoints)
	{
		patrolPoints = newPatrolPoints;
	}
}