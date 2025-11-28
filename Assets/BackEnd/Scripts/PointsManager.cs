using UnityEngine;
using static APIManager;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;
using TMPro;

[System.Serializable]
public class UserUpdateDto
{
    public int? points;
    public int? currentpoints;
}

public class PointsManager : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    private PlayFabManager playfab;
    private APIManager api;

    private const string baseUrl = "https://reviewgameapi.squareweb.app/api";
    private void Start()
    {
        playfab = FindAnyObjectByType<PlayFabManager>();
        api = FindAnyObjectByType<APIManager>();

        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Test Points Activated");
            StartCoroutine(GetUserPoints(
                userId: 1,
                onSuccess: (points) =>
                {
                    Debug.Log("Pontos do jogador: " + points);
                },
                onError: (err) =>
                {
                    Debug.LogError("Erro ao buscar pontos: " + err);
                }
            ));
        }
    }

    public IEnumerator GetUserPoints(int userId, System.Action<int> onSuccess, System.Action<string> onError)
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
                onSuccess?.Invoke(user.currentpoints);
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

        UserUpdateDto update = new UserUpdateDto
        {
            points = points,
            currentpoints = currentPoints
        };

        string json = JsonUtility.ToJson(update);

        using (UnityWebRequest request = UnityWebRequest.Put(url, json))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error);
                yield break;
            }

            onSuccess?.Invoke();
        }
    }

    public void AddPoints(int userId, int amount)
    {
        StartCoroutine(GetUserPoints(
            userId,
            onSuccess: (currentPoints) =>
            {
                int newPoints = currentPoints + amount;

                StartCoroutine(UpdateUserPoints(
                    userId,
                    points: null,
                    currentPoints: newPoints,
                    onSuccess: () =>
                    {
                        Debug.Log($"✅ {amount} pontos adicionados ao usuário {userId}");
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
            onSuccess: (currentPoints) =>
            {
                int newPoints = Mathf.Max(0, currentPoints - amount);

                StartCoroutine(UpdateUserPoints(
                    userId,
                    points: null,
                    currentPoints: newPoints,
                    onSuccess: () =>
                    {
                        Debug.Log($"❌ {amount} pontos removidos do usuário {userId}");
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

    public void LoadPoints()
    {
        StartCoroutine(GetUserPoints(
                userId: playfab.GetUserId(),
                onSuccess: (points) =>
                {
                    Debug.Log("Pontos do jogador: " + points);
                    coinsText.text = points.ToString();
                },
                onError: (err) =>
                {
                    Debug.LogError("Erro ao buscar pontos: " + err);
                }
            ));
    }

}
