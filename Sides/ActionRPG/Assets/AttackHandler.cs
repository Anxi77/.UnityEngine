using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    [SerializeField] float attackRange = 1.0f;
    Animator animator;
    CharacterMovement characterMovement;

    InteractableObject target;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterMovement = GetComponent<CharacterMovement>();
    }

    internal void Attack(InteractableObject target)
    {
        this.target = target;
        ProcessAttack();
    }

    private void Update()
    {
        if (target != null)
        {
            ProcessAttack();
        }
    }

    private void ProcessAttack()
    {

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance < attackRange)
        {
            characterMovement.UpdateRotation(target.transform.position);
            animator.SetTrigger("Attack");
            //characterMovement.StopMoving();
            target = null;
        }
        else
        {
            characterMovement.SetDestination(target.transform.position);
        }
    }
}