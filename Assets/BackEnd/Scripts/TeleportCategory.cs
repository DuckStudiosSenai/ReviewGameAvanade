using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class TeleportCategory : MonoBehaviour
{
    public enum ProductCategory
    {
        DADOS_IA,
        NUVEM_E_PLATAFORMAS,
        SEGURANCA,
        TECNOLOGIA_INOVACAO,
        AVANADE,
        OUTROS
    }

    public GameObject notAllowed;

    [Header("Category")]
    public ProductCategory productCategory;

    [Header("Teleport Positions")]
    public Transform dataLocation;
    public Transform cloudLocation;
    public Transform securityLocation;
    public Transform techLocation;
    public Transform avanadeLocation;
    public Transform othersLocation;

    [Header("Audio")]
    public AudioClip openDoorSound;
    public AudioClip closeDoorSound;
    private AudioSource audioSource;

    private PlayFabManager playfab;

    private void Start()
    {
        playfab = FindAnyObjectByType<PlayFabManager>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();

        if (pm == null || !pm.photonView.IsMine)
            return;

        if (productCategory == ProductCategory.AVANADE)
        {
            StartCoroutine(playfab.GetUserRole(playfab.GetUserId(), (role) =>
            {
                if (role == 3)
                {
                    Debug.Log("🟩 Cargo permitido. Teleportando para Avanade.");
                    Vector3 pos = avanadeLocation.position;
                    pm.photonView.RPC("RPC_Teleport", RpcTarget.AllBuffered, pos);
                }
                else
                {
                    Debug.Log("🟥 Cargo NÃO permitido. Bloqueando entrada.");
                    notAllowed.SetActive(true);
                }
            }));

            return;
        }

        Vector3 finalPos = GetTeleportPosition(productCategory);
        pm.photonView.RPC("RPC_Teleport", RpcTarget.AllBuffered, finalPos);
        PlayDoorOpenSound();
    }

    private Vector3 GetTeleportPosition(ProductCategory category)
    {
        switch (category)
        {
            case ProductCategory.DADOS_IA:
                return dataLocation.position;
            case ProductCategory.NUVEM_E_PLATAFORMAS:
                return cloudLocation.position;
            case ProductCategory.SEGURANCA:
                return securityLocation.position;
            case ProductCategory.TECNOLOGIA_INOVACAO:
                return techLocation.position;
            case ProductCategory.OUTROS:
                return othersLocation.position;
            default:
                return Vector3.zero;
        }
    }

    private void PlayDoorOpenSound()
    {
        audioSource.PlayOneShot(openDoorSound);
    }

    private void PlayDoorCloseSound()
    {
        audioSource.PlayOneShot(closeDoorSound);

    }
}
