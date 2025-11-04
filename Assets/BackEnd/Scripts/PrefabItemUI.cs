using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductItemUI : MonoBehaviour
{
    public Button productButton;
    public TextMeshProUGUI txtProductName;
    public TextMeshProUGUI txtEnterpriseName;
    public TextMeshProUGUI txtCategory;
    public TextMeshProUGUI txtDescription;

    public void SetData(APIManager.ProductObject product)
    {
        txtProductName.text = "Produto: " + product.name;
        txtEnterpriseName.text = "Empresa: " + product.enterprisename;
        txtCategory.text = "Categoria: " + product.category;
        txtDescription.text = "Descrição: " + product.description;
    }
}
