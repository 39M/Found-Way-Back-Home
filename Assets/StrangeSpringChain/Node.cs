using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class Node : MonoBehaviour
{
    public bool linked = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnJointBreak(float breakForce)
    {
        var nodeList = SprintChainGenerator.instance.nodeList;
        int index = nodeList.IndexOf(gameObject);

        if (index < 0)
        {
            return;
        }

        for (int i = nodeList.Count - 1; i >= index + 1; i--)
        {
            var go = nodeList[i];

            go.GetComponent<Rigidbody>().velocity = go.GetComponent<Rigidbody>().velocity / 2;
            nodeList.Remove(go);

            if (go.GetComponent<Node>())
            {
                go.GetComponent<Node>().linked = false;
            }
            else
            {
                Debug.LogError(go.name);
            }
            if (SprintChainGenerator.instance.dropEffect)
            {
                Instantiate(SprintChainGenerator.instance.dropEffect, go.transform).transform.localScale *= 3;
            }

            if (go.GetComponent<Animator>())
            {
                go.GetComponent<Animator>().enabled = true;
            }
        }

        StrangeGameManager.instance?.SubPercentage();


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (linked)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player") ||
            (collision.gameObject.CompareTag("Node") && collision.gameObject.GetComponent<Node>().linked))
        {
            var sj = Resources.FindObjectsOfTypeAll<SpringJoint>();
            foreach (var s in sj)
            {
                if (s.connectedBody && s.connectedBody.gameObject == gameObject)
                {
                    Destroy(s);
                }
            }
            if (SprintChainGenerator.instance.pickUpEffect)
            {
                Instantiate(SprintChainGenerator.instance.pickUpEffect, gameObject.transform.position, gameObject.transform.rotation).transform.localScale *= 3;
            }
            SprintChainGenerator.instance.AddNode(gameObject);
            StrangeGameManager.instance?.AddPercentage();

        }
    }
}
