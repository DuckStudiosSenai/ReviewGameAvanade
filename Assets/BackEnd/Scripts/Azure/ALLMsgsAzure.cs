using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using TMPro;
using System;
using Photon.Pun;

public class ALLMsgsAzure : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI[] textOutputs;
    public UnityEngine.UI.Button sendButton;

    public string apiUrl = "https://reviewgameapi.squareweb.app/api/ChatBot/respond";

    private bool apiOnline = false;

    public string playerId;

    private PlayFabManager playfab;

    private void Start()
    {
        playfab = FindAnyObjectByType<PlayFabManager>();

        StartCoroutine(WaitForPlayerId());

        foreach (var text in textOutputs)
            text.text = "Verificando conexão...";

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

                sendButton.interactable = true;
            }
            else
            {
                apiOnline = false;
                foreach (var text in textOutputs)
                    text.text = "⚠️ Erro: servidor indisponível.";

                sendButton.interactable = false;
            }
        }
    }

    public void OnSendButton()
    {
        if (!apiOnline) return;

        string message = inputField.text.Trim();
        if (string.IsNullOrEmpty(message)) return;

        foreach (var text in textOutputs)
            text.text = "Digitando...";

        SendMessage(message, response =>
        {
            foreach (var text in textOutputs)
                text.text = response;
        });
    }

    public void SendMessage(string userMessage, Action<string> callback)
    {
        StartCoroutine(Request(userMessage, callback));
    }

    private IEnumerator Request(string userMessage, Action<string> callback)
    {
        var jsonBody = new JObject
        {
            ["playerId"] = playerId,
            ["message"] = userMessage,
            ["context"] = "topdown"
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
                var responseJson = JObject.Parse(request.downloadHandler.text);
                string resposta = responseJson["message"]?.ToString() ?? "Erro ao interpretar resposta.";
                callback?.Invoke(resposta);
            }
            else
            {
                callback?.Invoke("Erro de conexão: " + request.error);
            }
        }
    }

    private IEnumerator WaitForPlayerId()
    {
        playfab = FindAnyObjectByType<PlayFabManager>();

        while (playfab == null || playfab.GetUserId() == 0)
        {
            yield return null;
        }

        playerId = playfab.GetUserId().ToString();
        Debug.Log("PlayerID carregado: " + playerId);

        foreach (var text in textOutputs)
            text.text = "Verificando conexão...";

        sendButton.interactable = false;

        StartCoroutine(TestarConexao());
    }

}
