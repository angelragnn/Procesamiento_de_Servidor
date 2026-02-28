using System;

[System.Serializable]
public class PaqueteDato
{
    public string id;
    public int tamanoCarga;
    public float tiempoLlegada;

    public string Id { get => id; set => id = value; }
    public int TamanoCarga { get => tamanoCarga; set => tamanoCarga = value; }
    public float TiempoLlegada { get => tiempoLlegada; set => tiempoLlegada = value; }

    // Constructor: recibe tamaño y tiempo, genera ID único usando UUID
    public PaqueteDato(int tamanoCarga, float tiempoLlegada)
    {
        // Se usa la librería System.Guid para generar identificador único
        Id = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

        TamanoCarga = tamanoCarga;
        TiempoLlegada = tiempoLlegada;
    }
}