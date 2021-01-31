using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhaleFall
{
    [System.Serializable]
    public struct NPCWayPoint
    {
        public Transform StartPos;
        public Transform EndPos;
    }

    public class NPCManager : MonoBehaviour
    {
        public List<NPCController> npcTemplateList;

        public List<NPCWayPoint> spawnPoints = new List<NPCWayPoint>();

        public void Start()
        {
            for (int i = 0; i < 20; i++)
            {
                if (npcTemplateList.Count > 0)
                {
                    var go = Instantiate(npcTemplateList[Random.Range(0, npcTemplateList.Count)], transform, false);
                    go.gameObject.SetActive(true);
                    go.transform.position = GetNextWayPoint();

                    var npcController = go.GetComponent<NPCController>();
                    npcController.nPCManager = this;
                }
            }
        }

        public Vector3 GetNextWayPoint()
        {
            if (spawnPoints.Count > 0)
            {
                return spawnPoints[Random.Range(0, spawnPoints.Count)].StartPos.position;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}
