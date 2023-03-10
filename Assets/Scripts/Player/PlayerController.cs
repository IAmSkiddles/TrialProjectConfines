using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Character Attributes:")]
    public float baseSpeed = 4.0f;
    public float speedMultiplier = 2.0f;
    public float acceleration = 2.0f;
    public float deceleration = 4.0f;
    public float attackDelay = 0.3f;
    public float hitBoxRadius;

    [HideInInspector]
    public Vector2 MovementInput { get; set; }
    [HideInInspector]
    public bool attackBlocked = false;
    [HideInInspector]
    public float speed;

    public bool attacking { get; private set; }

    private Vector2 oldMovementInput;

    private Rigidbody2D rb2d;
    public Transform hitBox;
    public Animator animator;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        ProcessMovement();
    }

    private void ProcessMovement()
    {
        if (MovementInput.magnitude > 0 && speed >= 0) 
        {
            oldMovementInput = MovementInput;
            speed += baseSpeed * acceleration * Time.deltaTime;
        }
        else 
        {
            speed -= baseSpeed * deceleration * Time.deltaTime;
        }

        speed = Mathf.Clamp(speed, 0, baseSpeed);
        rb2d.velocity = (oldMovementInput * speed) * speedMultiplier;
    }

    public void Attack() 
    {
        if (attackBlocked) return;
        animator.SetTrigger("Attack");
        attacking = true;
        attackBlocked = true;

        StartCoroutine(DelayAttack());
    }

    public void ResetAttacking()
    {
        attacking = false;
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(attackDelay);
        attackBlocked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 position = hitBox == null ? Vector3.zero : hitBox.position;
        Gizmos.DrawWireSphere(position, hitBoxRadius);
    }

    public void DetectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(hitBox.position, hitBoxRadius))
        {
            if (collider.gameObject == gameObject) continue;

            Health health;
            if ((health = collider.GetComponent<Health>()))
            {
                health.OnHit(1, gameObject);
                
                return;
            }
        }
    }
}  
