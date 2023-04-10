namespace Module.Unity.Quest
{
    using Module.Core.Systems.Collections.Generic;
    using Module.Core.Systems.Events;
    using System.Collections.Generic;
    using UnityEngine;

    public class QuestManager
    {
        private Dictionary<int, UnorderedList<IQuest>> questList = new Dictionary<int, UnorderedList<IQuest>>();

        private EventEmmiter eventEmmiter = new EventEmmiter();
        public EventEmmiter EventEmmiter { get => eventEmmiter; }
        public void Init()
        {
        }


        public void AddQuest<T>(short id) where T : IQuest, new()
        {
            T quest = new T();
            quest.GetQuestData(id);

            if (!questList.TryGetValue(quest.Type, out var value))
            {
                value = new UnorderedList<IQuest>() { quest };
                questList.Add(quest.Type, value);
            }
            else
            {
                value.Add(quest);
                questList.Add(quest.Type, value);
            }
        }

        public IQuest GetQuest(short type, int id)
        {
            if (questList.TryGetValue((int)type, out var value))
            {
                return value.Find(x => x.Id == id);
            }

            return null;
        }


        public UnorderedList<IQuest> GetQuestList(short type)
        {
            if (questList.TryGetValue(type, out var val))
            {
                return val;
            }

            return null;
        }

        public void CheckQuest(int type, string data)
        {
            if (questList.TryGetValue(type, out var value))
            {
                for (int i = value.Count - 1; i >= 0; --i)
                {
                    if (!value[i].CheckQuestData(data))
                        continue;

                    value[i].CheckQuest(data);

                    if (value[i].Clear &&value[i].GetAutoReward)
                        value[i].GetReward();
                }
                eventEmmiter.Invoke();
            }
        }

        public void GetReward(int type, int id)
        {
            if(questList.TryGetValue(type, out var value))
            {
                value.Find(x => x.Id == id).GetReward();
            }
        }

        public void RemoveQuest(short type, IQuest quest)
        {
            if (questList.TryGetValue(type, out var value))
            {
                value.Remove(quest);
            }
            else
            {
                Debug.LogError("Null Quest");
            }
        }

    }
}

