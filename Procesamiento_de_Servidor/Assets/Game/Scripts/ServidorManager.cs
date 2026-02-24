using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServidorManager : MonoBehaviour
{
    // ===== ESTRUCTURAS PRINCIPALES =====
    public Queue<PaqueteDato> colaProcesamiento = new Queue<PaqueteDato>();
    public Dictionary<string, PaqueteDato> historialProcesados = new Dictionary<string, PaqueteDato>();

    // ===== MÉTRICAS =====
    private float sumaTiemposEspera = 0f;
    private int totalProcesados = 0;
    public float TiempoPromedioEspera
    {
        get
        {
            if (totalProcesados > 0)
                return sumaTiemposEspera / totalProcesados;
            else
                return 0f;
        }
    }

    // ===== CONFIGURACIÓN GENERACIÓN =====
    [Header("Configuración de generación")]
    [SerializeField] private float intervaloMin = 2f;
    [SerializeField] private float intervaloMax = 4f;
    [SerializeField] private int minPaquetes = 1;
    [SerializeField] private int maxPaquetes = 6; // Para Random.Range(1,6) usamos 6
    [SerializeField] private int tamanoMin = 1;
    [SerializeField] private int tamanoMax = 1024;

    // ===== REFERENCIA A UI =====
    [Header("Referencia UI")]
    [SerializeField] private UIManager uiManager;

    private void Start()
    {
        StartCoroutine(GenerarPaquetes());
    }

    IEnumerator GenerarPaquetes()
    {
        while (true)
        {
            float espera = Random.Range(intervaloMin, intervaloMax);
            yield return new WaitForSeconds(espera);

            int cantidad = Random.Range(minPaquetes, maxPaquetes);
            for (int i = 0; i < cantidad; i++)
            {
                int tamano = Random.Range(tamanoMin, tamanoMax + 1);
                PaqueteDato nuevoPaquete = new PaqueteDato(tamano, Time.time);
                colaProcesamiento.Enqueue(nuevoPaquete);
            }

            // Actualizar UI después de generar
            if (uiManager != null)
            {
                uiManager.ActualizarContadorCola(colaProcesamiento.Count);
                uiManager.ActualizarSaturacion(colaProcesamiento.Count); // <-- AÑADIDO
            }
        }
    }

    public void ProcesarSiguiente()
    {
        if (colaProcesamiento.Count == 0)
        {
            Debug.Log("No hay paquetes en la cola.");
            return;
        }

        PaqueteDato paquete = colaProcesamiento.Dequeue();
        float espera = Time.time - paquete.TiempoLlegada;

        // Verificación obligatoria con ContainsKey
        if (!historialProcesados.ContainsKey(paquete.Id))
        {
            historialProcesados.Add(paquete.Id, paquete);
        }
        else
        {
            Debug.LogWarning("ID duplicado (no debería ocurrir con UUID). No se agregó al historial.");
        }

        // Actualizar métricas
        sumaTiemposEspera += espera;
        totalProcesados++;

        // Actualizar UI con todos los elementos
        if (uiManager != null)
        {
            uiManager.ActualizarContadorCola(colaProcesamiento.Count);
            uiManager.ActualizarUltimoProcesado(paquete, espera);
            uiManager.ActualizarTotalHistorico(totalProcesados); // o historialProcesados.Count
            uiManager.ActualizarPromedio(TiempoPromedioEspera);  // <-- AÑADIDO (obligatorio)
            uiManager.ActualizarSaturacion(colaProcesamiento.Count);
        }
    }
}