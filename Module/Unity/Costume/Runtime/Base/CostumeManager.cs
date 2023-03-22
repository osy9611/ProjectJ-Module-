namespace Module.Unity.Custermization
{
    using Module.Core.Systems.Collections.Generic;
    using Module.Core.Systems.Events;
    using Module.Unity.Addressables;
    using Module.Unity.Custermization;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CostumeManager
    {        
        public bool ChangeColor(ComCostumeAgent agent, int partIdx, Color color, IEventArgs args = null)
        {
            PartAssetData data = PartAssetData.Create(0, color, args);
            PartAssetData? result = null;
            agent.ChangeOrAttach(data, out result);

            return result == null;
        }

        public bool ChangeMaterial(ComCostumeAgent agent, int partIdx, Material material, IEventArgs args = null)
        {
            PartAssetData data = PartAssetData.Create(0, material, args);
            PartAssetData? result = null;
            agent.ChangeOrAttach(data, out result);

            return result == null;
        }

        public bool ChangeGameObject(ComCostumeAgent agent, int partIdx, GameObject gameobject, IEventArgs args = null)
        {
            PartAssetData data = PartAssetData.Create(0, gameobject, args);
            PartAssetData? result = null;
            agent.ChangeOrAttach(data, out result);

            return result == null;
        }

        public bool ChangeMeshRenderer(ComCostumeAgent agent, int partIdx, MeshRenderer renderer, bool? sameBoneOrder = null, IEventArgs args = null)
        {
            PartAssetData data;
            PartAssetData? result = null;
            if (sameBoneOrder == null)
            {
                data = PartAssetData.Create(partIdx, renderer, args);
            }
            else
            {
                data = PartAssetData.Create(partIdx, renderer, (bool)sameBoneOrder, args);
            }

            agent.ChangeOrAttach(data, out result);

            return result == null;
        }

        public bool SkinnedMeshRenderer(ComCostumeAgent agent, int partIdx, SkinnedMeshRenderer renderer, bool? sameBoneOrder = null, IEventArgs args = null)
        {
            PartAssetData data;
            PartAssetData? result = null;
            if (sameBoneOrder == null)
            {
                data = PartAssetData.Create(partIdx, renderer, args);
            }
            else
            {
                data = PartAssetData.Create(partIdx, renderer, (bool)sameBoneOrder, args);
            }

            agent.ChangeOrAttach(data, out result);

            return result == null;
        }
    }
}
