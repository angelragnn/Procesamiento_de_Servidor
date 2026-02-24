using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Textos de la interfaz (TextMeshPro)")]
    public TMP_Text txtContadorCola;          // "Paquetes en cola: 0"
    public TMP_Text txtUltimoProcesado;       // Muestra ID, tamaño y espera (todo junto)
    public TMP_Text txtTotalHistorico;        // "Procesados: 0"
    public TMP_Text txtPromedioEspera;        // "Promedio: 0.00 s"  (OBLIGATORIO)
    public TMP_Text txtSaturacion;             // "SERVIDOR SATURADO" (se activa cuando cola > 20)
    public Image panelSaturacion;               // Opcional: panel que cambia de color

    [Header("Colores")]
    public Color colorNormal = Color.white;
    public Color colorSaturado = Color.red;

    // Actualizar contador de cola
    public void ActualizarContadorCola(int count)
    {
        if (txtContadorCola != null)
            txtContadorCola.text = "Paquetes en cola: " + count;
        else
            Debug.LogWarning("txtContadorCola no asignado");
    }

    // Actualizar el texto del último procesado (todo en uno)
    public void ActualizarUltimoProcesado(PaqueteDato paquete, float tiempoEspera)
    {
        if (txtUltimoProcesado != null)
        {
            txtUltimoProcesado.text = $"ID: {paquete.Id}\nTamaño: {paquete.TamanoCarga}\nEspera: {tiempoEspera:F2} s";
            Debug.Log($"Último procesado actualizado: {paquete.Id}"); // Para depurar
        }
        else
        {
            Debug.LogWarning("txtUltimoProcesado no asignado");
        }
    }

    // Actualizar total de procesados (histórico)
    public void ActualizarTotalHistorico(int total)
    {
        if (txtTotalHistorico != null)
            txtTotalHistorico.text = "Procesados: " + total;
        else
            Debug.LogWarning("txtTotalHistorico no asignado");
    }

    // Actualizar promedio de espera (obligatorio)
    public void ActualizarPromedio(float promedio)
    {
        if (txtPromedioEspera != null)
            txtPromedioEspera.text = "Promedio: " + promedio.ToString("F2") + " s";
        else
            Debug.LogWarning("txtPromedioEspera no asignado");
    }

    // Actualizar indicador de saturación
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
}