using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FoodController : MonoBehaviour
{
    public GameObject foodPrefab; // Prefab de la comida
    // Este método se llama cuando se crea una instancia de la comida
    public void OnCreated()
    {
        // Aquí puedes agregar lógica que se ejecuta cuando se crea la comida
        // Por ejemplo, iniciar una animación o efectos visuales
        Debug.Log("Comida creada en posición: " + transform.position);
    }

    // Este método se llama antes de que la comida sea destruida
    public void OnDestroyed()
    {
        // Aquí puedes agregar lógica que se ejecuta justo antes de destruir la comida
        // Por ejemplo, reproducir un sonido, generar un efecto, etc.
        Debug.Log("Comida destruida en posición: " + transform.position);
    }

    // Puedes agregar aquí más funciones específicas relacionadas con la comida
}