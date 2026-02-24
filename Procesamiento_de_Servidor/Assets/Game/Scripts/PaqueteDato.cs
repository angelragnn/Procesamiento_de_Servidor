using System;

[System.Serializable]
public class PaqueteDato
{
    // Campos privados
    public string id;
    public int tamanoCarga;
    public float tiempoLlegada;

    // Propiedades públicas
    public string Id { get => id; set => id = value; }
    public int TamanoCarga { get => tamanoCarga; set => tamanoCarga = value; }
    public float TiempoLlegada { get => tiempoLlegada; set => tiempoLlegada = value; }

    // Constructor: recibe tamaño y tiempo, genera ID único
    public PaqueteDato(int tamanoCarga, float tiempoLlegada)
    {
        Id = Guid.NewGuid().ToString();
        TamanoCarga = tamanoCarga;
        TiempoLlegada = tiempoLlegada;
    }
}