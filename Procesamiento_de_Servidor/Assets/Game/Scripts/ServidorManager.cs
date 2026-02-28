using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[SerializeField]
public class ServidorManager : MonoBehaviour
{
    
    public Queue<PaqueteDato> colaProcesamiento = new Queue<PaqueteDato>();
    public Dictionary<string, PaqueteDato> historialProcesados = new Dictionary<string, PaqueteDato>();

    
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

    
    
    private float intervaloMin = 2f;
    private float intervaloMax = 4f;
    private int minPaquetes = 1;
    private int maxPaquetes = 6;
    private int tamanoMin = 1;
    private int tamanoMax = 1024;

    
    
    private UIManager uiManager;

    
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

    
    public void CargarEstado()
    {
        if (!File.Exists(rutaArchivo))
        {
            Debug.Log("No hay archivo de guardado previo.");
            return;
        }

        string json = File.ReadAllText(rutaArchivo);
        DatosServidor datos = JsonUtility.FromJson<DatosServidor>(json);

        foreach (var p in datos.colaList)
        {
            p.TiempoLlegada = Time.time;
            colaProcesamiento.Enqueue(p);
        }

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


public class DatosServidor
{
    public List<PaqueteDato> colaList;
    public List<PaqueteDato> historialList;
    public float sumaTiemposEspera;
}