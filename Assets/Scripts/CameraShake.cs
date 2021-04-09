using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float _shakeAmount = 0.2f;
    [SerializeField] private float _shakeTime = 0f;

    public void ActivateCameraShake()
    {
        StartCoroutine(CameraShakeRoutine());
    }

    private IEnumerator CameraShakeRoutine()
    {
        var randomXvalue = Random.value * _shakeAmount * 2;
        var randomYvalue = Random.value * _shakeAmount * 2;

        var randomXoffset = Random.Range(-randomXvalue, randomXvalue);
        var randomYoffset = Random.Range(-randomYvalue, randomYvalue);

        var randomPosition = new Vector3(transform.position.x + randomXoffset, transform.position.y + randomYoffset, -10);

        transform.position = randomPosition;

        yield return new WaitForSeconds(_shakeTime);

        transform.position = new Vector3(0, 0, -10);
    }
}
