using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileAnimator : MonoBehaviour
{
    public void tweenTile(Tile tile, Vector3 target, float duration)
    {
        if (tile.transform == null) return;
        transform.DOMove(target, duration)
          .OnComplete(() => tile.setCanDestroy(true))
          .SetEase(Ease.InOutExpo);
    }

    public void deleteAnim(Tile tile)
    {
        transform.DOScale(new Vector3(0, 0), 0.2f)
            .OnComplete(() => Destroy(tile.gameObject))
            .SetEase(Ease.OutExpo);
    }


}
