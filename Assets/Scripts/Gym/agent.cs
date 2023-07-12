using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class agent : MonoBehaviour
{
    public GameObject agentPrefab;
    
    
    // Start is called before the first frame update
    void Start()
    {
        //instanciate agent
        Instantiate(agentPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
