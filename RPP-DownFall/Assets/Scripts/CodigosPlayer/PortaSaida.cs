using UnityEngine;

public class PortaSaida : MonoBehaviour
{
    public Animator animPorta;         
    public GameObject portalTrigger;   

    private bool aberta = false;

    public void AbrirPorta(Player player)
    {
        if (aberta) return;

        if (player.temChave)
        {
            aberta = true;
            Debug.Log("[PORTA] Porta destrancada!");

            if (animPorta != null)
                animPorta.SetTrigger("Abrir");

            if (portalTrigger != null)
                portalTrigger.SetActive(true); // agora o portal funciona
        }
        else
        {
            Debug.Log("[PORTA] Você precisa do cartão para abrir.");
        }
    }
}