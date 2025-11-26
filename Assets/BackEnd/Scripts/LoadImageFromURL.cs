using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class LoadImageFromURL : MonoBehaviour
{
    [System.Serializable]
    public class UserPublicDto
    {
        public int id;
        public string name;
        public string email;
        public string enterprise;
        public string avatarUrl;
    }

    public Image targetUI;
    public string imageUrl;

    [Header("Player Name")]
    public TextMeshProUGUI nameText;
    public RectTransform avatarRect;
    public RectTransform bgRect;

    public float spacing = 10f;
    public Vector2 padding = new Vector2(20f, 10f);

    private PlayFabManager playfab;

    private void Start()
    {
        playfab = FindAnyObjectByType<PlayFabManager>();

        StartCoroutine(GetUserAndLoadAvatar(playfab.GetUserId()));
    }

    void LateUpdate()
    {
        Vector2 textSize = nameText.GetPreferredValues(nameText.text);

        float totalWidth = avatarRect.sizeDelta.x + spacing + textSize.x + padding.x;
        float totalHeight = Mathf.Max(avatarRect.sizeDelta.y, textSize.y) + padding.y;

        bgRect.sizeDelta = new Vector2(totalWidth, totalHeight);

        nameText.rectTransform.sizeDelta = new Vector2(textSize.x, nameText.rectTransform.sizeDelta.y);

        avatarRect.anchoredPosition = new Vector2(avatarRect.anchoredPosition.x, 0);
        nameText.rectTransform.anchoredPosition = new Vector2(nameText.rectTransform.anchoredPosition.x, 0);
    }


    IEnumerator LoadImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            Debug.LogWarning("URL do avatar está vazia ou nula.");
            yield break;
        }

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro ao baixar imagem: " + www.error + " - URL: " + imageUrl);
            yield break;
        }

        Texture2D tex = DownloadHandlerTexture.GetContent(www);

        targetUI.sprite = Sprite.Create(tex, new Rect(0,0,tex.width,tex.height), new Vector2(0.5f,0.5f));
    }

    IEnumerator GetUserAndLoadAvatar(int userId)
    {
        string url = $"https://reviewgameapi.squareweb.app/api/Users/{userId}";
        UnityWebRequest req = UnityWebRequest.Get(url);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro: " + req.error);
            yield break;
        }

        UserPublicDto user = JsonUtility.FromJson<UserPublicDto>(req.downloadHandler.text);

        if (!string.IsNullOrEmpty(user.avatarUrl))
        {
            user.avatarUrl = user.avatarUrl.Replace("http://", "https://");
        }

        StartCoroutine(LoadImage(user.avatarUrl));
    }


}
