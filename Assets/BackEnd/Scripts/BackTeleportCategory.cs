using Photon.Pun;
using UnityEngine;
using UnityEngine.Audio;

public class BackTeleportCategory : MonoBehaviour
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

    [Header("Category")]
    public ProductCategory productCategory;

    [Header("Back Teleport Positions")]
    public Transform backDataLocation;
    public Transform backCloudLocation;
    public Transform backSecurityLocation;
    public Transform backTechLocation;
    public Transform backAvanadeLocation;
    public Transform backOthersLocation;

    [Header("Audio")]
    public AudioClip openDoorSound;
    public AudioClip closeDoorSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();
        if (pm == null)
            return;

        if (!pm.photonView.IsMine)
            return;

        Vector3 targetPos = GetTeleportPosition();

        pm.photonView.RPC("RPC_Teleport", RpcTarget.AllBuffered, targetPos);
        PlayDoorOpenSound();
    }

    private Vector3 GetTeleportPosition()
    {
        switch (productCategory)
        {
            case ProductCategory.DADOS_IA:
                return backDataLocation.position;

            case ProductCategory.NUVEM_E_PLATAFORMAS:
                return backCloudLocation.position;

            case ProductCategory.SEGURANCA:
                return backSecurityLocation.position;

            case ProductCategory.TECNOLOGIA_INOVACAO:
                return backTechLocation.position;

            case ProductCategory.AVANADE:
                return backAvanadeLocation.position;

            case ProductCategory.OUTROS:
                return backOthersLocation.position;
        }

        return Vector3.zero;
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
