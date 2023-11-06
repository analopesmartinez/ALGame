using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
	public NavMeshAgent agent;

	public Transform[] patrolPoints;
	private int currentPatrolIndex;

	[Range(0, 360)]
	public float frontViewAngle; // The angle the AI can see in front
	public float frontViewRadius; // The range the AI can see in front

	[Range(0, 360)]
	public float backViewAngle; // The angle the AI can see behind
	public float backViewRadius; // The range the AI can see behind

	public LayerMask targetMask; // Mask for the player
	public LayerMask obstacleMask; // Mask for the obstacles

	public float chaseSpeed = 5f;
	public float patrolSpeed = 3.5f;
	public float chaseDuration = 10f; // Duration enemy will chase the player after losing sight
	private float chaseTimer;

	private bool isChasing;
	private Transform playerTransform;

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		currentPatrolIndex = 0;
		MoveToNextPatrolPoint();
	}

	private void Update()
	{
		FindVisibleTargets();
		if (isChasing)
		{
			// If chasing, continue to set the player's position as the destination
			if (CanSeePlayer())
			{
				ChaseTarget(playerTransform);
				chaseTimer = chaseDuration; // Reset the chase timer
			}
			else
			{
				// If the timer has expired and the player is no longer visible, stop chasing
				chaseTimer -= Time.deltaTime;
				if (chaseTimer <= 0f)
				{
					isChasing = false; // It's better to set isChasing here
					ReturnToPatrol();
				}
			}
		}
		else
		{
			// Regular patrol behavior
			Patrol();
		}
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
		if (patrolPoints.Length > 0)
		{
			agent.speed = patrolSpeed;
			agent.destination = patrolPoints[currentPatrolIndex].position;
			currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
		}

	}


	void FindVisibleTargets()
	{
		// Check for targets in the front view
		Collider[] targetsInViewFrontRadius = Physics.OverlapSphere(transform.position, frontViewRadius, targetMask);
		foreach (var targetCollider in targetsInViewFrontRadius)
		{
			Transform target = targetCollider.transform;
			playerTransform = target; // Assign the player transform
			if (CanSeePlayer()) // This will now check if the target is in front and visible
			{
				ChaseTarget(target);
				return; // Break the loop if a target is found and being chased
			}
		}

		// Check for targets in the back view
		Collider[] targetsInViewBackRadius = Physics.OverlapSphere(transform.position, backViewRadius, targetMask);
		foreach (var targetCollider in targetsInViewBackRadius)
		{
			Transform target = targetCollider.transform;
			playerTransform = target; // Assign the player transform
			if (CanSeePlayer()) // This will now check if the target is behind and visible
			{
				ChaseTarget(target);
				return; // Break the loop if a target is found and being chased
			}
		}
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



	void ChaseTarget(Transform target)
	{
		isChasing = true;
		agent.speed = chaseSpeed;
		agent.SetDestination(target.position);
	}

	void ReturnToPatrol()
	{
		isChasing = false;
		agent.speed = patrolSpeed; // Reset the speed to patrol speed
								   // Find the closest patrol point to return to
		float closestDistanceSqr = Mathf.Infinity;
		Transform closestPatrolPoint = null;
		foreach (Transform patrolPoint in patrolPoints)
		{
			Vector3 directionToTarget = patrolPoint.position - transform.position;
			float dSqrToTarget = directionToTarget.sqrMagnitude;
			if (dSqrToTarget < closestDistanceSqr)
			{
				closestDistanceSqr = dSqrToTarget;
				closestPatrolPoint = patrolPoint;
			}
		}

		if (closestPatrolPoint != null)
		{
			agent.destination = closestPatrolPoint.position;
			currentPatrolIndex = Array.IndexOf(patrolPoints, closestPatrolPoint);
		}
	}


	void StopChasing()
	{
		isChasing = false;
		agent.speed = patrolSpeed;
		MoveToNextPatrolPoint(); // Return to patrol
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
}