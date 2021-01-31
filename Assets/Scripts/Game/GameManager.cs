using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace WhaleFall
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public GameObject startUI;
        public Text loadingText;
        public List<Image> startGameList = new List<Image>();
        public List<Image> endGameList = new List<Image>();

        IEnumerator Start()
        {
            startUI.gameObject.SetActive(true);
            loadingText.text = "按任意键...";
            GameObject soundManager = Instantiate(Resources.Load("SoundManager") as GameObject, transform);
            yield return SoundManager.ins.LoadAllSound();
            SoundHandler.PlayMusic("Toby Fox - sans");

            for (int i = 0; i < endGameList.Count; i++)
            {
                endGameList[i].gameObject.SetActive(false);
            }
            int count = 1;
            for (int i = 0; i < startGameList.Count; i++)
            {
                startGameList[i].gameObject.SetActive(true);
                if (i == 0)
                {
                    startGameList[i].color = new Color(1, 1, 1, 1);
                }
                else
                {
                    startGameList[i].color = new Color(1, 1, 1, 0);
                }
            }

            yield return null;

            while (count < startGameList.Count)
            {
                while (!Input.anyKeyDown)
                {
                    yield return null;
                }

                loadingText.text = "";

                while (startGameList[count].color.a < 1)
                {
                    startGameList[count].color = new Color(1, 1, 1, Mathf.Clamp01(startGameList[count].color.a + Time.deltaTime));
                    yield return null;
                }
                count++;
            }

            while (!Input.anyKeyDown)
            {
                yield return null;
            }

            loadingText.text = "载入中...";
            LoadScene();

            while (!isFinishedLoadScene) yield return null;
        }

        public GameObject ShowPanel(string path)
        {
            GameObject panel = Instantiate(Resources.Load(path) as GameObject, transform);
            panel.name = Path.GetFileNameWithoutExtension(path);
            return panel;
        }

        public bool isFinishedLoadScene
        {
            get
            {
                return async_operation.isDone;
            }
        }

        private AsyncOperation async_operation;
        public static string levelName = "SimpleTown_DemoScene";

        public void LoadScene()
        {
            StartCoroutine(LoadSceneAsync());
        }

        IEnumerator LoadSceneAsync()
        {
            async_operation = SceneManager.LoadSceneAsync(levelName);
            yield return async_operation;
            startUI.gameObject.SetActive(false);
        }

        public void RestartGame()
        {
            StartCoroutine(Start());
        }

        public void EndGame()
        {
            StartCoroutine(UnloadSceneAsync());
        }

        IEnumerator UnloadSceneAsync()
        {
            startUI.gameObject.SetActive(true);
            async_operation = SceneManager.UnloadSceneAsync(levelName);
            for (int i = 0; i < startGameList.Count; i++)
            {
                startGameList[i].gameObject.SetActive(false);
            }
            int count = 1;
            for (int i = 0; i < endGameList.Count; i++)
            {
                endGameList[i].gameObject.SetActive(true);
                if (i == 0)
                {
                    endGameList[i].color = new Color(1, 1, 1, 1);
                }
                else
                {
                    endGameList[i].color = new Color(1, 1, 1, 0);
                }
            }

            yield return null;

            while (count < endGameList.Count)
            {
                while (!Input.anyKeyDown)
                {
                    yield return null;
                }

                while (endGameList[count].color.a < 1)
                {
                    endGameList[count].color = new Color(1, 1, 1, Mathf.Clamp01(endGameList[count].color.a + Time.deltaTime));
                    yield return null;
                }
                count++;
            }

            while (!Input.anyKeyDown)
            {
                yield return null;
            }

            yield return async_operation;
            RestartGame();
        }
    }
}