using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhaleFall
{
    public class ClothPhysicsManager : SingletonMonoBehaviour<ClothPhysicsManager>
    {
        public override void Init()
        {
            base.Init();
            gameObject.GetOrAddComponent<MagicaCloth.MagicaPhysicsManager>();
        }
    }
}