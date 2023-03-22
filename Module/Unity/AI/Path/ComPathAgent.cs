namespace Module.Unity.AI
{
    using Module.Automation;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    [System.Serializable]
    public class PathTransInfo
    {
        [SerializeField] int id;
        public int Id => id;

        [SerializeField] List<Transform> path = new List<Transform>();
        public List<Transform> Path => path;

        public bool ShowPath = false;

        public void Clear()
        {
            for (int i = 0, range = path.Count; i < range; ++i)
            {
                UnityEngine.Object.DestroyImmediate(path[i].gameObject);
            }
            path.Clear();
        }
    }

    [System.Serializable]
    public class PathInfo
    {
        [SerializeField] int id;
        public int Id => id;

        [SerializeField] List<Vector3> pathData = new List<Vector3>();
        public List<Vector3> PathData => pathData;


        public Vector3? GetPath(int index)
        {
            if (pathData.Count - 1 < index || index == -1)
                return null;

            return pathData[index];
        }


        public void GenerateData(List<Transform> path)
        {
            if (path.Count == 0) return;
            pathData.Clear();
            for (int i = 0, range = path.Count; i < range; ++i)
            {
                pathData.Add(path[i].position);
            }
        }

        public PathInfo(int id, List<Transform> path)
        {
            this.id = id;
            GenerateData(path);
        }
    }


    public class ComPathAgent : MonoBehaviour
    {

        [SerializeField] int id = -1;
        public int Id => id;

        [SerializeField] List<PathTransInfo> pathInfo = new List<PathTransInfo>();
        public List<PathTransInfo> PathInfo => pathInfo;


#if UNITY_EDITOR
        public void GenerateData()
        {
            List<PathInfo> path = new List<PathInfo>();
            foreach(var info in pathInfo)
            {
                path.Add(new PathInfo(info.Id, info.Path));
            }

            GameObject go = AssetDatabase.LoadAssetAtPath("Assets/Res/Data/ComPathData.prefab", typeof(UnityEngine.Object)) as GameObject;
            if (go == null)
                return;
            ComPathData data = go.GetComponent<ComPathData>();
            if (data == null) return;
            data.Add(id, path);
            PrefabUtility.SavePrefabAsset(data.gameObject);
        }

        public void ClearPath()
        {
            for (int i = 0, range = pathInfo.Count; i < range; ++i)
            {
                pathInfo[i].Clear();
            }
            pathInfo.Clear();

            PrefabUtility.SavePrefabAsset(this.gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            foreach (var info in pathInfo)
            {
                if (!info.ShowPath)
                    continue;

                for (int i = 0, range = info.Path.Count - 1; i < range; ++i)
                {
                    Gizmos.DrawLine(info.Path[i].position, info.Path[i + 1].position);
                }
            }
        }
#endif
    }

}

