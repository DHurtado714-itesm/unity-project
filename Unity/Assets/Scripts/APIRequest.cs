using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic; 
using System;
using System.Linq;

[System.Serializable]
public class AgentData
{
    public int[] position;
    public int id;
}

[System.Serializable]
public class APIData
{
    public List<AgentData> agents;
    public List<FoodData> food;
    public int[] deposit_cell;
}

[System.Serializable]
public class FoodData
{
    public int[] position;
}

public class APIRequest : MonoBehaviour
{
    // URL de tu API Flask
    private string apiUrl = "http://127.0.0.1:5000/";

    // Prefab del agente para la instanciación
    public GameObject agentPrefab;

    // Prefab de la comida para la instanciación
    public GameObject foodPrefab;

    // Iniciar la corutina al iniciar el juego
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
                // Enviar la solicitud y esperar la respuesta
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    // Manejar el error de red
                    Debug.Log("Error: " + webRequest.error);
                }
                else
                {
                    // Procesar la respuesta
                    Debug.Log("Response: " + webRequest.downloadHandler.text);
                    ProcessResponse(webRequest.downloadHandler.text);
                }
            }

            // Esperar un tiempo antes de la siguiente solicitud
            yield return new WaitForSeconds(1f);
        }
    }

    // Procesar la respuesta JSON de la API
    void ProcessResponse(string json)
    {

    Debug.Log("Raw JSON response: " + json); // Imprime la respuesta JSON 

        APIData data = JsonUtility.FromJson<APIData>(json);

        if (data.agents == null)
        {
            Debug.LogError("Agent list is null");
            return;
        }

        foreach (var agentData in data.agents)
        {
            Vector2Int agentPosition = new Vector2Int(agentData.position[0], agentData.position[1]);
            int agentId = agentData.id;

            AgentController agentController = FindAgentControllerById(agentId);
            if (agentController != null)
            {
                agentController.UpdateStateFromPython(agentPosition, false);
            }
            else
            {
                Debug.LogError("No AgentController found for ID: " + agentId);
            }
        }
    }

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

    AgentController InstantiateAndRegisterAgent(int id, Vector2Int position)
    {
        Vector3 position3D = new Vector3(position.x, 0, position.y); 
        GameObject agentObject = Instantiate(agentPrefab, position3D, Quaternion.identity);
        AgentController agentController = agentObject.GetComponent<AgentController>();

        agentController.id = id;
        return agentController;
    }
}
