using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening;

public class StrangeGameManager : MonoBehaviour
{
    public float timeOffset = 0f;
    public static StrangeGameManager instance;
    public GameObject intro;
    public Image endMask;

    public Text introText;

    public Text countdownLabel;
    public Slider countdownSlider;
    public Tweener countdownTweener;

    float percentPerNode = 0.02718281828459f * 2;
    public Text percentageLabel;
    public Slider percentageSlider;

    public GameObject finalTimeNode;
    public GameObject finalPercentageNode;
    public GameObject resultMask;
    public GameObject thanksMask;

    private void Awake()
    {
        instance = this;
        intro.SetActive(true);
        introText.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 前奏 20 
        // total 109
        //intro.transform.DOMoveZ(-0.1f, 20).OnComplete(StartGame);
    }

    // TODO 动效！！

    bool gamestart = false;
    void StartGame()
    {
        if (gamestart)
            return;

        gamestart = true;

        GetComponent<AudioSource>().Play();

        StartCoroutine(InnerStartGame());
    }

    IEnumerator InnerStartGame()
    {
        intro.transform.Find("OffsetText").GetComponent<Text>().text = "";
        string text = introText.text;
        introText.gameObject.SetActive(true);
        introText.text = "";
        introText.DOText(text, 20).SetEase(Ease.Linear).OnComplete(() =>
        {
            intro.GetComponent<CanvasGroup>().DOFade(0, 1f);
        });

        yield return new WaitForSeconds(20);

        countdownSlider.value = 1;

        //GetComponent<AudioSource>().time = 20 + 70f;
        //countdownTweener = countdownSlider.DOValue(0, 85.75f - 70f).SetEase(Ease.Linear).OnComplete(() =>
        countdownTweener = countdownSlider.DOValue(0, 84.91f + timeOffset).SetEase(Ease.Linear).OnComplete(() =>
        {
            //Time.timeScale = 0;
            //countdownSlider.transform.DOScaleZ(1.01f, 2f).SetUpdate(UpdateType.Normal, true).OnComplete(() =>
            //{
            //    Time.timeScale = 1;
            //    CheckResult();
            //});



            percentageLabel.gameObject.SetActive(false);
            percentageSlider.gameObject.SetActive(false);
            countdownLabel.gameObject.SetActive(false);
            countdownSlider.gameObject.SetActive(false);

            //Time.timeScale = 0.25f;
            //GetComponent<AudioSource>().pitch = 0.25f;
            StartCoroutine(DiscardNode());

        });

        percentageSlider.value = 0;
        UpdatePercentage();

    }

    IEnumerator DiscardNode()
    {
        var nodeList = SprintChainGenerator.instance.nodeList;
        for (int i = 8; i >= 1; i--)
        {
            if (nodeList.Count - 1 >= i)
            {
                var prevGo = nodeList[i - 1];
                if (prevGo.GetComponent<SpringJoint>())
                {
                    Destroy(prevGo.GetComponent<SpringJoint>());
                }

                var go = nodeList[i];

                Debug.LogWarning("detach node " + i.ToString());

                var rb = go.GetComponent<Rigidbody>();
                rb.velocity = rb.velocity.normalized * 500;
                nodeList.Remove(go);

                if (go.GetComponent<Node>())
                {
                    go.GetComponent<Node>().linked = false;
                }

                SprintChainGenerator.instance.UpdateLineRenderer();
            }
            yield return new WaitForSeconds(0.14f);
        }
        CheckResult();
        yield return new WaitForSeconds(0.14f);
        finalTimeNode.SetActive(true);
        yield return new WaitForSeconds(0.385f);
        finalPercentageNode.SetActive(true);
        yield return new WaitForSeconds(1.75f);
        GetComponent<AudioSource>().Stop();
        yield return new WaitForSeconds(3f);
        thanksMask.SetActive(true);
        resultMask.transform.DOLocalMoveZ(0.1f, 7).OnComplete(() => { resultMask.SetActive(true); });
        yield return new WaitForSeconds(7f);
    }

    void CheckResult()
    {
        var sj = Resources.FindObjectsOfTypeAll<SpringJoint>();
        foreach (var s in sj)
        {
            if (s.GetComponent<Node>())
            {
                s.GetComponent<Node>().linked = false;
            }
            Destroy(s);

            s.GetComponent<Rigidbody>().velocity *= 1.5f;
        }

        SprintChainGenerator.instance.nodeList.Clear();
        SprintChainGenerator.instance.nodeList.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (countdownTweener?.IsPlaying() ?? false)
        {
            var hours = 48f * (1 - countdownTweener.ElapsedPercentage());
            int wholeHour = (int)Mathf.Floor(hours);
            int wholeMinute = (int)(Mathf.Round((hours - wholeHour) * 60));
            countdownLabel.text = string.Format("离 Global Game Jam 2021 结束还剩：{0:00} 小时 {1:00} 分钟", wholeHour, wholeMinute);
        }
        else
        {
            countdownLabel.text = string.Format("离 Global Game Jam 2021 结束还剩：{0:00} 小时 {1:00} 分钟", 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            //GetComponent<AudioSource>().time = 20;
            intro.transform.DOComplete(true);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            GetComponent<AudioSource>().time = 100;
            intro.transform.DOMoveZ(-0.1f, 7.5f).OnComplete(CheckResult);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1"))
        {
            StartGame();
        }

        // TODO 越到后面，越不容易脱缰

        if (Input.GetKey(KeyCode.E))
        {
            timeOffset += 0.005f;
            intro.transform.Find("OffsetText").GetComponent<Text>().text = timeOffset.ToString("0.00");
        }

        if (Input.GetKey(KeyCode.Q))
        {
            timeOffset -= 0.005f;
            intro.transform.Find("OffsetText").GetComponent<Text>().text = timeOffset.ToString("0.00");
        }
    }

    public void AddPercentage()
    {
        UpdatePercentage();
        if (!DOTween.IsTweening(percentageLabel))
        {
            percentageLabel.transform.localScale = Vector3.one;
            percentageLabel.transform.DOScale(1.3f, 0.2f).SetLoops(2, LoopType.Yoyo);
        }
    }

    public void SubPercentage()
    {
        UpdatePercentage();
        //percentageLabel.DOKill();
        //percentageLabel.transform.localScale = Vector3.one;
        if (!DOTween.IsTweening(percentageLabel))
        {
            var position = percentageLabel.transform.position;
            percentageLabel.transform.DOShakePosition(1f, 50, 20).OnComplete(() => { percentageLabel.transform.position = position; });
        }
    }

    public void UpdatePercentage()
    {
        float percent = ((SprintChainGenerator.instance.nodeList.Count - 1) * percentPerNode * Random.Range(0.8f, 1.2f));
        percentageLabel.text = string.Format("当前进度：{0: 0}%", (int)(percent * 100));
        percentageSlider.DOValue(percent, 0.4f);
    }
}
