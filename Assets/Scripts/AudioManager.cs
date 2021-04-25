using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource    _audioSource            = null;
    [SerializeField] private AudioSource    _audioSourceMusic       = null;
    [SerializeField] private AudioSource    _audioSourceGameOver    = null;
    [SerializeField] private AudioClip      _laser                  = null;
    [SerializeField] private AudioClip      _tripleShot             = null;
    [SerializeField] private AudioClip      _godsWish               = null;
    [SerializeField] private AudioClip      _eatThis                = null;
    [SerializeField] private AudioClip      _explosion              = null;
    [SerializeField] private AudioClip      _droneExplosion         = null;
    [SerializeField] private AudioClip      _powerup                = null;
    [SerializeField] private AudioClip      _ammoEmpty              = null;
    [SerializeField] private AudioClip      _shieldDamage           = null;
    [SerializeField] private AudioClip      _targeted               = null;
    [SerializeField] private AudioClip      _gameOver               = null;
    [SerializeField] private AudioClip      _winMusic               = null;
        
    public void PlayVictoryMusicSound()
    {
        _audioSourceMusic.clip = _winMusic;
        _audioSourceMusic.Play();
    }

    public void PlayExplosionSound()
    {
        _audioSource.PlayOneShot(_explosion);
    }

    public void PlayDroneExplosionSound()
    {
        _audioSource.clip = _droneExplosion;
        _audioSource.Play();
    }

    public void PlayLaserSound()
    {
        _audioSource.PlayOneShot(_laser);
    }

    public void PlayMissileSound()
    {
        _audioSource.PlayOneShot(_eatThis);
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

    public void PlayShieldDamageSound()
    {
        _audioSource.PlayOneShot(_shieldDamage);
    }

    public void PlayTargetSound()
    {
        _audioSource.clip = _targeted;
        _audioSource.Play();
    }

    public void PlayGameOverSound()
    {
        _audioSourceMusic.Stop();
        _audioSourceMusic.gameObject.SetActive(false);
        _audioSourceGameOver.gameObject.SetActive(true);
    }
}
