using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using TMPro;
using System;

public class AzureChatClient : MonoBehaviour
{
    [Header("Chat")]
    public TMP_InputField inputField;
    public TextMeshProUGUI[] textOutputs;
    public UnityEngine.UI.Button sendButton;

    [Header("API Endpoint")]
    public string apiUrl = "https://reviewgameapi.squareweb.app/api/ChatBot/respond";

    private bool apiOnline = false;

    private void Start()
    {
        foreach (var text in textOutputs)
            text.text = "Verificando conexão...";

        if (sendButton != null)
            sendButton.interactable = false;

        StartCoroutine(TestarConexao());
    }

    private IEnumerator TestarConexao()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl.Replace("/respond", "")))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success ||
                request.responseCode == 404 || request.responseCode == 405)
            {
                apiOnline = true;
                foreach (var text in textOutputs)
                    text.text = "✅ Conectado! Pronto para conversar.";

                if (sendButton != null)
                    sendButton.interactable = true;

                Debug.Log("✅ API online e acessível.");
            }
            else
            {
                apiOnline = false;
                foreach (var text in textOutputs)
                    text.text = "⚠️ Erro: servidor indisponível.";

                if (sendButton != null)
                    sendButton.interactable = false;

                Debug.LogError($"❌ Falha ao conectar na API: {request.error}");
            }
        }
    }

    public void OnSendButton()
    {
        if (!apiOnline)
        {
            foreach (var text in textOutputs)
                text.text = "⚠️ Servidor ainda não está pronto.";
            return;
        }

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
        var jsonBody = new JObject
        {
            ["message"] = usuarioMensagem.Trim(),
            ["content"] = "topdown"
        };

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody.ToString());

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var responseJson = JObject.Parse(request.downloadHandler.text);
                    string resposta = responseJson["message"]?.ToString()?.Trim();

                    if (string.IsNullOrEmpty(resposta))
                        resposta = "Ainda não possuo essas informações, mas posso auxiliá-lo neste ambiente.";

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