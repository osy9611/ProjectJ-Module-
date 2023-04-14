namespace Module.Unity.Quest
{
    using Module.Core.Systems.Events;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IReward
    {
        public void SetData();
    }

    public class Reward<T> : IReward
    {
        public List<T> rewardInfos = new List<T>();

        public virtual void SetData()
        {

        }

        protected virtual void GetReward(IArgs args)
        {

        }


    }
}
