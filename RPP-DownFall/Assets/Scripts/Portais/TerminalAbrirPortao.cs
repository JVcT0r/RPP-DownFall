using UnityEngine;

public class TerminalAbrirPortao : MonoBehaviour
{
    [Header("Portão a ser aberto")]
    public Portao portao;

    [Header("UI - Pop-up do E")]
    public GameObject popupE;   
    
    public void MostrarPopup(bool mostrar)
    {
        if (popupE != null)
            popupE.SetActive(mostrar);
    }

    public void AtivarTerminal()
    {
        if (portao != null)
            portao.Abrir();
    }
}