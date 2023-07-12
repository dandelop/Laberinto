using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objetive : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Cuando colisiona con el jugador activamos la pantalla de victoria
        if (other.gameObject.CompareTag("Player"))
        {
            Generator2.instance.VictoryScreen.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
