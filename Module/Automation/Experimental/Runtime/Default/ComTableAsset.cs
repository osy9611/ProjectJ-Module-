using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Automation
{
    [Serializable]
    public class TableAssetInfo
    {
        [SerializeField] int tableId;
        [SerializeField] string addressable;

        public int TableId => tableId;
        public string Addressable => addressable;

        public TableAssetInfo(int tableId, string addressable)
        {
            this.tableId = tableId;
            this.addressable = addressable;
        }
    }


    public class ComTableAsset : MonoBehaviour
    {
        [SerializeField] List<TableAssetInfo> datas = new List<TableAssetInfo>();
        public List<TableAssetInfo> Datas => datas;

        public void Add(TableAssetInfo info)
        {
            if (Get(info.TableId) != null)
                return;

            datas.Add(info);
        }

        public TableAssetInfo Get(int id)
        {
            for (int i = 0, range = datas.Count; i < range; ++i)
            {
                if (datas[i].TableId == id)
                    return datas[i];
            }

            return null;
        }

        public void Remove(int id)
        {
            for (int i = 0, range = datas.Count; i < range; ++i)
            {
                if (datas[i].TableId == id)
                    datas.RemoveAt(i);
            }
        }
    }

}
