using Photon.Pun;
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

    [Header("Category")]
    public ProductCategory productCategory;

    [Header("Teleport Positions")]
    public Transform dataLocation;
    public Transform cloudLocation;
    public Transform securityLocation;
    public Transform techLocation;
    public Transform avanadeLocation;
    public Transform othersLocation;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();
        if (pm == null)
            return;

        Vector3 finalPos = Vector3.zero;

        switch (productCategory)
        {
            case ProductCategory.DADOS_IA:
                finalPos = dataLocation.position;
                break;

            case ProductCategory.NUVEM_E_PLATAFORMAS:
                finalPos = cloudLocation.position;
                break;

            case ProductCategory.SEGURANCA:
                finalPos = securityLocation.position;
                break;

            case ProductCategory.TECNOLOGIA_INOVACAO:
                finalPos = techLocation.position;
                break;

            case ProductCategory.AVANADE:
                finalPos = avanadeLocation.position;
                break;

            case ProductCategory.OUTROS:
                finalPos = othersLocation.position;
                break;
        }

        pm.photonView.RPC("RPC_Teleport", Photon.Pun.RpcTarget.All, finalPos);
    }
}
