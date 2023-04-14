
namespace Module.Unity.Quest
{
    using Module.Core.Systems.Collections.Generic;
    using Module.Core.Systems.Events;
    using System;
    using System.Collections.Generic;

    public interface IQuest
    {
        public string Name { get; }
        public int Id { get; }
        public int Type { get;}
        public bool Clear { get;}
        public bool GetAutoReward { get; }
        public void GetQuestData(short id);
        public void CheckQuest(string data);
        public bool CheckQuestData(string data);
        public void GetReward();
    }

    public class Quest<T> : IQuest
    {
        protected string name;
        public string Name => name;

        protected int id;
        public int Id => id;

        protected int type;
        public int Type => type;

        protected bool clear = false;
        public bool Clear => clear;

        protected bool getAutoReward = false;
        public bool GetAutoReward=>getAutoReward;

        protected T questInfo;
        protected Dictionary<Type, IReward> rewardInfos = new Dictionary<Type, IReward>();
        protected Dictionary<string, IArgs> reachQuest = new Dictionary<string, IArgs>();
        public Dictionary<string,IArgs> ReachQuest => reachQuest;
        public virtual void CheckQuest(string data)
        {
        }

        public virtual void GetQuestData(short id)
        {
        }

        public virtual bool CheckQuestData(string data)
        {
            return ReachQuest.ContainsKey(data);
        }

        public virtual void GetReward()
        {

        }

    }
}
