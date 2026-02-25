using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ServidorManager : MonoBehaviour
{
    // ===== ESTRUCTURAS PRINCIPALES =====
    public Queue<PaqueteDato> colaProcesamiento = new Queue<PaqueteDato>();
    public Dictionary<string, PaqueteDato> historialProcesados = new Dictionary<string, PaqueteDato>();

    // ===== MÉTRICAS =====
    private float sumaTiemposEspera = 0f;

    public float TiempoPromedioEspera
    {
        get
        {
            if (historialProcesados.Count > 0)
                return sumaTiemposEspera / historialProcesados.Count;
            else
                return 0f;
        }
    }

    // ===== CONFIGURACIÓN GENERACIÓN =====
    [Header("Configuración de generación")]
    [SerializeField] private float intervaloMin = 2f;
    [SerializeField] private float intervaloMax = 4f;
    [SerializeField] private int minPaquetes = 1;
    [SerializeField] private int maxPaquetes = 6;
    [SerializeField] private int tamanoMin = 1;
    [SerializeField] private int tamanoMax = 1024;

    // ===== REFERENCIA A UI =====
    [Header("Referencia UI")]
    [SerializeField] private UIManager uiManager;

    // ===== PERSISTENCIA =====
    private string rutaArchivo => Path.Combine(Application.streamingAssetsPath, "datosServidor.json");

    private void Start()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
            if (uiManager == null)
                Debug.LogError("No se encontró UIManager en la escena. Asigna la referencia manualmente.");
        }

        CargarEstado();
        StartCoroutine(GenerarPaquetes());
    }

    private void OnApplicationQuit()
    {
        GuardarEstado();
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

            if (uiManager != null)
            {
                uiManager.ActualizarContadorCola(colaProcesamiento.Count);
                uiManager.ActualizarSaturacion(colaProcesamiento.Count);
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

        if (!historialProcesados.ContainsKey(paquete.Id))
        {
            historialProcesados.Add(paquete.Id, paquete);
            sumaTiemposEspera += espera;
        }
        else
        {
            Debug.LogWarning("ID duplicado (no debería ocurrir). El paquete no se almacenó en el historial.");
        }

        if (uiManager != null)
        {
            uiManager.ActualizarContadorCola(colaProcesamiento.Count);
            uiManager.ActualizarUltimoProcesado(paquete, espera);
            uiManager.ActualizarTotalHistorico(historialProcesados.Count);
            uiManager.ActualizarPromedio(TiempoPromedioEspera);
            uiManager.ActualizarSaturacion(colaProcesamiento.Count);
        }
    }

    [ContextMenu("Guardar Estado")]
    public void GuardarEstado()
    {
        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);

        DatosServidor datos = new DatosServidor();
        datos.colaList = new List<PaqueteDato>(colaProcesamiento);
        datos.historialList = new List<PaqueteDato>(historialProcesados.Values);
        datos.sumaTiemposEspera = sumaTiemposEspera;

        string json = JsonUtility.ToJson(datos, true);
        File.WriteAllText(rutaArchivo, json);
        Debug.Log("Estado guardado en: " + rutaArchivo);
    }

    [ContextMenu("Cargar Estado")]
    public void CargarEstado()
    {
        if (!File.Exists(rutaArchivo))
        {
            Debug.Log("No hay archivo de guardado previo.");
            return;
        }

        string json = File.ReadAllText(rutaArchivo);
        DatosServidor datos = JsonUtility.FromJson<DatosServidor>(json);

        colaProcesamiento.Clear();
        foreach (var p in datos.colaList)
            colaProcesamiento.Enqueue(p);

        historialProcesados.Clear();
        foreach (var p in datos.historialList)
        {
            if (!historialProcesados.ContainsKey(p.Id))
                historialProcesados.Add(p.Id, p);
        }

        sumaTiemposEspera = datos.sumaTiemposEspera;

        if (uiManager != null)
        {
            uiManager.ActualizarContadorCola(colaProcesamiento.Count);
            uiManager.ActualizarTotalHistorico(historialProcesados.Count);
            uiManager.ActualizarPromedio(TiempoPromedioEspera);
            uiManager.ActualizarSaturacion(colaProcesamiento.Count);
        }

        Debug.Log("Estado cargado desde: " + rutaArchivo);
    }

    public PaqueteDato BuscarPorId(string id)
    {
        if (historialProcesados.ContainsKey(id))
            return historialProcesados[id];
        else
            return null;
    }
}

[System.Serializable]
public class DatosServidor
{
    public List<PaqueteDato> colaList;
    public List<PaqueteDato> historialList;
    public float sumaTiemposEspera;
}