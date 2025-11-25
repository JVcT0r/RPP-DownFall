using UnityEngine;

public class Portao : MonoBehaviour
{
    [Header("Movimentação do Portão")]
    public Vector3 offsetAbertura = new Vector3(2f, 0f, 0f); // direção que vai abrir
    public float velocidade = 2f;

    private bool aberto = false;
    private Vector3 posInicial;
    private Vector3 posFinal;

    void Start()
    {
        posInicial = transform.localPosition;
        posFinal = posInicial + offsetAbertura;
    }

    public void Abrir()
    {
        if (aberto) return;
        aberto = true;
        StartCoroutine(Mover());
    }

    private System.Collections.IEnumerator Mover()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * velocidade;
            transform.localPosition = Vector3.Lerp(posInicial, posFinal, t);
            yield return null;
        }
    }
}