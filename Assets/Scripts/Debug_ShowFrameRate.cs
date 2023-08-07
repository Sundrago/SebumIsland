using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debug_ShowFrameRate : MonoBehaviour
{
    float deltatimeAdd;

    void Update()
    {
        float msec, fps;
        string text;
        if(Time.frameCount % 20 == 0)
        {
            deltatimeAdd += Time.deltaTime;
            msec = deltatimeAdd/20f * 1000.0f;
            fps = 1.0f / (deltatimeAdd / 20f);
            text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GetComponent<Text>().text = text;
            deltatimeAdd = 0;
        } else
        {
            deltatimeAdd += Time.deltaTime;
        }
    }
}
