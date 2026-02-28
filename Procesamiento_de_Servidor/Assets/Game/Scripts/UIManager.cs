using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    
    public TMP_Text txtContadorCola;          
    public TMP_Text txtUltimoProcesado;       
    public TMP_Text txtTotalHistorico;        
    public TMP_Text txtPromedioEspera;        
    public TMP_Text txtSaturacion;            
    public Image panelSaturacion;             

    
    public TMP_InputField inputBuscarID;      
    public TMP_Text txtResultadoBusqueda;     

    
    public Color colorNormal = Color.white;
    public Color colorSaturado = Color.red;

    private ServidorManager servidorManager;

    private void Start()
    {
        servidorManager = FindObjectOfType<ServidorManager>();

        if (txtUltimoProcesado != null)
            txtUltimoProcesado.gameObject.SetActive(false);

        if (txtResultadoBusqueda != null)
            txtResultadoBusqueda.text = "";
    }

    public void ActualizarContadorCola(int count)
    {
        if (txtContadorCola != null)
            txtContadorCola.text = "Paquetes en cola: " + count;
        else
            Debug.LogWarning("txtContadorCola no asignado");
    }

  
    public void ActualizarUltimoProcesado(PaqueteDato paquete, float tiempoEspera)
    {
        if (txtUltimoProcesado != null)
        {
            txtUltimoProcesado.gameObject.SetActive(true);
            txtUltimoProcesado.text =
                $"ID: {paquete.Id}\nTamaño: {paquete.TamanoCarga}\nEspera: {tiempoEspera:F2} s";
        }
    }

    public void ActualizarTotalHistorico(int total)
    {
        if (txtTotalHistorico != null)
            txtTotalHistorico.text = "Procesados: " + total;
        else
            Debug.LogWarning("txtTotalHistorico no asignado");
    }

    public void ActualizarPromedio(float promedio)
    {
        if (txtPromedioEspera != null)
            txtPromedioEspera.text = "Promedio: " + promedio.ToString("F2") + " s";
        else
            Debug.LogWarning("txtPromedioEspera no asignado");
    }


    public void ActualizarSaturacion(int colaCount)
    {
        bool saturado = colaCount > 20;

        if (txtSaturacion != null)
        {
            txtSaturacion.gameObject.SetActive(saturado);
            txtSaturacion.text = "SERVIDOR SATURADO";
            txtSaturacion.color = colorSaturado;
        }

        if (panelSaturacion != null)
        {
            panelSaturacion.color = saturado ? colorSaturado : colorNormal;
        }
    }

   
    public void BuscarPorID()
    {
        if (servidorManager == null || inputBuscarID == null)
            return;

        string idBuscado = inputBuscarID.text.Trim();

        if (string.IsNullOrEmpty(idBuscado))
        {
            txtResultadoBusqueda.text = "Ingrese un ID válido.";
            return;
        }

        if (servidorManager.historialProcesados.ContainsKey(idBuscado))
        {
            PaqueteDato paquete = servidorManager.historialProcesados[idBuscado];
            txtResultadoBusqueda.text = "Encontrado\nTamaño: " + paquete.TamanoCarga;
        }
        else
        {
            txtResultadoBusqueda.text = "No existe un paquete con ese ID.";
        }
    }
}