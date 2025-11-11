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
        if (collision.tag != "Player") return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();
        if (pm == null) return;

        Vector2 targetPos = Vector2.zero;

        switch (productCategory)
        {
            case ProductCategory.DADOS_IA:
                targetPos = dataLocation.position;
                break;
            case ProductCategory.NUVEM_E_PLATAFORMAS:
                targetPos = cloudLocation.position;
                break;
            case ProductCategory.SEGURANCA:
                targetPos = securityLocation.position;
                break;
            case ProductCategory.TECNOLOGIA_INOVACAO:
                targetPos = techLocation.position;
                break;
            case ProductCategory.AVANADE:
                targetPos = avanadeLocation.position;
                break;
            case ProductCategory.OUTROS:
                targetPos = othersLocation.position;
                break;
        }

        pm.TeleportTo(targetPos);
    }

}
