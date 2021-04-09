using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flasher : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer = null;

    public void FlashWhenHit()
    {
        StartCoroutine(FlashSpriteRoutine());
    }

    private IEnumerator FlashSpriteRoutine()
    {
        var tempColor = Color.red;
        var defaultColor = _renderer.material.color;

        _renderer.color = tempColor;

        yield return new WaitForSeconds(0.05f);

        _renderer.color = defaultColor;

        yield return new WaitForSeconds(0.05f);

        _renderer.color = tempColor;

        yield return new WaitForSeconds(0.05f);

        _renderer.color = defaultColor;

        yield return new WaitForSeconds(0.05f);

        _renderer.color = tempColor;

        yield return new WaitForSeconds(0.05f);

        _renderer.color = defaultColor;
    }
}
