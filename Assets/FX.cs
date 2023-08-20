using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX : MonoBehaviour
{
    [SerializeField] ParticleSystem[] particleSystems = new ParticleSystem[3];
    [SerializeField] float durationInSec;

    private FXType fXType;

    public void InitAndPlayFX(Transform target, FXType type)
    {
        fXType = type;
        if (target == null) gameObject.transform.position = Vector3.zero;
        else gameObject.transform.position = target.position;

        foreach (ParticleSystem particle in particleSystems)
        {
            particle.Play();
        }

        Invoke("OnParticleSystemStopped", durationInSec);
    }

    public void OnParticleSystemStopped()
    {
        if (gameObject.activeSelf) FXManager.Instance.KillFX(this);
    }

    public FXType GetFXType()
    {
        return fXType;
    }
}