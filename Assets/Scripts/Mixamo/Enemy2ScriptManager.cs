using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy2ScriptManager : MonoBehaviour
{
    private Patrulla PatrolScript;
    private Alerta AlertScript;
    private Ataque AttackScript;
    private Animator animator;
    private NavMeshAgent agent;
    private float angleVision = 120;
    private float detectionDistance = 15;
    private float distanceToPlayer;
    private Vector3 directionToPlayer;
    public bool easyMode = true;

    private void Awake()
    {
        PatrolScript = GetComponent<Patrulla>();
        AlertScript = GetComponent<Alerta>();
        AttackScript = GetComponent<Ataque>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        // Los enemigos empiezan en modo patrulla
        PatrolScript.enabled = true;
        AlertScript.enabled = false;
        AttackScript.enabled = false;
    }

    // Establecemos las condiciones para que el enemigo pase de un estado a otro
    void Update()
    {
        angleVision = easyMode ? angleVision / 2 : angleVision;
        
        detectionDistance = easyMode ? detectionDistance / 2 : detectionDistance;
        
        distanceToPlayer = Vector3.Distance(transform.position, GameObject.FindWithTag("Player").transform.position);

        animator.SetFloat("distancia", distanceToPlayer);

        animator.SetFloat("velocidad", agent.velocity.magnitude);

        // Si el jugador está en el rango de visión y no está a rango de ataque, va a por él
        if (IsPlayerInSight() && !IsPlayerInAttackRange())
        {
            PatrolScript.enabled = false;
            AlertScript.enabled = true;
            AttackScript.enabled = false;
        }
        // Si el jugador está en rango de ataque, empieza a atacar
        else if (IsPlayerInAttackRange())
        {
            PatrolScript.enabled = false;
            AlertScript.enabled = false;
            AttackScript.enabled = true;
        }
        // Si el jugador no está en el rango de visión ni le está atacando, vuelve a patrullar
        else if (!IsPlayerInSight() && !IsPlayerInAttackRange())
        {
            PatrolScript.enabled = true;
            AlertScript.enabled = false;
            AttackScript.enabled = false;
        }
    }

    // Determina si el jugador está dentro del rango de ataque
    private bool IsPlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, GameObject.FindWithTag("Player").transform.position) <= 2;
    }

    // Determina si el jugador está dentro del rango de visión
    private Boolean IsPlayerInSight()
    {
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position - transform.position;

        directionToPlayer = playerPosition - transform.position;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < angleVision / 2)
        {
            if (Physics.Raycast(transform.position, directionToPlayer, out var hit, detectionDistance))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SwitchDifficulty()
    {
        easyMode = !easyMode;
    }

    /* private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angleVision / 2, 0) * transform.forward * (detectionDistance+1));
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angleVision / 2, 0) * transform.forward * (detectionDistance+1));
        Gizmos.DrawSphere(transform.position, detectionDistance);
    } */
}