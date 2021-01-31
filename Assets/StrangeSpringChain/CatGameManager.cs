using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CatGameManager : MonoBehaviour
{
    public float countdownTime = 180;
    public int totalScore = 20;
    public Text scoreLabel;
    public Text resultLabel;
    public Text countdownLabel;
    public Slider countdownSlider;
    public Tweener countdownTweener;

    string scoreFormat;
    string countdownFormat;
    // Start is called before the first frame update
    void Start()
    {
        scoreFormat = scoreLabel.text;
        countdownFormat = countdownLabel.text;
        StartGame();
    }

    void StartGame()
    {
        countdownSlider.value = 1;
        countdownTweener = countdownSlider.DOValue(0, countdownTime).SetEase(Ease.Linear).OnComplete(CheckResult);
    }

    void CheckResult()
    {
        if (SprintChainGenerator.instance.nodeList.Count >= totalScore)
        {
            resultLabel.DOText("找到所有的尾巴，回到了主人身边", 2f);
            transform.DOMoveZ(0.1f, 3).OnComplete(() =>
            {
                WhaleFall.GameManager.ins.EndGame();
            });
        }
        else
        {
            resultLabel.DOText("猫猫已回到天国...     ", 3f).OnComplete(() =>
            {
                WhaleFall.GameManager.ins.RestartGame();
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        scoreLabel.text = string.Format(scoreFormat, SprintChainGenerator.instance.nodeList.Count - 1, totalScore);

        if (countdownTweener?.IsPlaying() ?? false)
        {
            countdownLabel.text = string.Format(countdownFormat, (countdownTime - countdownTweener.Elapsed()).ToString("0.00"));
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            WhaleFall.GameManager.ins.RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            WhaleFall.GameManager.ins.EndGame();
        }
    }
}
