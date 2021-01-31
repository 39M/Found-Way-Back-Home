using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhaleFall
{
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {
        public AudioSource myMusic;             //BGM => loop
        public AudioSource myShortMusic;        //Win, lose sound
        static int maxEffect = 5;
        public AudioSource[] myEffect = new AudioSource[maxEffect];     //Effect sound
        int curEffect;

        public override void Init()
        {
            base.Init();
            curEffect = 0;
        }

        ResourceRequest rr;
        public IEnumerator LoadAllSound()
        {
            List<FileInfo> soundFileList = new List<FileInfo>();
            string soundFloderPath = "Assets/Resources/Sound";
            DirectoryInfo dir = new DirectoryInfo(soundFloderPath);
            Extend.GetAllChildFiles(dir.FullName, soundFileList);
            foreach(FileInfo path in soundFileList)
            {
                if(Path.GetExtension(path.Name) != ".meta")
                {
                    rr = Resources.LoadAsync("Sound/" + Path.GetFileNameWithoutExtension(path.Name));
                    while (!rr.isDone)
                    {
                        yield return null;
                    }
                }
            }
        }

        public void PlayMusic(string clipName)
        {
            myMusic.Stop();

            if (clipName != "null")
            {
                AudioClip myAudioClip = Resources.Load("Sound/" + clipName) as AudioClip;
                if (myAudioClip == null)
                {
                    Debug.Log("Find no AudioClip : " + clipName);
                    return;
                }
                myMusic.clip = myAudioClip;
                myMusic.Play();
            }
        }
        public void PlayShortMusic(string clipName)
        {
            if (clipName != "null")
            {
                AudioClip myAudioClip = Resources.Load("Sound/" + clipName) as AudioClip;
                if (myAudioClip == null)
                {
                    Debug.Log("Find no AudioClip : " + clipName);
                    return;
                }

                myShortMusic.PlayOneShot(myAudioClip);

            }

        }

        public void PlayEffect(string clipName, float pitch, float volume)
        {
            if (clipName != "null")
            {
                AudioClip myAudioClip = Resources.Load("Sound/" + clipName) as AudioClip;
                if (myAudioClip == null)
                {
                    Debug.Log("Find no AudioClip : " + clipName);
                    return;
                }

                myEffect[curEffect].PlayOneShot(myAudioClip);
                myEffect[curEffect].pitch = pitch;
                myEffect[curEffect].volume = volume;


                curEffect++;
                curEffect = curEffect % maxEffect;
            }
        }
    }
}