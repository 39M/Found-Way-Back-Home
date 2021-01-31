using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using DG.Tweening;

public class SpringChainController : SprintChainGenerator
{
    public List<string> featureTextList;
    public GameObject textPrefab;

    private void Awake()
    {
        DOTween.defaultTimeScaleIndependent = true;

        var example = (@"
解谜
Roguelike
RPG
节奏
射击
弹幕
meta
剧情
立意
切题
创新
完成度
光追
像素风
音乐
音效
特效
打击感
恋爱
模拟
8-bit
VR
AR
策略
深度
子弹时间
沉浸感
电影过场
球
步行模拟
手绘
程序生成
未来
复古
极简主义
开放世界
CRPG
多结局
恐怖
物理
拟真
欢乐
裸露
魂Like
文字冒险
放置
动作
竞速
").Trim().Split('\n');
        featureTextList = new List<string>(example);

        foreach (var text in featureTextList)
        {
            var go = Instantiate(textPrefab);
            var tmp = go.GetComponent<TextMeshPro>();
            tmp.text = text;
            tmp.ForceMeshUpdate(true, true);

            var box = go.GetComponent<BoxCollider>();
            box.center = tmp.mesh.bounds.center;
            var size = tmp.mesh.bounds.size;
            size.z = 5;
            box.size = size;

            // 随着速度改变相机 size
            // TODO 均匀分布？  改成单线分布？
            // 卡着音乐最后的节奏做个表现
            go.transform.position = new Vector3((Random.value > 0.5 ? 1 : 1) * Random.Range(10, 750), 0f, (Random.value > 0.5 ? 1 : -1) * Random.Range(0, 30));
            go.transform.eulerAngles = new Vector3(0, Random.Range(-30, 30), 0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.onBeforeRender += UpdateLineRenderer;

        reverseInput = true;

        instance = this;

        rb = GetComponent<Rigidbody>();

        nodeList = new List<GameObject>();
        nodeList.Add(gameObject);

        for (int i = 0; i < nodeCount; i++)
        {
            //AddNode();

            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-200, 200), 0, Random.Range(-200, 200));
            Instantiate(foodPrefab).transform.position = spawnPosition;
        }
    }

    public override void AddNode(GameObject go)
    {
        var prevGo = nodeList[nodeList.Count - 1];
        if (!go.GetComponent<SpringJoint>())
        {
            // 落单的节点，才单独设置位置
            float x;
            if (prevGo.GetComponent<MeshFilter>())
            {
                x = prevGo.GetComponent<MeshFilter>().mesh.bounds.size.x / 2f;
            }
            else
            {
                x = GetComponent<CapsuleCollider>().radius;
            }

            x += go.GetComponent<MeshFilter>().mesh.bounds.size.x / 2f;
            go.transform.position = prevGo.transform.position + new Vector3(0, 0, x + 1f);
        }

        if (!prevGo.GetComponent<SpringJoint>())
        {
            prevGo.AddComponent<SpringJoint>();
        }
        SpringJoint springJoint = prevGo.GetComponent<SpringJoint>();
        SetSpringJoint(springJoint, go);

        nodeList.Add(go);
        go.GetComponent<Node>().linked = true;

        var s = go.GetComponent<SpringJoint>();
        if (s && s.connectedBody)
        {
            AddNode(s.connectedBody.gameObject);
        }
    }

    void SetSpringJoint(SpringJoint springJoint, GameObject connectedGameObject)
    {
        springJoint.connectedBody = connectedGameObject.GetComponent<Rigidbody>();
        SpringJoint prefab = springJoint.gameObject == gameObject ? rootJointPrefab : childJointPrefab;
        springJoint.anchor = prefab.anchor;
        springJoint.autoConfigureConnectedAnchor = prefab.autoConfigureConnectedAnchor;

        var sbc = springJoint.GetComponent<BoxCollider>();
        if (sbc)
        {
            Vector3 anchor = sbc.size / 2;
            anchor.y = 0;
            anchor.z = 0;
            springJoint.anchor = anchor;
        }

        var cbc = connectedGameObject.GetComponent<BoxCollider>();
        if (cbc)
        {
            springJoint.autoConfigureConnectedAnchor = false;
            Vector3 anchor = cbc.size / 2;
            anchor.y = 0;
            anchor.z = 0;
            anchor.x = -anchor.x;
            springJoint.connectedAnchor = anchor;
        }

        springJoint.spring = prefab.spring;
        springJoint.damper = prefab.damper;
        springJoint.minDistance = prefab.minDistance;
        springJoint.maxDistance = prefab.maxDistance;
        springJoint.tolerance = prefab.tolerance;
        if (springJoint.gameObject != gameObject)
        {
            springJoint.breakForce = prefab.breakForce * Mathf.Sqrt(Mathf.Sqrt(1f / nodeList.Count));
        }
        else
        {
            springJoint.breakForce = prefab.breakForce;
        }
        springJoint.breakTorque = prefab.breakTorque;
        springJoint.enableCollision = prefab.enableCollision;
        springJoint.enablePreprocessing = prefab.enablePreprocessing;
        springJoint.massScale = prefab.massScale;
        springJoint.connectedMassScale = prefab.connectedMassScale;
    }

    private void FixedUpdate()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (reverseInput)
        {
            input *= -1;
        }
        rb.AddForce(input * -controlForce, ForceMode.Acceleration);

        rb.transform.forward = -input.normalized;
        arrowNode.transform.forward = input.normalized;
        var arrowScale = input.magnitude * 2;
        var arrowTransform = arrowNode.transform.GetChild(0);
        arrowTransform.localScale = new Vector3(1, 1, arrowScale);
        arrowTransform.gameObject.SetActive(arrowScale > 0.1);
    }

    public Cinemachine.CinemachineVirtualCamera virtualCamera;
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    var sj = Resources.FindObjectsOfTypeAll<SpringJoint>();
        //    foreach (var s in sj)
        //    {
        //        if (s.GetComponent<Node>())
        //        {
        //            s.GetComponent<Node>().linked = false;
        //        }
        //        Destroy(s);
        //    }

        //    nodeList.Clear();
        //    nodeList.Add(gameObject);
        //}

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        float targetSize = 15 + Mathf.Lerp(0, 30, (input.magnitude));
        virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothStep(virtualCamera.m_Lens.OrthographicSize, targetSize, Time.deltaTime * 5);
    }

    public override void UpdateLineRenderer()
    {
        List<Vector3> vector3s = new List<Vector3>();
        foreach (var j in nodeList)
        {
            vector3s.Add(j.transform.position);
        }
        lineRenderer.positionCount = vector3s.Count;
        lineRenderer.SetPositions(vector3s.ToArray());
    }
}
