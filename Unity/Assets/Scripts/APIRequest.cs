using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

// Definición de las estructuras de datos para la API
[System.Serializable]
public class AgentData
{
    public int[] position;
    public int id;
    public bool is_carrying;
}

[System.Serializable]
public class FoodData
{
    public int[] position;
}

[System.Serializable]
public class APIData
{
    public List<AgentData> agents;
    public List<FoodData> food;
    public int[] deposit_cell;
}

public class APIRequest : MonoBehaviour
{
    // URL de la API
    private string apiUrl = "http://127.0.0.1:5000";

    // Prefabs para instanciación
    public GameObject agentPrefab;
    public GameObject foodPrefab;
    public GameObject depositPrefab;

    // Control para la instanciación única del depósito
    private bool depositCreated = false;

    // Diccionario para mantener las instancias de comida
    public Dictionary<Vector2Int, GameObject> foodInstances;

    // Singleton de APIRequest
    public static APIRequest Instance { get; private set; }

    // Inicialización de componentes y singleton
    void Awake()
    {
        //foodInstances = new Dictionary<Vector2Int, GameObject>();
        if (Instance == null)
        {
            Instance = this;
            foodInstances = new Dictionary<Vector2Int, GameObject>();
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Iniciar la solicitud a la API cuando el juego comienza
    void Start()
    {
        StartCoroutine(GetRequest(apiUrl));
    }

    // Corutina para realizar la solicitud GET
    IEnumerator GetRequest(string uri)
    {
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.Log("Error: " + webRequest.error);
                }
                else
                {
                    ProcessResponse(webRequest.downloadHandler.text);
                }
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    // Procesar la respuesta JSON de la API
    void ProcessResponse(string json)
    {
        APIData data = JsonUtility.FromJson<APIData>(json);
        Debug.Log("Received JSON: " + json);

        if (data == null)
        {
            Debug.LogError("Failed to deserialize JSON");
            return;
        }        

        if (data.food == null)
        {
            Debug.LogError("food is null");
            return;
        }

        // Actualizar agentes
        foreach (var agentData in data.agents)
        {
            Vector2Int agentPosition = new Vector2Int(agentData.position[0], agentData.position[1]);
            AgentController agentController = FindAgentControllerById(agentData.id);
            if (agentController != null)
            {
                agentController.UpdateStateFromPython(agentPosition, agentData.is_carrying);
            }
            else
            {
                Debug.LogError("No AgentController found for ID: " + agentData.id);
            }
        }

        // Instanciar el depósito si aún no se ha creado
        if (!depositCreated)
        {
            InstantiateDeposit(data.deposit_cell);
            depositCreated = true;
        }

        // Actualizar instancias de comida
       UpdateFoodInstances(data.food);
    }

    // Instanciar el depósito
    void InstantiateDeposit(int[] depositPosition)
    {
        Vector3 position3D = new Vector3(depositPosition[0], 0, depositPosition[1]);
        Instantiate(depositPrefab, position3D, Quaternion.identity);
    }

    // Encontrar un agente por su ID
    AgentController FindAgentControllerById(int id)
    {
        foreach (AgentController controller in FindObjectsOfType<AgentController>())
        {
            if (controller.id == id)
            {
                return controller;
            }
        }

        return null;
    }

    // Instanciar y registrar un agente
    AgentController InstantiateAndRegisterAgent(int id, Vector2Int position)
    {
        Vector3 position3D = new Vector3(position.x, 0, position.y);
        GameObject agentObject = Instantiate(agentPrefab, position3D, Quaternion.identity);
        AgentController agentController = agentObject.GetComponent<AgentController>();
        agentController.id = id;
        //agentController.SetFoodInstances(foodInstances); // Si se queda así, marca el error de null
        return agentController;
    }

    // Actualizar las instancias de comida en el juego
    void UpdateFoodInstances(List<FoodData> foodDataList)
    {
        if (foodDataList == null)
        {
            Debug.LogError("foodDataList is null in UpdateFoodInstances");
            return;
        }

        HashSet<Vector2Int> updatedFoodPositions = new HashSet<Vector2Int>();

        foreach (var foodData in foodDataList)
        {
            if (foodData == null || foodData.position == null || foodData.position.Length != 2)
            {
                Debug.LogError("Invalid foodData in UpdateFoodInstances");
                continue;
            }

            Vector2Int pos = new Vector2Int(foodData.position[0], foodData.position[1]);
            updatedFoodPositions.Add(pos);

            if (!foodInstances.ContainsKey(pos))
            {
                Vector3 position3D = new Vector3(pos.x, 0.12f, pos.y);
                GameObject foodObj = Instantiate(foodPrefab, position3D, Quaternion.identity);
                foodInstances[pos] = foodObj;
            }
        }

        // Eliminar instancias de comida que ya no están en la lista actualizada
        foreach (var existingPos in new List<Vector2Int>(foodInstances.Keys))
        {
            if (!updatedFoodPositions.Contains(existingPos))
            {
                Destroy(foodInstances[existingPos]);
                foodInstances.Remove(existingPos);
            }
        }
    }

}
