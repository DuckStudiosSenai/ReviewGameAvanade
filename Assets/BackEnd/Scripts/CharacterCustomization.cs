using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCustomization : MonoBehaviourPunCallbacks
{
    [Header("Animators")]
    public Animator bodyAnimator;
    public Animator hairAnimator;
    public Animator eyesAnimator;
    public Animator clothesAnimator;
    public Animator backAccessoryAnimator;

    [Header("Appearance Options")]
    public RuntimeAnimatorController[] bodyOptions;
    public RuntimeAnimatorController[] hairOptions;
    public RuntimeAnimatorController[] eyesOptions;
    public RuntimeAnimatorController[] clothesOptions;
    public RuntimeAnimatorController[] backAccessoryOptions;

    [Header("Shop Items")]
    public ShopCosmeticItem[] bodyShopItems;
    public ShopCosmeticItem[] hairShopItems;
    public ShopCosmeticItem[] eyesShopItems;
    public ShopCosmeticItem[] clothesShopItems;
    public ShopCosmeticItem[] backAccessoryShopItems;

    private int bodyIndex = 0;
    private int hairIndex = 0;
    private int eyesIndex = 0;
    private int clothesIndex = 0;
    private int backAccessoryIndex = 0;

    private int lastHash;
    private PhotonView pv;

    private int userId;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            userId = FindAnyObjectByType<PlayFabMainMenu>().GetUserId();
        }
        else
        {
            userId = FindAnyObjectByType<PlayFabManager>().GetUserId();
        }
        Debug.Log("UserId: " + userId);

        pv = GetComponent<PhotonView>();
        Debug.Log("PV: " + pv);

        if (!pv.IsMine)
        {
            ApplyRemoteCustomization(pv.Owner.CustomProperties);
            bodyAnimator.SetTrigger("IdleDown");
            return;
        }

        LoadCustomization();

        int pending = 5; // body, hair, eyes, clothes, backAccessory

        void OnCategoryLoaded()
        {
            pending--;

            if (pending == 0)
            {
                ApplyLoadedOptions();
                SendCustomizationToOthers();
                bodyAnimator.SetTrigger("IdleDown");
            }
        }

        AddShopItems(bodyOptions, bodyShopItems, result =>
        {
            bodyOptions = result;
            OnCategoryLoaded();
        });

        AddShopItems(hairOptions, hairShopItems, result =>
        {
            hairOptions = result;
            OnCategoryLoaded();
        });

        AddShopItems(eyesOptions, eyesShopItems, result =>
        {
            eyesOptions = result;
            OnCategoryLoaded();
        });

        AddShopItems(clothesOptions, clothesShopItems, result =>
        {
            clothesOptions = result;
            OnCategoryLoaded();
        });

        AddShopItems(backAccessoryOptions, backAccessoryShopItems, result =>
        {
            backAccessoryOptions = result;
            OnCategoryLoaded();
        });
    }


    void Update()
    {
        // Sincroniza estados do Animator para todos (opcional)
        AnimatorStateInfo state = bodyAnimator.GetCurrentAnimatorStateInfo(0);
        if (state.fullPathHash == lastHash) return;
        lastHash = state.fullPathHash;

        SyncAnimator(hairAnimator, state);
        SyncAnimator(clothesAnimator, state);
    }

    void SyncAnimator(Animator anim, AnimatorStateInfo state)
    {
        anim.Play(state.fullPathHash, 0, state.normalizedTime);
    }

    // ----------------- SAVE / LOAD -----------------

    public void SaveCustomization()
    {
        PlayerPrefs.SetInt("BodyIndex", bodyIndex);
        PlayerPrefs.SetInt("HairIndex", hairIndex);
        PlayerPrefs.SetInt("EyesIndex", eyesIndex);
        PlayerPrefs.SetInt("ClothesIndex", clothesIndex);
        PlayerPrefs.SetInt("BackAccessoryIndex", backAccessoryIndex);
        PlayerPrefs.Save();
        SendCustomizationToOthers();
    }

    public void LoadCustomization()
    {
        bodyIndex = PlayerPrefs.GetInt("BodyIndex", 0);
        hairIndex = PlayerPrefs.GetInt("HairIndex", 0);
        eyesIndex = PlayerPrefs.GetInt("EyesIndex", 0);
        clothesIndex = PlayerPrefs.GetInt("ClothesIndex", 0);
        backAccessoryIndex = PlayerPrefs.GetInt("BackAccessoryIndex", 0);
    }

    public void ApplyLoadedOptions()
    {
        if (bodyOptions.Length > 0)
        {
            bodyIndex = ClampIndex(bodyIndex, bodyOptions.Length);
            bodyAnimator.runtimeAnimatorController = bodyOptions[bodyIndex];
        }

        if (hairOptions.Length > 0)
        {
            hairIndex = ClampIndex(hairIndex, hairOptions.Length);
            hairAnimator.runtimeAnimatorController = hairOptions[hairIndex];
        }

        if (eyesOptions.Length > 0)
        {
            eyesIndex = ClampIndex(eyesIndex, eyesOptions.Length);
            eyesAnimator.runtimeAnimatorController = eyesOptions[eyesIndex];
        }

        if (clothesOptions.Length > 0)
        {
            clothesIndex = ClampIndex(clothesIndex, clothesOptions.Length);
            clothesAnimator.runtimeAnimatorController = clothesOptions[clothesIndex];
        }

        if (backAccessoryOptions.Length > 0)
        {
            backAccessoryIndex = ClampIndex(backAccessoryIndex, backAccessoryOptions.Length);
            backAccessoryAnimator.runtimeAnimatorController = backAccessoryOptions[backAccessoryIndex];
        }
    }


    // ----------------- PHOTON SYNC -----------------

    void SendCustomizationToOthers()
    {
        Hashtable data = new Hashtable
        {
            { "BodyIndex", bodyIndex },
            { "HairIndex", hairIndex },
            { "EyesIndex", eyesIndex },
            { "ClothesIndex", clothesIndex },
            { "BackAccessoryIndex", backAccessoryIndex }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(data);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (pv.Owner != targetPlayer) return;

        if (changedProps.ContainsKey("BodyIndex"))
        {
            ApplyRemoteCustomization(targetPlayer.CustomProperties);
        }
    }

    void ApplyRemoteCustomization(Hashtable props)
    {
        if (props.ContainsKey("BodyIndex"))
            bodyAnimator.runtimeAnimatorController = bodyOptions[(int)props["BodyIndex"]];
        if (props.ContainsKey("HairIndex"))
            hairAnimator.runtimeAnimatorController = hairOptions[(int)props["HairIndex"]];
        if (props.ContainsKey("EyesIndex"))
            eyesAnimator.runtimeAnimatorController = eyesOptions[(int)props["EyesIndex"]];
        if (props.ContainsKey("ClothesIndex"))
            clothesAnimator.runtimeAnimatorController = clothesOptions[(int)props["ClothesIndex"]];
        if (props.ContainsKey("BackAccessoryIndex"))
            backAccessoryAnimator.runtimeAnimatorController = backAccessoryOptions[(int)props["BackAccessoryIndex"]];
    }

    // ----------------- BUTTONS (somente local) -----------------

    public void NextBody() { bodyIndex = (bodyIndex + 1) % bodyOptions.Length; bodyAnimator.runtimeAnimatorController = bodyOptions[bodyIndex]; SendCustomizationToOthers(); }
    public void PreviousBody() { bodyIndex = (bodyIndex - 1 + bodyOptions.Length) % bodyOptions.Length; bodyAnimator.runtimeAnimatorController = bodyOptions[bodyIndex]; SendCustomizationToOthers(); }

    public void NextHair() { hairIndex = (hairIndex + 1) % hairOptions.Length; hairAnimator.runtimeAnimatorController = hairOptions[hairIndex]; SendCustomizationToOthers(); }
    public void PreviousHair() { hairIndex = (hairIndex - 1 + hairOptions.Length) % hairOptions.Length; hairAnimator.runtimeAnimatorController = hairOptions[hairIndex]; SendCustomizationToOthers(); }

    public void NextEyes() { eyesIndex = (eyesIndex + 1) % eyesOptions.Length; eyesAnimator.runtimeAnimatorController = eyesOptions[eyesIndex]; SendCustomizationToOthers(); }
    public void PreviousEyes() { eyesIndex = (eyesIndex - 1 + eyesOptions.Length) % eyesOptions.Length; eyesAnimator.runtimeAnimatorController = eyesOptions[eyesIndex]; SendCustomizationToOthers(); }

    public void NextClothes() { clothesIndex = (clothesIndex + 1) % clothesOptions.Length; clothesAnimator.runtimeAnimatorController = clothesOptions[clothesIndex]; SendCustomizationToOthers(); }
    public void PreviousClothes() { clothesIndex = (clothesIndex - 1 + clothesOptions.Length) % clothesOptions.Length; clothesAnimator.runtimeAnimatorController = clothesOptions[clothesIndex]; SendCustomizationToOthers(); }

    public void NextBackAccessory() { backAccessoryIndex = (backAccessoryIndex + 1) % backAccessoryOptions.Length; backAccessoryAnimator.runtimeAnimatorController = backAccessoryOptions[backAccessoryIndex]; SendCustomizationToOthers(); }
    public void PreviousBackAccessory() { backAccessoryIndex = (backAccessoryIndex - 1 + backAccessoryOptions.Length) % backAccessoryOptions.Length; backAccessoryAnimator.runtimeAnimatorController = backAccessoryOptions[backAccessoryIndex]; SendCustomizationToOthers(); }

    void AddShopItems(
     RuntimeAnimatorController[] baseOptions,
     ShopCosmeticItem[] shopItems,
     System.Action<RuntimeAnimatorController[]> onComplete
 )
    {
        var list = new System.Collections.Generic.List<RuntimeAnimatorController>(baseOptions);

        if (shopItems.Length == 0)
        {
            onComplete?.Invoke(list.ToArray());
            return;
        }

        int pending = shopItems.Length;

        foreach (var item in shopItems)
        {
            ShopManager.PlayerHasShopItem(item.itemId, userId, owns =>
            {
                if (owns && item.animator != null)
                {
                    list.Add(item.animator);
                    Debug.Log("Item liberado: " + item.itemId);
                }

                pending--;

                if (pending == 0)
                {
                    onComplete?.Invoke(list.ToArray());
                }
            });
        }
    }



    int ClampIndex(int index, int length)
    {
        if (length <= 0) return 0;
        return Mathf.Clamp(index, 0, length - 1);
    }

}
