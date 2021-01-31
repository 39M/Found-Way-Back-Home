using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBubbleOnCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") ||
            (collision.gameObject.CompareTag("Node") && collision.gameObject.GetComponent<Node>().linked))
        {
            GetComponentInChildren<SpeechBubble>().ShowBubble();
        }
    }
}
