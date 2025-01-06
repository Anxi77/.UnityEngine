using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] MouseInput mouseInput;
    [Header("CharacterRotation")]
    NavMeshAgent agent;
    public float rotationSpeed = 21f; // 회전 속도 조절을 위한 변수 추가
    public float accelerationRate = 20f; // 가속도 조절을 위한 변수
    public float decelerationRate = 20f; // 감속도 조절을 위한 변수
    public float directionChangeThreshold = 60f; // 방향 변화 감지 임계값 (도)

    Animator animator;
    private Vector3 lastDirection;
    private bool isMoving = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        lastDirection = transform.forward;
        // NavMeshAgent 설정
        agent.acceleration = accelerationRate;
    }

    public void StopMoving()
    {
        isMoving = false;
        agent.ResetPath();
    }

    public void SetDestination(Vector3 destinationPosition)
    {
        agent.SetDestination(destinationPosition);
        isMoving = true;
    }

    private void Update()
    {
        AnimatorMovement();
        
        if (isMoving)
        {
            UpdateMovement();
        }
        // 목적지에 도착했는지 확인
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isMoving = false;
        }
    }

    private void UpdateMovement()
    {
        if (agent.hasPath)
        {
            Vector3 desiredDirection = agent.desiredVelocity.normalized;

            AdjustVelocity(desiredDirection);
            UpdateRotation(desiredDirection);

            lastDirection = desiredDirection;
        }
    }

    private void AdjustVelocity(Vector3 desiredDirection)
    {
        float angleChange = Vector3.Angle(lastDirection, desiredDirection);

        if (angleChange > directionChangeThreshold)
        {
            agent.velocity = Vector3.Lerp(agent.velocity, Vector3.zero, Time.deltaTime * decelerationRate);
        }
        else
        {
            agent.velocity = Vector3.Lerp(agent.velocity, agent.desiredVelocity, Time.deltaTime * accelerationRate);
        }
    }

    public void UpdateRotation(Vector3 desiredDirection)
    {
        if (desiredDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(desiredDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void AnimatorMovement() 
    {
        float motion = agent.velocity.magnitude;

        animator.SetFloat("motion", motion);
    }
}
