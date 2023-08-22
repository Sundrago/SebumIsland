using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
        AudioCtrl.Instance.PlayBGM(SFX_tag.bgm_main);
    }
}
