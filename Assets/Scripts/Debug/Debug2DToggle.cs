using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Debug2DToggle : MonoBehaviour
{
    public void Debug_particle()
    {
        var objects = GameObject.FindGameObjectsWithTag("pigi");
        foreach (var obj in objects)
        {
            obj.GetComponent<PigiCtrl>().autoParticleOn = gameObject.GetComponent<Toggle>().isOn;
        }
    }

    public void Debug_coin()
    {
        var objects = GameObject.FindGameObjectsWithTag("pigi");
        foreach (var obj in objects)
        {
            obj.GetComponent<PigiCtrl>().showCoin = gameObject.GetComponent<Toggle>().isOn;
        }
    }

    public void fire()
    {
        var objects = GameObject.FindGameObjectsWithTag("pigi");
        foreach (var obj in objects)
        {
            obj.GetComponent<PigiCtrl>().AutoHarvest();
        }
    }
}
