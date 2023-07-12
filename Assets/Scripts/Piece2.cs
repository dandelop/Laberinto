
using UnityEngine;
using UnityEngine.AI;

public class Piece2 : MonoBehaviour
{
    public bool w = false;
    public bool a = false;
    public bool d = false;
    public bool s = false;
    [SerializeField] private GameObject Pw;
    [SerializeField] private GameObject Pa;
    [SerializeField] private GameObject Pd;
    [SerializeField] private GameObject Ps;
    [SerializeField] private int densidadMuros = 10;
    
    public int posX;
    public int posZ;

    private void Start()
    {
        densidadMuros = densidadMuros + UnityEngine.Random.Range(-20, 20);
        int random = UnityEngine.Random.Range(0, 100);
        
        while(((posZ < Generator2.instance.mapSize.y - 1) || (posX < Generator2.instance.mapSize.x - 1) || (posZ > 0) || (posX > 0)) && !(a || w || s || d))
        {
            if (random < densidadMuros && (posZ < Generator2.instance.mapSize.y - 1))
            {
                w = true;
            }
            random = UnityEngine.Random.Range(0, 100);
            
            if (random < densidadMuros && posX > 0)
            {
                a = true;
            }
            random = UnityEngine.Random.Range(0, 100);
            
            if (random < densidadMuros && (posX < Generator2.instance.mapSize.x - 2))
            {
                d = true;
            }
            random = UnityEngine.Random.Range(0, 100);
            
            if (random < densidadMuros && posZ > 0)
            {
                s = true;
            }
        }
        
        //Elimino las paredes con vecinos
        
        if(posZ < Generator2.instance.mapSize.y - 1 && Generator2.instance.mapa[posX, posZ + 1] != null)
        {
            w = true;
        }
        if (posX < Generator2.instance.mapSize.x - 1 && Generator2.instance.mapa[posX + 1, posZ] != null)
        {
            d = true;
        }
        if (posZ > 0 && Generator2.instance.mapa[posX, posZ - 1] != null)
        {
            s = true;
        }
        if (posX > 0 && Generator2.instance.mapa[posX - 1, posZ] != null)
        {
            a = true;
        }
        
        compruebaMuros();
        crearVecinos();
    }

    private void compruebaMuros()
    {
        if (w)
        {
            Ps.SetActive(false);
        }
        if (s)
        {
            Pw.SetActive(false);
        }
        if (a)
        {
            Pd.SetActive(false);
        }
        if (d)
        {
            Pa.SetActive(false);
        }
    }

    private void crearVecinos()
    {
        if (w  && Generator2.instance.mapa[posX, posZ + 1] == null)
        {   
            //si esta pared no esta, debe haber otro bloque delante
            Generator2.instance.crearPieza(posX, posZ + 1);
        }
        if (s  &&  Generator2.instance.mapa[posX, posZ - 1] == null)
        {   
            Generator2.instance.crearPieza(posX, posZ - 1);
        }
        if (d && Generator2.instance.mapa[posX + 1, posZ] == null)
        {
            Generator2.instance.crearPieza(posX + 1, posZ);
        }
        if ( a && Generator2.instance.mapa[posX - 1, posZ] == null)
        {
            Generator2.instance.crearPieza(posX - 1, posZ);
        }
    }
}
