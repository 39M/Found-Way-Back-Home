using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhaleFall
{
    public class PlaySound : MonoBehaviour
    {
        public SoundType mySoundType;
        public SoundTrigger mySoundTrigger;
        public string myFileName;


        void Start()
        {
            if (mySoundTrigger == SoundTrigger.Start)
            {
                CheckSoundType();
            }
        }
        void OnClick()
        {
            if (mySoundTrigger == SoundTrigger.Click)
            {
                CheckSoundType();
            }
        }

        void CheckSoundType()
        {
            if (mySoundType == SoundType.Music)
                SoundHandler.PlayMusic(myFileName);
            if (mySoundType == SoundType.ShortMusic)
                SoundHandler.PlayShortMusic(myFileName);
            if (mySoundType == SoundType.Effect)
                SoundHandler.PlayEffect(myFileName);
        }

    }

    public enum SoundType
    {
        Music,
        ShortMusic,
        Effect,
    }

    public enum SoundTrigger
    {
        Click,
        Start,
    }
}