using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource    _audioSource    = null;
    [SerializeField] private AudioClip      _laser          = null;
    [SerializeField] private AudioClip      _tripleShot     = null;
    [SerializeField] private AudioClip      _godsWish       = null;
    [SerializeField] private AudioClip      _explosion      = null;
    [SerializeField] private AudioClip      _powerup        = null;
    [SerializeField] private AudioClip      _ammoEmpty      = null;

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

    public void PlayAmmoEmptySound()
    {
        _audioSource.clip = _ammoEmpty;
        _audioSource.Play();
    }

    public void PlayTripleShotSound()
    {
        _audioSource.PlayOneShot(_tripleShot);
    }

    public void PlayGodsWishSound()
    {
        _audioSource.PlayOneShot(_godsWish);
    }
}
