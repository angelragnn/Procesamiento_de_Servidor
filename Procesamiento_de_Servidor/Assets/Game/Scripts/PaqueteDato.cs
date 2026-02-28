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

    
    public PaqueteDato(int tamanoCarga, float tiempoLlegada)
    {
        
        Id = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

        TamanoCarga = tamanoCarga;
        TiempoLlegada = tiempoLlegada;
    }
}