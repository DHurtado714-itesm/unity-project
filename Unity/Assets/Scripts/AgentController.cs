using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    // Variables de configuración del agente
    public int id; // Identificador del agente
    public float speed = 5.0f; // Velocidad de movimiento del agente
    public GameObject foodPrefab; // Prefab de la comida
    public Transform foodAnchor; // Punto de anclaje para la comida

    // Estado y posicionamiento del agente
    public Vector2Int targetPosition; // Posición objetivo del agente
    public bool isCarryingFood = false; // Indica si el agente está llevando comida

    // Variables internas
    private GameObject foodInstance; // Instancia actual de la comida
    private Dictionary<Vector2Int, GameObject> foodInstances; // Referencia al diccionario de instancias de comida

    // Inicialización
    void Start()
    {
        // Obtener la referencia a foodInstances desde el singleton APIRequest
        if (APIRequest.Instance != null)
        {
            foodInstances = APIRequest.Instance.foodInstances;
        }
        else
        {
            Debug.LogError("APIRequest.Instance is null in AgentController Start");
        }
    }

    // Actualización en cada frame
    void Update()
    {
        // Mover al agente hacia la posición objetivo
        Vector3 targetPosition3D = new Vector3(targetPosition.x, 0, targetPosition.y);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition3D, speed * Time.deltaTime);

        // Comprobar si hay comida en la posición actual del agente
        CheckForFoodAtPosition();
    }

    // Actualizar el estado del agente desde Python
    public void UpdateStateFromPython(Vector2Int newPosition, bool carryingFood)
    {
        targetPosition = newPosition;
        isCarryingFood = carryingFood;
    }

    // Comprobar si hay comida en la posición actual del agente
    void CheckForFoodAtPosition()
    {
        if (foodInstances == null)
        {
            Debug.LogError("foodInstances is null in AgentController");
            return;
        }

        Vector2Int currentPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

        if (foodInstances.ContainsKey(currentPos))
        {
            GameObject foodAtPosition = foodInstances[currentPos];

            if (isCarryingFood)
            {
                Destroy(foodAtPosition);
                foodInstances.Remove(currentPos);
            }
            else
            {
                // Lógica cuando el agente encuentra comida pero no la recoge
            }
        }
    }
}
