using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab; // Prefab de la comida
    public int gridSize = 20; // Tamaño de la cuadrícula
    public float spawnInterval = 5.0f; // Intervalo entre generaciones de comida
    public int maxFood = 47; // Máxima cantidad de comida

    private float timeSinceLastSpawn = 0.0f;
    private int foodGenerated = 0; // Cantidad de comida generada hasta ahora

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            // Calcula la cantidad máxima de comida que aún se puede generar
            int remainingFood = maxFood - foodGenerated;
            
            // Asegúrate de no generar más comida de la que falta para llegar al límite
            int cellsToSpawn = Random.Range(2, Mathf.Min(6, remainingFood + 1));

            for (int i = 0; i < cellsToSpawn && foodGenerated < maxFood; i++)
            {
                SpawnFood();
            }

            timeSinceLastSpawn = 0.0f;
        }
    }

    void SpawnFood()
    {
        int x = Random.Range(0, gridSize);
        int y = Random.Range(0, gridSize);

        Vector3 spawnPosition = new Vector3(x, 0.5f, y); // Usa un valor decimal para Y

        Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        foodGenerated++; // Incrementa el contador de comida
    }
}
