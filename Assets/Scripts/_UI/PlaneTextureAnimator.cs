using UnityEngine;

/// <summary>
///     Animates the texture on a plane mesh.
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class PlaneTextureAnimator : MonoBehaviour
{
    [SerializeField] private Material[] materials = new Material[6];
    [SerializeField] private int speed;
    private int count;
    private int materialTotalCount;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        materialTotalCount = materials.Length;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (Time.frameCount % speed == 0)
        {
            meshRenderer.material = materials[count];
            count += 1;
            if (count >= materialTotalCount) count = 0;
        }
    }
}