using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintChainGenerator : MonoBehaviour
{
    public static SprintChainGenerator instance;

    public bool reverseInput = false;

    protected Rigidbody rb;
    public float controlForce = 1f;

    public int nodeCount = 10;

    public float positionOffset = 1.25f;

    public List<GameObject> nodeList;

    public GameObject nodePrefab;
    public GameObject foodPrefab;

    public SpringJoint rootJointPrefab;
    public SpringJoint childJointPrefab;

    public int animatorSpeedRatio = 50;

    public GameObject lowSpeedEffect;
    public GameObject highSpeedEffect;

    Animator animator { get; set; }
    KartGame.KartSystems.KartMovement kartMovement { get; set; }
    int animIdMovement;
    int animIdInAir;

    public GameObject pickUpEffect;
    public GameObject dropEffect;

    // Start is called before the first frame update
    void Start()
    {
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

        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        kartMovement = GetComponent<KartGame.KartSystems.KartMovement>();
        animIdMovement = Animator.StringToHash("Movement");
        animIdInAir = Animator.StringToHash("InAir");
    }

    public virtual void AddNode(GameObject go)
    {
        var prevGo = nodeList[nodeList.Count - 1];
        if (!go.GetComponent<SpringJoint>())
        {
            // 落单的节点，才单独设置位置
            go.transform.position = prevGo.transform.position - positionOffset * (prevGo.transform.forward);
        }

        if (go.GetComponent<BoxCollider>())
        {
            float scale = 4f / go.GetComponent<BoxCollider>().size.magnitude;
            go.transform.localScale = new Vector3(scale, scale, scale);
        }

        if (!prevGo.GetComponent<SpringJoint>())
        {
            prevGo.AddComponent<SpringJoint>();
        }
        SpringJoint springJoint = prevGo.GetComponent<SpringJoint>();
        SetSpringJoint(springJoint, go);

        if (go.GetComponent<Animator>())
        {
            go.GetComponent<Animator>().enabled = false;
        }

        nodeList.Add(go);
        go.GetComponent<Node>().linked = true;

        var s = go.GetComponent<SpringJoint>();
        if (s && s.connectedBody)
        {
            AddNode(s.connectedBody.gameObject);
        }

        var dp = go.GetComponentInParent<DG.Tweening.DOTweenPath>();
        if (dp)
        {
            Destroy(dp);
        }

        dp = go.GetComponentInChildren<DG.Tweening.DOTweenPath>();
        if (dp)
        {
            Destroy(dp);
        }

        kartMovement.SetSprintCount(nodeList.Count);
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
            anchor.x = 0;
            springJoint.anchor = anchor;
        }

        var cbc = connectedGameObject.GetComponent<BoxCollider>();
        if (cbc)
        {
            springJoint.autoConfigureConnectedAnchor = false;
            Vector3 anchor = cbc.size / 2;
            anchor.x = 0;
            anchor.z = -anchor.z;
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

    const string HORIZONTAL_INPUT_STR = "Horizontal";
    const string VERTICAL_INPUT_STR = "Vertical";
    private void FixedUpdate()
    {
        Vector3 input = new Vector3(Input.GetAxis(HORIZONTAL_INPUT_STR), 0, Input.GetAxis(VERTICAL_INPUT_STR));
        if (reverseInput)
        {
            input *= -1;
        }

        if (input != Vector3.zero)
        {
            arrowNode.transform.forward = input.normalized;
        }

        var arrowScale = input.magnitude * 2;
        var arrowTransform = arrowNode.transform.GetChild(0);
        arrowTransform.localScale = new Vector3(1, 1, arrowScale);
        arrowTransform.gameObject.SetActive(arrowScale > 0.1);

        animator.speed = Mathf.Abs(Mathf.Clamp(kartMovement.LocalSpeed, -10, 30) / 10);
        if (kartMovement.LocalSpeed >= 25f)
        {
            animator.SetInteger(animIdMovement, 2);

            if (lowSpeedEffect)
            {
                lowSpeedEffect.gameObject.SetActive(false);
            }

            if (highSpeedEffect)
            {
                highSpeedEffect.gameObject.SetActive(true);
            }
        }
        else if (kartMovement.LocalSpeed > .01f)
        {
            animator.SetInteger(animIdMovement, 1);

            if (lowSpeedEffect)
            {
                lowSpeedEffect.gameObject.SetActive(true);
            }

            if (highSpeedEffect)
            {
                highSpeedEffect.gameObject.SetActive(false);
            }
        }
        else if (kartMovement.LocalSpeed < 0)
        {
            animator.SetInteger(animIdMovement, -1);

            if (lowSpeedEffect)
            {
                lowSpeedEffect.gameObject.SetActive(true);
            }

            if (highSpeedEffect)
            {
                highSpeedEffect.gameObject.SetActive(false);
            }
        }
        else
        {
            animator.speed = 1;
            animator.SetInteger(animIdMovement, 0);

            if (lowSpeedEffect)
            {
                lowSpeedEffect.gameObject.SetActive(false);
            }

            if (highSpeedEffect)
            {
                highSpeedEffect.gameObject.SetActive(false);
            }
        }
    }


    public LineRenderer lineRenderer;
    public GameObject arrowNode;
    private void Update()
    {
        List<Vector3> vector3s = new List<Vector3>();
        foreach (var j in nodeList)
        {
            vector3s.Add(j.transform.position);
        }
        lineRenderer.positionCount = vector3s.Count;
        lineRenderer.SetPositions(vector3s.ToArray());

        //if (Input.GetKeyDown(KeyCode.X))
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
    }

    public float pushPower = 30f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody body = collision.rigidbody;
        if (body == null || body.isKinematic || collision.gameObject.GetComponent<Node>() != null)
        {
            return;
        }

        Vector3 pushDir = (collision.transform.position - transform.position);
        pushDir.y = 0;
        body.velocity = pushDir.normalized * pushPower;
    }

    public virtual void UpdateLineRenderer()
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
