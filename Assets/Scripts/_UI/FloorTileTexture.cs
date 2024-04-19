using UnityEngine;

/// <summary>
///     Represents a floor tile texture in a game.
/// </summary>
public class FloorTileTexture : MonoBehaviour
{
    [SerializeField] private MeshRenderer a, b, c, d;
    [SerializeField] private GameObject sideA, sideB;

    public void SetSideMeshTexture()
    {
        a.material = gameObject.GetComponent<MeshRenderer>().material;
        b.material = gameObject.GetComponent<MeshRenderer>().material;
        c.material = gameObject.GetComponent<MeshRenderer>().material;
        d.material = gameObject.GetComponent<MeshRenderer>().material;

        a.gameObject.SetActive(true);
        b.gameObject.SetActive(true);
        c.gameObject.SetActive(true);
        d.gameObject.SetActive(true);
        sideA.SetActive(true);
        sideB.SetActive(true);
    }
}