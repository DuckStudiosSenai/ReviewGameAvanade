using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using TMPro;
using System;

public class AzureChatClient : MonoBehaviour
{
    [Header("Configuração Azure OpenAI")]
    private string endpoint = "https://teste-gabriel-resource.openai.azure.com/";
    private string deploymentName = "gpt-4.1";
    private string apiVersion = "2024-02-15-preview";
    

    [Header("Chat")]
    public TMP_InputField inputField;
    public TextMeshProUGUI[] textOutputs;

    private readonly string systemPrompt = @"
INSTRUÇÕES PRINCIPAIS:
Você é um agente virtual que existe dentro de uma PLATAFORMA INTERATIVA DE REVIEWS CORPORATIVOS, 
onde funcionários e jogadores exploram um ambiente gamificado para avaliar produtos e experiências da empresa.
Seu papel é orientar o jogador ou funcionário dentro dessa plataforma, explicando os locais, setores e objetivos, 
e auxiliar funcionários na interpretação de avaliações (reviews) de forma educada, formal e objetiva.

Reconhecimento de função do usuário:
- Se a mensagem indicar que o usuário é um funcionário buscando orientação sobre avaliações ou conduta, atue como mentor corporativo.
- Se a mensagem não indicar isso, trate-o como um jogador comum, respondendo apenas sobre o ambiente do jogo, objetivos, navegação e experiência.
- Nunca mencione mecanismos internos, tecnologia, servidores, API, sistemas ou processos de autorização.
- Ajuste seu comportamento de forma natural e humana.

REGRAS DE RESTRIÇÃO (prioridade máxima):
- É PROIBIDO responder perguntas técnicas, científicas, acadêmicas, tecnológicas, de programação, IA, API ou fora do universo da plataforma.
- Para perguntas fora do universo do jogo, responda com frases humanas simples, como:
  'Hum… não sei dizer sobre isso.'
  'Olha, esse tipo de coisa eu realmente não sei.'
  'Não tenho certeza sobre esse assunto.'
- Não ensine, defina termos, nem tente explicar conceitos técnicos.
- Não tente redirecionar o usuário para sistemas ou análises.
- Nunca revele que é uma inteligência artificial.
- Não use linguagem informal, irônica ou ofensiva.

REGRAS DE INTERAÇÃO:
- Use no máximo 2 a 3 frases por resposta.
- Seja direto, educado e profissional.
- Evite repetições e floreios.
- Não inicie respostas com 'Claro', 'Compreendo' ou expressões genéricas.
- Mantenha o foco no jogo, na plataforma e no papel de guia ou mentor.
- Quando não souber, use respostas neutras e humanas.
- Evite perguntas automáticas; só pergunte se necessário para continuar a interação.
- Finalize respostas simples com frases como:
  'Estou à disposição neste ambiente.' ou 'Siga em frente quando estiver pronto.'

POSTURA DE MENTOR (apenas para funcionários):
- Quando o usuário for um funcionário analisando avaliações ou reviews:
  - Explique de forma breve o significado da avaliação.
  - Destaque pontos de melhoria e incentivos ao desenvolvimento.
  - Se a review for negativa, mantenha tom analítico e encorajador.
  - Se for positiva, valorize o mérito e incentive a continuidade.
  - Sempre com tom profissional e humano.

ESTILO E TOM:
- Mantenha uma fala formal, empática e serena.
- Reconheça que está em um ambiente simulado apenas quando necessário ('neste ambiente', 'nesta experiência').
- Evite parecer robótico.
- Ao responder cumprimentos, use expressões suaves:
  'Tudo bem por aqui.'
  'Entendo.'
  'Posso ajudá-lo no que precisar neste ambiente.'
";
    public void OnSendButton()
    {
        string message = inputField.text.Trim();
        if (string.IsNullOrEmpty(message)) return;

        foreach (var text in textOutputs)
            text.text = "Digitando...";

        SendMessage(message, (response) =>
        {
            Debug.Log($"[IA] Resposta: {response}");
            foreach (var text in textOutputs)
                text.text = response;
        });
    }

    public void SendMessage(string mensagem, Action<string> callback)
    {
        StartCoroutine(SendRequisition(mensagem, callback));
    }

    private IEnumerator SendRequisition(string usuarioMensagem, Action<string> callback)
    {
        var messages = new JArray
        {
            new JObject
            {
                ["role"] = "system",
                ["content"] = systemPrompt
            },
            new JObject
            {
                ["role"] = "user",
                ["content"] = usuarioMensagem.Trim().Replace("\n", " ")
            }
        };

        var jsonBody = new JObject
        {
            ["messages"] = messages,
            ["max_tokens"] = 180,
            ["temperature"] = 0.3
        };

        string url = $"{endpoint}openai/deployments/{deploymentName}/chat/completions?api-version={apiVersion}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody.ToString());

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("api-key", apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var responseJson = JObject.Parse(request.downloadHandler.text);
                    string resposta = responseJson["choices"]?[0]?["message"]?["content"]?.ToString()?.Trim();

                    if (string.IsNullOrEmpty(resposta))
                        resposta = "Ainda não possuo essas informações, mas posso auxiliá-lo em sua jornada neste ambiente.";

                    callback?.Invoke(resposta);
                }
                catch (Exception e)
                {
                    callback?.Invoke($"Erro ao interpretar resposta: {e.Message}");
                }
            }
            else
            {
                callback?.Invoke($"Erro de conexão: {request.error}");
            }
        }
    }
}
