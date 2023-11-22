using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    // Variables para controlar el movimiento y estado
    public Vector2Int targetPosition;
    public bool isCarryingFood = false;
    public float speed = 5.0f;
    public GameObject foodPrefab; // Prefab de la comida
    public Transform foodAnchor; // Punto de anclaje para la comida
    private GameObject foodInstance; // Instancia actual de la comida

    void Update()
    {
        Vector3 targetPosition3D = new Vector3(targetPosition.x, 0, targetPosition.y);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition3D, speed * Time.deltaTime);

        if (isCarryingFood && foodInstance == null)
        {
            // Si el agente está llevando comida y no hay una instancia de comida la crea
            foodInstance = Instantiate(foodPrefab, foodAnchor.position, Quaternion.identity, foodAnchor);
        }
        else if (!isCarryingFood && foodInstance != null)
        {
            // Si el agente no está llevando comida y hay una instancia de comida, destrúyela
            Destroy(foodInstance);
        }
    }

    public void UpdateStateFromPython(Vector2Int newPosition, bool carryingFood)
    {
        targetPosition = newPosition;
        isCarryingFood = carryingFood;
    }
}
