using UnityEngine;

public class TerminalAbrirPortao : MonoBehaviour
{
    [Header("Portão a ser aberto")]
    public Portao portao;

    public void AtivarTerminal()
    {
        if (portao != null)
            portao.Abrir();
    }
}