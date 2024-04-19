using UnityEngine;

/// <summary>
///     Represents a special effect (FX) object that can be created, initialized, and destroyed.
/// </summary>
public class FX : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particleSystems = new ParticleSystem[3];
    [SerializeField] private float durationInSec;

    private FXType fXType;

    public void OnParticleSystemStopped()
    {
        if (gameObject.activeSelf) FXManager.Instance.KillFX(this);
    }

    public void InitAndPlayFX(Transform target, FXType type)
    {
        fXType = type;
        if (target == null) gameObject.transform.position = Vector3.zero;
        else gameObject.transform.position = target.position;

        foreach (var particle in particleSystems) particle.Play();

        Invoke("OnParticleSystemStopped", durationInSec);
    }

    public FXType GetFXType()
    {
        return fXType;
    }
}