using UnityEngine;
using static APIManager;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;
using TMPro;

public class PointsManager : MonoBehaviour
{
    private const string baseUrl = "https://reviewgameapi.squareweb.app/api";

    public IEnumerator GetUserPoints(int userId, Action<int, int> onSuccess, Action<string> onError)
    {
        string url = $"{baseUrl}/Users/{userId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error);
                yield break;
            }

            try
            {
                UserDto user = JsonUtility.FromJson<UserDto>(request.downloadHandler.text);
                onSuccess?.Invoke(user.points, user.currentpoints);
            }
            catch (System.Exception e)
            {
                onError?.Invoke("Erro ao converter JSON: " + e.Message);
            }
        }
    }

    public IEnumerator UpdateUserPoints(int userId, int? points, int? currentPoints,
                                    Action onSuccess, Action<string> onError)
    {
        string url = $"{baseUrl}/Users/{userId}";
        Debug.Log($"[UpdateUserPoints] URL: {url}");

        UserUpdateDto update = new UserUpdateDto
        {
            points = points,
            currentpoints = currentPoints
        };

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(update);
        Debug.Log($"[UpdateUserPoints] JSON ENVIADO: {json}");

        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            Debug.Log($"[UpdateUserPoints] Status Code: {request.responseCode}");
            Debug.Log($"[UpdateUserPoints] Response: {request.downloadHandler.text}");

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[UpdateUserPoints] ERRO: {request.error}");
                Debug.LogError($"[UpdateUserPoints] Response de erro: {request.downloadHandler.text}");

                onError?.Invoke(request.error);
                yield break;
            }

            onSuccess?.Invoke();
        }

        Debug.Log("JSON enviado: " + json);

    }
    public void AddPoints(int userId, int amount)
    {
        StartCoroutine(GetUserPoints(
            userId,
            onSuccess: (points, currentPoints) =>
            {
                int newCurrentPoints = currentPoints + amount;
                int newTotalPoints = points + amount;

                StartCoroutine(UpdateUserPoints(
                    userId,
                    points: newTotalPoints,
                    currentPoints: newCurrentPoints,
                    onSuccess: () =>
                    {
                        Debug.Log($"✅ {amount} pontos adicionados ao usuário {userId}. Total: {newTotalPoints}, Current: {newCurrentPoints}");
                    },
                    onError: (err) =>
                    {
                        Debug.LogError("Erro ao atualizar pontos: " + err);
                    }
                ));
            },
            onError: (err) =>
            {
                Debug.LogError("Erro ao buscar pontos: " + err);
            }
        ));
    }

    public void RemovePoints(int userId, int amount)
    {
        StartCoroutine(GetUserPoints(
            userId,
            onSuccess: (_, currentPoints) =>
            {
                int newCurrentPoints = Mathf.Max(0, currentPoints - amount);

                StartCoroutine(UpdateUserPoints(
                    userId,
                    points: null,
                    currentPoints: newCurrentPoints,
                    onSuccess: () =>
                    {
                        Debug.Log($"❌ {amount} pontos removidos do usuário {userId}. CurrentPoints atualizado: {newCurrentPoints}");
                    },
                    onError: (err) =>
                    {
                        Debug.LogError("Erro ao atualizar pontos: " + err);
                    }
                ));
            },
            onError: (err) =>
            {
                Debug.LogError("Erro ao buscar pontos: " + err);
            }
        ));
    }
}

[Serializable]
public class UserDto
{
    public int id;
    public string username;
    public int points;
    public int currentpoints;
}

[Serializable]
public class UserUpdateDto
{
    public int? points;
    public int? currentpoints;
}
