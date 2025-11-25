using UnityEngine;

public class PortaSaida : MonoBehaviour
{
    public enum DirecaoAbertura
    {
        Cima,
        Baixo,
        Direita,
        Esquerda
    }

    [Header("Movimento da Porta")]
    public DirecaoAbertura direcao = DirecaoAbertura.Cima;
    public float distanciaAbrir = 1.5f;
    public float velocidade = 2f;
    public bool aberta = false;

    private Vector3 posicaoInicial;
    private Vector3 posicaoFinal;

    private void Start()
    {
        posicaoInicial = transform.localPosition;

        // escolher direção
        Vector3 deslocamento = Vector3.zero;

        switch (direcao)
        {
            case DirecaoAbertura.Cima:
                deslocamento = new Vector3(0, distanciaAbrir, 0);
                break;

            case DirecaoAbertura.Baixo:
                deslocamento = new Vector3(0, -distanciaAbrir, 0);
                break;

            case DirecaoAbertura.Direita:
                deslocamento = new Vector3(distanciaAbrir, 0, 0);
                break;

            case DirecaoAbertura.Esquerda:
                deslocamento = new Vector3(-distanciaAbrir, 0, 0);
                break;
        }

        posicaoFinal = posicaoInicial + deslocamento;
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