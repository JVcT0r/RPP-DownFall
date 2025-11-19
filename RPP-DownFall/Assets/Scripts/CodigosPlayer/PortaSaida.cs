using UnityEngine;

public class PortaSaida : MonoBehaviour
{
    [Header("Movimento da Porta")]
    public float distanciaAbrir = 1.5f;   
    public float velocidade = 2f;         
    public bool aberta = false;
    
    private Vector3 posicaoInicial;
    private Vector3 posicaoFinal;

    private void Start()
    {
        posicaoInicial = transform.localPosition;
        posicaoFinal = posicaoInicial + new Vector3(0, distanciaAbrir, 0);
        
    }

    public void AbrirPorta(Player player)
    {
        if (player == null) return;

        if (!player.temChave)
        {
            player.MostrarMensagem("Você precisa do cartão!", 2f);
            return;
        }

        if (!aberta)
        {
            aberta = true;
            StartCoroutine(MoverPorta());
        }
    }

    private System.Collections.IEnumerator MoverPorta()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * velocidade;
            transform.localPosition = Vector3.Lerp(posicaoInicial, posicaoFinal, t);
            yield return null;
        }
    }
}