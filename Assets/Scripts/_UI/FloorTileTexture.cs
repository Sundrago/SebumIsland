using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTileTexture : MonoBehaviour
{
    [SerializeField] MeshRenderer a,b,c,d;
    [SerializeField] GameObject sideA, sideB;

    public void SetSideMeshTexture(){
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
