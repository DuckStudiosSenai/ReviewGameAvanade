using UnityEngine;

public class CameraIdleMotion : MonoBehaviour
{
    public float intensidade = 0.2f;   // quão forte vai balançar
    public float velocidade = 0.5f;    // velocidade do movimento

    private Vector3 posicaoInicial;

    void Start()
    {
        posicaoInicial = transform.localPosition;
    }

    void Update()
    {
        float x = Mathf.PerlinNoise(Time.time * velocidade, 0f) - 0.5f;
        float y = Mathf.PerlinNoise(0f, Time.time * velocidade) - 0.5f;

        Vector3 deslocamento = new Vector3(x, y, 0f) * intensidade;
        transform.localPosition = posicaoInicial + deslocamento;
    }
}
    