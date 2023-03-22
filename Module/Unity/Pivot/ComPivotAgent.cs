namespace Module.Unity.Pivot
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum PivotType
    {
        Effect,
        UI
    }

    [System.Serializable]
    public class PivotInfo
    {
        [SerializeField] PivotType type;
        public PivotType Type => type;
        [SerializeField] int id;
        public int Id => id;
        [SerializeField] Transform pivotTr;
        [SerializeField] bool setParent;
        public bool SetParent=>setParent;
        public Transform PivotTr => pivotTr;
        [SerializeField] Vector3 positionOffset;
        public Vector3 PositionOffset => positionOffset;
        [SerializeField] Vector3 rotationOffset;
        public Vector3 RotationOffset => rotationOffset;
        [SerializeField] Vector3 scaleOffset = Vector3.one;
        public Vector3 ScaleOffset => scaleOffset;

        public PivotInfo(PivotType type, int id, Transform pivotTr, bool setParent,Vector3 positionOffset, Vector3 rotationOffset, Vector3 scaleOffset)
        {
            this.type = type;
            this.id = id;
            this.pivotTr = pivotTr;
            this.setParent = setParent;
            this.positionOffset = positionOffset;
            this.rotationOffset = rotationOffset;
            this.scaleOffset = scaleOffset;
        }
    }

    public class ComPivotAgent : MonoBehaviour
    {
        [SerializeField] List<PivotInfo> pivots = new List<PivotInfo>();
        public List<PivotInfo> Pivots { get => pivots; }
        private Dictionary<int, PivotInfo> pivotsDict = new Dictionary<int, PivotInfo>();
        
        private void Awake()
        {
            for (int i = 0, range = pivots.Count; i < range; ++i)
            {
                if (!pivotsDict.ContainsKey(pivots[i].Id))
                {
                    pivotsDict.Add(pivots[i].Id, pivots[i]);
                }
            }
        }

        public PivotInfo GetPivotInfo(int id)
        {
            if (pivotsDict.ContainsKey(id))
            {
                return pivotsDict[id];
            }

            return null;
        }

        public Vector3 GetPosition(int id)
        {
            var info = GetPivotInfo(id);
            return info.PivotTr.position + info.PositionOffset;
        }

#if UNITY_EDITOR
        public void GenerateData()
        {

        }
#endif
    }

}
