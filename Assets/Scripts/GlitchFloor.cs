using UnityEngine;

public class MaterialTilingY : MonoBehaviour
{
    public float speed = 1f; // Speed at which the tiling increases
    private Renderer objRenderer;
    private Material objMaterial;
    private Vector2 currentTiling;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
        {
            objMaterial = objRenderer.material;
            currentTiling = objMaterial.mainTextureScale;
        }
    }

    void Update()
    {
        if (objMaterial != null)
        {
            currentTiling.y += speed * Time.deltaTime; // Increase Y tiling over time
            if (currentTiling.y > 1000)
                speed = -1f;
            if (currentTiling.y < -1000)
                speed = 1f;

            objMaterial.mainTextureScale = currentTiling;
        }
    }
}
