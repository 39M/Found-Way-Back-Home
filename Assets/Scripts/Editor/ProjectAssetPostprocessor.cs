using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace WhaleFall
{
    public class ProjectAssetPostprocessor : AssetPostprocessor
    {
        //模型导入之前调用
        public void OnPreprocessModel()
        {
            //Debug.Log("OnPreprocessModel=" + this.assetPath);
        }
        //模型导入之前调用
        public void OnPostprocessModel(GameObject go)
        {
            //Debug.Log("OnPostprocessModel=" + go.name);
        }

        public void OnPreprocessAnimation()
        {
            if (assetPath.Contains("Assets/Arts/MannequinRun"))
            {
                ModelImporter modelImporter = assetImporter as ModelImporter;
                var animations = modelImporter.defaultClipAnimations;
                for (int i = 0; i < animations.Length; i++)
                {
                    animations[i].loopTime = true;
                    animations[i].takeName = Path.GetFileNameWithoutExtension(assetPath);
                    animations[i].name = Path.GetFileNameWithoutExtension(assetPath);
                    animations[i].lockRootHeightY = true;
                    animations[i].keepOriginalPositionXZ = true;
                }
                modelImporter.clipAnimations = animations;

                modelImporter.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
                var go = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Arts/Mannequin/Mesh/SK_Mannequin.FBX");
                var avator = go.GetComponent<Animator>().avatar;
                modelImporter.sourceAvatar = avator;
                modelImporter.animationType = ModelImporterAnimationType.Human;
                modelImporter.SaveAndReimport();
            }
        }
    }
}