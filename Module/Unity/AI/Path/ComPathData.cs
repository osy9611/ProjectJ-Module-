namespace Module.Unity.AI
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class PathDatas
    {
        [SerializeField] int id;
        public int Id => id;

        [SerializeField] List<PathInfo> pathInfos = new List<PathInfo>();
        public List<PathInfo> PathInfos => pathInfos;

        public void SetList(List<PathInfo> info)
        {
            pathInfos = info;
        }

        public PathDatas(int id, List<PathInfo>info) 
        {
            this.id = id;
            this.pathInfos = info;
        }
    }

    public class ComPathData : MonoBehaviour
    {
        [SerializeField] List<PathDatas> pathDatas = new List<PathDatas>();
        public List<PathDatas> PathDatas => pathDatas;

        private Dictionary<int, PathDatas> pathDatasDict = new Dictionary<int, PathDatas>();

        public void Init()
        {
            foreach(var data in pathDatas)
            {
                if (pathDatasDict.ContainsKey(data.Id))
                    continue;

                pathDatasDict.Add(data.Id, data);
            }
        }

        public List<PathInfo> Get(int modelId)
        {
            PathDatas data = null;
            if (!pathDatasDict.TryGetValue(modelId, out data))
                return null;

            return data.PathInfos;
        }

        public PathInfo Get(int modelId, int pathId)
        {
            PathDatas data = null;
            if(!pathDatasDict.TryGetValue(modelId, out data))
                return null;

            PathInfo info = data.PathInfos.Find(x => x.Id == pathId);
            return info;
        }

        public void Add(int id, List<PathInfo> info)
        {
            PathDatas data = PathDatas.Find(x => x.Id == id);
            if (data != null)
            {
                data.SetList(info); 
            }
            else
            {
                data = new PathDatas(id, info);
                pathDatas.Add(data);
            }
            
        }
    }

}
