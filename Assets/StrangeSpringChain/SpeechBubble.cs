using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;

public class SpeechBubble : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 5, 0);
    public bool onlyShowOnCollision = true;
    public float showDurationOnCollision = 3f;

    // Start is called before the first frame update
    void Start()
    {
        var node = GetComponentInParent<Node>();
        if (node)
        {
            onlyShowOnCollision = false;
        }
        else
        {
            gameObject.AddComponent<CanvasGroup>().alpha = 0;
            transform.parent.gameObject.AddComponent<ShowBubbleOnCollision>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position + offset;
        transform.eulerAngles = new Vector3(45, 0, 0);

        var parentScale = transform.parent.localScale;
        transform.localScale = new Vector3(1f / parentScale.x, 1f / parentScale.y, 1f / parentScale.z) * 0.05f;
    }

    public void ShowBubble()
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        if (!DOTween.IsTweening(canvasGroup))
        {
            canvasGroup.DOFade(1, 0.5f).OnComplete(() =>
            {
                canvasGroup.DOFade(0, 0.5f).SetDelay(showDurationOnCollision);
            });
        }
    }
}
