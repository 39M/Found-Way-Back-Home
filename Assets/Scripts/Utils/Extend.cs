using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace WhaleFall
{
    public static class Extend
    {
        /// <summary>
        /// 两个向量之间的线性插值
        /// </summary>
        /// <param name="from">向量from</param>
        /// <param name="to">向量to</param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 Lerp(this Vector3 from, Vector3 to, float t)
        {
            if (t <= 0)
            {
                return from;
            }
            else if (t >= 1)
            {
                return to;
            }
            return t * to + (1 - t) * from;
        }

        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            var com = obj.GetComponent<T>();
            if (com == null)
            {
                com = obj.AddComponent<T>();
            }
            return com;
        }

        public static Component GetOrAddComponent(this GameObject obj, Type type)
        {
            var com = obj.GetComponent(type);
            if (com == null)
            {
                com = obj.AddComponent(type);
            }
            return com;
        }

        public static T GetOrAddComponent<T>(this Component behaviour) where T : Component
        {
            var com = behaviour.GetComponent<T>();
            if (com == null)
            {
                com = behaviour.gameObject.AddComponent<T>();
            }
            return com;
        }

        /// <summary>
        /// 获取指定目录所有文件及子文件夹中文件路径
        /// </summary>
        /// <param name="dir">指定目录</param>
        /// <param name="allFiles">储存路径的List</param>
        /// <returns></returns>
        public static void GetAllChildFiles(string dir, List<FileInfo> allFiles)
        {
            if (allFiles == null)
            {
                return;
            }
            DirectoryInfo di = new DirectoryInfo(dir);
            if (!di.Exists) return;//如果目录不存在,退出
            var currentDirFiles = di.GetFiles().Select(p => p);//获取当前目录所有文件bai
            allFiles.AddRange(currentDirFiles);//将当前目录文件放到allFiles中
            var currentDirSubDirs = di.GetDirectories().ToList();//获取子目录
            currentDirSubDirs.ForEach(p => GetAllChildFiles(p.FullName, allFiles));//将子目录中的文件放入allFiles中
        }
    }
}