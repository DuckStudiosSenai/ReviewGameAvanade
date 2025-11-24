using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
    [Header("Animators")]
    public Animator bodyAnimator;
    public Animator hairAnimator;
    public Animator eyesAnimator;
    public Animator clothesAnimator;
    public Animator backAccessoryAnimator;

    [Header("Opções de Aparência")]
    public RuntimeAnimatorController[] hairOptions;
    public RuntimeAnimatorController[] eyesOptions;
    public RuntimeAnimatorController[] clothesOptions;
    public RuntimeAnimatorController[] backAccessoryOptions;

    private int hairIndex = 0;
    private int eyesIndex = 0;
    private int clothesIndex = 0;
    private int backAccessoryIndex = 0;

    private int lastHash;

    void Start()
    {
        ApplyInitialOptions();

        bodyAnimator.Update(0f);
        hairAnimator.Update(0f);
        eyesAnimator.Update(0f);
        clothesAnimator.Update(0f);
        backAccessoryAnimator.Update(0f);
    }


    void ApplyInitialOptions()
    {
        if (hairOptions.Length > 0)
        {
            hairAnimator.runtimeAnimatorController = hairOptions[0];
            Debug.Log("Hair option applied: " + hairOptions[0].name);
        }
            

        if (eyesOptions.Length > 0)
            eyesAnimator.runtimeAnimatorController = eyesOptions[0];

        if (clothesOptions.Length > 0)
            clothesAnimator.runtimeAnimatorController = clothesOptions[0];

        if (backAccessoryOptions.Length > 0)
            backAccessoryAnimator.runtimeAnimatorController = backAccessoryOptions[0];
    }

    void Update()
    {
        AnimatorStateInfo state = bodyAnimator.GetCurrentAnimatorStateInfo(0);

        if (state.fullPathHash == lastHash)
            return;

        lastHash = state.fullPathHash;

        SyncAnimator(hairAnimator, state);
        SyncAnimator(eyesAnimator, state);
        SyncAnimator(clothesAnimator, state);
        SyncAnimator(backAccessoryAnimator, state);
    }

    void SyncAnimator(Animator anim, AnimatorStateInfo state)
    {
        anim.Play(state.fullPathHash, 0, state.normalizedTime);
    }

    // ------- HAIR -------
    public void NextHair()
    {
        hairIndex = (hairIndex + 1) % hairOptions.Length;
        hairAnimator.runtimeAnimatorController = hairOptions[hairIndex];
    }

    public void PreviousHair()
    {
        hairIndex = (hairIndex - 1 + hairOptions.Length) % hairOptions.Length;
        hairAnimator.runtimeAnimatorController = hairOptions[hairIndex];
    }

    // ------- EYES -------
    public void NextEyes()
    {
        eyesIndex = (eyesIndex + 1) % eyesOptions.Length;
        eyesAnimator.runtimeAnimatorController = eyesOptions[eyesIndex];
    }

    public void PreviousEyes()
    {
        eyesIndex = (eyesIndex - 1 + eyesOptions.Length) % eyesOptions.Length;
        eyesAnimator.runtimeAnimatorController = eyesOptions[eyesIndex];
    }

    // ------- CLOTHES -------
    public void NextClothes()
    {
        clothesIndex = (clothesIndex + 1) % clothesOptions.Length;
        clothesAnimator.runtimeAnimatorController = clothesOptions[clothesIndex];
    }

    public void PreviousClothes()
    {
        clothesIndex = (clothesIndex - 1 + clothesOptions.Length) % clothesOptions.Length;
        clothesAnimator.runtimeAnimatorController = clothesOptions[clothesIndex];
    }

    // ------- BACK ACCESSORY -------
    public void NextBackAccessory()
    {
        backAccessoryIndex = (backAccessoryIndex + 1) % backAccessoryOptions.Length;
        backAccessoryAnimator.runtimeAnimatorController = backAccessoryOptions[backAccessoryIndex];
    }

    public void PreviousBackAccessory()
    {
        backAccessoryIndex = (backAccessoryIndex - 1 + backAccessoryOptions.Length) % backAccessoryOptions.Length;
        backAccessoryAnimator.runtimeAnimatorController = backAccessoryOptions[backAccessoryIndex];
    }
}
