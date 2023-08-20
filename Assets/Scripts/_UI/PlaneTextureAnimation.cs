using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneTextureAnimation : MonoBehaviour
{
    public Material[] materials = new Material[6];
    public int speed;
    int materialTotalCount;
    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        materialTotalCount = materials.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.frameCount % speed == 0)
        {
            GetComponent<MeshRenderer>().material = materials[count];
            count += 1;
            if (count >= materialTotalCount) count = 0;
        }
    }
}
