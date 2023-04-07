
namespace Module.Unity.Quest
{
    using Module.Core.Systems.Events;
    using System.Collections.Generic;

    public interface IQuest
    {
        public string Name { get; }
        public int Id { get; }
        public int Type { get;}
        public bool Clear { get;}
        public void GetQuestData(short id);
        public void CheckQuest(string data);
        public void GetReward();
    }

    public class Quest<T> : IQuest
    {
        protected T questInfo;

        protected string name;
        public string Name => name;

        protected int id;

        public int Id => id;

        protected int type;
        public int Type => type;

        protected bool clear = false;
        public bool Clear => clear;


        protected Dictionary<string, IEventArgs> reachQuest = new Dictionary<string, IEventArgs>();
        public Dictionary<string,IEventArgs> ReachQuest => reachQuest;
        public virtual void CheckQuest(string data)
        {

        }

        public virtual void GetQuestData(short id)
        {
        }

        public virtual void GetReward()
        {

        }
    }
}
