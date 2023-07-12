using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy1ScriptManager : MonoBehaviour
{
    private Patrulla PatrolScript;
    private Alerta AlertScript;
    private Ataque AttackScript;
    private Animator animator;
    private NavMeshAgent agent;
    private float distanceToPlayer;
    private float detectionDistance = 15;
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
        detectionDistance = easyMode ? detectionDistance / 2: detectionDistance;
        
        distanceToPlayer = Vector3.Distance(transform.position, GameObject.FindWithTag("Player").transform.position);

        animator.SetFloat("distancia", distanceToPlayer);

        animator.SetFloat("velocidad", agent.velocity.magnitude);

        // Si el jugador está en el rango de visión y no está a rango de ataque, va a por él
        if (distanceToPlayer <= detectionDistance && !IsPlayerInAttackRange())
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
        else if (distanceToPlayer > detectionDistance && !IsPlayerInAttackRange())
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

    public void SwitchDifficulty()
    {
        easyMode = !easyMode;
        Debug.Log($"{gameObject.name} difficulty set to {easyMode}");
    }

    private void OnDrawGizmos() // Mostramos el rango de visión y de detección del enemigo
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position, detectionDistance);
    }
}