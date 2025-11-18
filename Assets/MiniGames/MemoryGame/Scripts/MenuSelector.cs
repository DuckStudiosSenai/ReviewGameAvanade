using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuSelector : MonoBehaviour
{
    public RectTransform[] opcoes;  // arraste aqui os botões/textos
    public RectTransform barra;     // a barra transparente
    public float velocidade = 10f;

    private int indiceAtual = 0;

    void Start()
    {
        AtualizarBarra();
    }

    void Update()
    {
        // Navegação com setas
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            indiceAtual = (indiceAtual + 1) % opcoes.Length;
            AtualizarBarra();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            indiceAtual = (indiceAtual - 1 + opcoes.Length) % opcoes.Length;
            AtualizarBarra();
        }

        // Confirmar seleção
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Selecionado: " + opcoes[indiceAtual].name);
        }

        // Movimento suave da barra até a opção
        barra.position = Vector3.Lerp(barra.position, opcoes[indiceAtual].position, Time.deltaTime * velocidade);
    }

    void AtualizarBarra()
    {
        // Posiciona a barra diretamente no início (sem transição)
        barra.position = opcoes[indiceAtual].position;
    }
}
