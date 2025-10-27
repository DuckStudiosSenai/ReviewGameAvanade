using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class ObjectOutline : MonoBehaviour
{
    [Header("Outline Settings")]
    public Color outlineColor = Color.yellow;
    [Range(0.001f, 0.05f)]
    public float pixelOffset = 0.01f;

    [Header("Behavior")]
    public bool showOnHover = true;
    public bool alwaysVisible = false;

    private SpriteRenderer mainRenderer;
    private List<SpriteRenderer> outlineRenderers = new List<SpriteRenderer>();
    private Material outlineMaterial;

    // offsets nas 8 direções
    private static readonly Vector2[] directions = new Vector2[]
    {
        new Vector2( 1,  0),
        new Vector2(-1,  0),
        new Vector2( 0,  1),
        new Vector2( 0, -1),
        new Vector2( 1,  1),
        new Vector2(-1,  1),
        new Vector2( 1, -1),
        new Vector2(-1, -1),
    };

    void Start()
    {
        mainRenderer = GetComponent<SpriteRenderer>();

        // cria o material do outline (usa shader sólido que ignora a cor original)
        Shader solidShader = Shader.Find("Custom/SpriteSolidColor");
        if (solidShader == null)
        {
            Debug.LogError("Shader 'Custom/SpriteSolidColor' não encontrado! Verifique se ele está em Assets/Shaders.");
            return;
        }

        outlineMaterial = new Material(solidShader);
        outlineMaterial.SetColor("_Color", outlineColor);

        // cria renderers filhos (8 cópias deslocadas)
        for (int i = 0; i < directions.Length; i++)
        {
            var childObj = new GameObject("OutlinePart_" + i);
            childObj.transform.SetParent(transform);
            childObj.transform.localRotation = Quaternion.identity;
            childObj.transform.localScale = Vector3.one;

            var sr = childObj.AddComponent<SpriteRenderer>();
            sr.sprite = mainRenderer.sprite;
            sr.sortingLayerID = mainRenderer.sortingLayerID;
            sr.sortingOrder = mainRenderer.sortingOrder - 1;
            sr.material = outlineMaterial; // aplica o material sólido
            outlineRenderers.Add(sr);
        }

        // adiciona collider se não existir (pra OnMouseEnter/Exit)
        if (GetComponent<Collider2D>() == null)
            gameObject.AddComponent<BoxCollider2D>();

        ApplyVisibility(alwaysVisible || !showOnHover);
    }

    void Update()
    {
        // mantém sincronizado com o sprite principal
        foreach (var sr in outlineRenderers)
        {
            if (sr.sprite != mainRenderer.sprite)
                sr.sprite = mainRenderer.sprite;

            if (sr.sortingLayerID != mainRenderer.sortingLayerID)
                sr.sortingLayerID = mainRenderer.sortingLayerID;

            if (sr.sortingOrder != mainRenderer.sortingOrder - 1)
                sr.sortingOrder = mainRenderer.sortingOrder - 1;
        }

        // atualiza cor dinamicamente
        outlineMaterial.SetColor("_Color", outlineColor);

        // aplica deslocamento para cada cópia
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2 dir = directions[i];
            outlineRenderers[i].transform.localPosition = new Vector3(
                dir.x * pixelOffset,
                dir.y * pixelOffset,
                0f
            );
        }
    }

    void OnMouseEnter()
    {
        if (showOnHover && !alwaysVisible)
            ApplyVisibility(true);
    }

    void OnMouseExit()
    {
        if (showOnHover && !alwaysVisible)
            ApplyVisibility(false);
    }

    private void ApplyVisibility(bool visible)
    {
        foreach (var sr in outlineRenderers)
            sr.enabled = visible;
    }
}
