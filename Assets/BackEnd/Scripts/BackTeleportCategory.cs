using Photon.Pun;
using UnityEngine;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();
        if (pm == null)
            return;

        // Só o dono do player executa o teleporte
        if (!pm.photonView.IsMine)
            return;

        Vector3 targetPos = GetTeleportPosition();

        // Teleporta e sincroniza com todos
        pm.photonView.RPC("RPC_Teleport", RpcTarget.AllBuffered, targetPos);
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
}
