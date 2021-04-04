using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource    _audioSource    = null;
    [SerializeField] private AudioClip      _laser          = null;
    [SerializeField] private AudioClip      _explosion      = null;
    [SerializeField] private AudioClip      _powerup        = null;

    public void PlayExplosionSound()
    {
        _audioSource.PlayOneShot(_explosion);
    }

    public void PlayLaserSound()
    {
        _audioSource.PlayOneShot(_laser);
    }

    public void PlayPowerupSound()
    {
        _audioSource.clip = _powerup;
        _audioSource.Play();
    }
}
