using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Generator2 : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab1;
    [SerializeField] private GameObject enemyPrefab2;
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private GameObject contenedor;
    [SerializeField] private int minBloques;
    [SerializeField] private GameObject objectivePrefab;

    [SerializeField] public GameObject LoadingScreen;
    [SerializeField] public GameObject VictoryScreen;
    [SerializeField] public GameObject DefeatScreen;
    [SerializeField] public GameObject changeDifficulty;
    [SerializeField] public GameObject _mainCamera;

    public Vector2 mapSize;
    public GameObject[,] mapa;
    public static Generator2 instance;

    private int contadorBloques = 0;
    private int bloqueObjetivo;
    private Vector2 posicionBloqueJugador;

    public NavMeshSurface PlayerSurface;
    public NavMeshSurface EnemySurface;
    
    public GameObject player;
    public GameObject enemy1;
    public GameObject enemy2;

    private void Awake()
    {
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        
        // Después de recargar la escena el tiempo vuelve a moverse
        Time.timeScale = 1;

        // Singleton
        instance = this;

        // Mostramos la pantalla de carga
        LoadingScreen.SetActive(true);

        // Creamos el mapa
        mapa = new GameObject[(int)mapSize.x, (int)mapSize.y];

        // Creamos el primer bloque
        primeraPieza();

        // Comprobación de bloques
        StartCoroutine(FinalCheck());
    }

    private IEnumerator FinalCheck()
    {
        // Después de 3 segundos comprobamos si se ha llegado al mínimo de bloques
        yield return new WaitForSeconds(3);
        if (contadorBloques < minBloques)
        {
            // Si no se ha llegado al mínimo de bloques reiniciamos la escena
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else // Si cumple con el mínimo de bloques
        {
            // Guardamos en un array los dos NavMeshSurface del contenedor
            NavMeshSurface[] navMeshSurfaceArray = contenedor.GetComponents<NavMeshSurface>();

            // Guardamos en variables los dos NavMeshSurface
            PlayerSurface = navMeshSurfaceArray[0];
            EnemySurface = navMeshSurfaceArray[1];

            // Tiempo de espera por seguridad
            yield return new WaitForSeconds(1);

            // Bakeamos ambas superficies
            PlayerSurface.BuildNavMesh();
            EnemySurface.BuildNavMesh();

            // Tiempo de espera por seguridad
            yield return new WaitForSeconds(1);

            // Creamos el objetivo en el centro del mapa
            Instantiate(objectivePrefab, new Vector3((mapSize.x / 2) * 5, 2f, (mapSize.y / 2) * 5),
                Quaternion.identity);

            // Nos aseguramos de que el jugador aparezca en un bloque que esté cerca del borde del mapa
            Instantiate(playerPrefab, new Vector3(posicionBloqueJugador.x, 2f, posicionBloqueJugador.y), Quaternion.identity);

            // Instanciamos los 2 tipos de enemigos en bloques aleatorios
            CreateEnemy(enemyPrefab1);
            CreateEnemy(enemyPrefab2);
            
            // Registramos al jugador y a los enemigos
            player = GameObject.FindGameObjectWithTag("Player");
            enemy1 = GameObject.FindGameObjectWithTag("Enemy1");
            enemy2 = GameObject.FindGameObjectWithTag("Enemy2");

            // La carga ha finalizado
            Debug.Log("Laberinto generado con exito");
            
            // Ocultamos la pantalla de carga y activamos el botón de cambiar de dificultad
            LoadingScreen.SetActive(false);
            changeDifficulty.SetActive(true);
            changeDifficulty.GetComponent<Image>().color = Color.green;
            
            // Metemos la cámara como hijo del jugador
            _mainCamera.transform.parent = player.transform;
            
            // Colocamos la cámara encima del jugador
            _mainCamera.transform.localPosition = new Vector3(0, 25, 0);
            _mainCamera.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
    }

    private void CreateEnemy(GameObject enemyTypePrefab)
    {
        // Creamos un enemigo en una posición aleatoria
        int x = Random.Range(0, (int)mapSize.x * 5);
        int z = Random.Range(0, (int)mapSize.y * 5);

        // Comprobamos que el bloque no esté ya ocupado por el jugador
        if (x != posicionBloqueJugador.x && z != posicionBloqueJugador.y)
        {
            Instantiate(enemyTypePrefab, new Vector3(x, 1.5f, z), Quaternion.identity);
        }
        else
        {
            // Si el bloque está ocupado por el jugador, volvemos a llamar a la función
            CreateEnemy(enemyTypePrefab);
        }
    }

    private void primeraPieza()
    {
        // Seleccionamos el centro del mapa y ponemos el primer bloque
        GameObject first = Instantiate(brickPrefab, new Vector3((mapSize.x / 2) * 5, 0, (mapSize.y / 2) * 5),
            Quaternion.identity);
        mapa[(int)(mapSize.x / 2), (int)(mapSize.y / 2)] = first;
        first.GetComponent<Piece2>().posX = (int)(mapSize.x / 2);
        first.GetComponent<Piece2>().posZ = (int)(mapSize.y / 2);

        // Lo guardamos en un contenedor
        first.transform.parent = contenedor.transform;

        //Selecciono las paredes libres. En la primera pieza son todas.
        first.GetComponent<Piece2>().a = true;
        first.GetComponent<Piece2>().s = true;
        first.GetComponent<Piece2>().d = true;
        first.GetComponent<Piece2>().w = true;

        // Contamos para comprobar si llega al minimo de bloques
        contadorBloques = 1;
    }

    public void crearPieza(int x, int z)
    {
        // Si la posición no está ocupada colocamos una pieza
        if (mapa[x, z] == null)
        {
            // Contamos cada bloque para comprobar si se llega al mínimo
            contadorBloques++;

            // Creamos el bloque
            GameObject bloque = Instantiate(brickPrefab, new Vector3(x * 5, 0, z * 5), Quaternion.identity);

            // Guardamos la posición del ultimo bloque creado para colocar al jugador, de esta forma nos aseguramos 
            // de que el jugador aparezca en un bloque que esté cerca del borde del mapa pero no fuera del mismo
            posicionBloqueJugador = new Vector2(bloque.transform.position.x, bloque.transform.position.z);

            // Lo registramos en el mapa
            bloque.GetComponent<Piece2>().posX = x;
            bloque.GetComponent<Piece2>().posZ = z;
            mapa[x, z] = bloque;

            // Lo guardamos en un contenedor
            bloque.transform.parent = contenedor.transform;
        }
    }

    private void Update()
    {
        // Botón para cambiar la dificultad
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeDifficulty();
        }
    }

    public void ChangeDifficulty()
    {
        // LLama a los metodos SwitchDifficulty de los scripts de los enemigos y del player
        player.GetComponent<MixamoPlayer>().SwitchDifficulty();
        enemy1.GetComponent<Enemy1ScriptManager>().SwitchDifficulty();
        enemy2.GetComponent<Enemy2ScriptManager>().SwitchDifficulty();
        
        // Cambia el color del botón y el texto
        changeDifficulty.GetComponent<Image>().color = (changeDifficulty.GetComponent<Image>().color == Color.red ? Color.green : Color.red);
        changeDifficulty.GetComponentInChildren<TextMeshProUGUI>().text = changeDifficulty.GetComponentInChildren<TextMeshProUGUI>().text == "Fácil" ? "Difícil" : "Fácil";
    }
    
    // Botón para reiniciar el juego que aparece cuando ganamos o perdemos
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}