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

    [Header("Teleport Positions")]
    public Transform backDataLocation;
    public Transform backCloudLocation;
    public Transform backSecurityLocation;
    public Transform backTechLocation;
    public Transform backAvanadeLocation;
    public Transform backOthersLocation;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player") return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();
        if (pm == null) return;

        Vector2 targetPos = Vector2.zero;

        switch (productCategory)
        {
            case ProductCategory.DADOS_IA:
                targetPos = backDataLocation.position;
                break;
            case ProductCategory.NUVEM_E_PLATAFORMAS:
                targetPos = backCloudLocation.position;
                break;
            case ProductCategory.SEGURANCA:
                targetPos = backSecurityLocation.position;
                break;
            case ProductCategory.TECNOLOGIA_INOVACAO:
                targetPos = backTechLocation.position;
                break;
            case ProductCategory.AVANADE:
                targetPos = backAvanadeLocation.position;
                break;
            case ProductCategory.OUTROS:
                targetPos = backOthersLocation.position;
                break;
        }

        pm.photonView.RPC("RPC_Teleport", pm.photonView.Owner, (Vector3)targetPos);
    }
}
