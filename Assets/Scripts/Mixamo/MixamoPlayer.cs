using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MixamoPlayer : MonoBehaviour
{
    private NavMeshAgent _agent;
    private bool firstClick = true;
    private Vector3 lastDestination = Vector3.zero;
    public bool easyMode = true;
    public Animator _animator;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    public void SwitchDifficulty()
    {
        easyMode = !easyMode;
        Debug.Log($"{gameObject.name} difficulty set to {easyMode}");
    }
    
    void Update()
    {
        _animator.SetFloat("velocidad", _agent.velocity.magnitude);
        
        _agent.speed = easyMode ? 3.5f * 0.8f : 3.5f * 0.6f;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (firstClick || Vector3.Distance(_agent.transform.position, lastDestination) <= 5)
                {
                    _agent.SetDestination(hit.point);
                    lastDestination = hit.point;
                    firstClick = false;
                }
            }
        }
    }
}