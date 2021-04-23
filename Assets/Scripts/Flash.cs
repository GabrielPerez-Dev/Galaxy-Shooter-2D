using System.Collections;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer = null;

    public void WhenHit()
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
