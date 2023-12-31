//AgentController
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
        Vector3 targetPosition3D = new Vector3(targetPosition.x, 0.12f, targetPosition.y);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition3D, speed * Time.deltaTime);

        // Calcular la dirección hacia la posición objetivo
        if (transform.position != targetPosition3D) // Asegúrate de que haya un movimiento
        {
            Vector3 direction = (targetPosition3D - transform.position).normalized; // Calcula la dirección hacia la posición objetivo
            Quaternion lookRotation = Quaternion.LookRotation(direction); // Crea la rotación hacia la dirección objetivo
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed); // Rota el agente hacia esa dirección
        }
       // Actualizar posición del asset de comida
       if (isCarryingFood && foodInstance != null)
       {
           Vector3 foodPosition = transform.position + new Vector3(0, 2f, -0f);
           foodInstance.transform.position = foodPosition;
       }

       // Comprobar comida solo si el agente no está cargando
       if (!isCarryingFood)
       {
           CheckForFoodAtPosition();
       }

   }

   // Actualizar el estado del agente desde Python
   public void UpdateStateFromPython(Vector2Int newPosition, bool carryingFood)
   {
        targetPosition = newPosition;
        
        if(carryingFood && !isCarryingFood)
        {
            isCarryingFood = true;
            // Calcula la posición inicial para la comida
            Vector3 initialFoodPosition = transform.position + new Vector3(0, 0.8f, -0.5f);
            // Instancia la comida directamente en la posición correcta
            foodInstance = Instantiate(foodPrefab, initialFoodPosition, Quaternion.identity, transform);
            foodInstance.transform.localScale = new Vector3(4, 4, 4);
        }
        else if(!carryingFood && isCarryingFood)
        {
            isCarryingFood = false;
            if(foodInstance != null)
            {
                Destroy(foodInstance);
                foodInstance = null;
            }
        }
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

       if (foodInstances.ContainsKey(currentPos) && !isCarryingFood)
       {
           GameObject foodAtPosition = foodInstances[currentPos];

           // Recoger la comida
           foodInstance = Instantiate(foodPrefab, foodAnchor.position, Quaternion.identity, transform);
           foodInstance.transform.localScale = new Vector3(4, 4, 4);
           Destroy(foodAtPosition);
           foodInstances.Remove(currentPos);

           isCarryingFood = true;
       }
   }
}
