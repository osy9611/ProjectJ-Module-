namespace Module.Unity.Quest
{
    using Module.Core.Systems.Collections.Generic;
    using System.Collections.Generic;
    using UnityEngine;

    public class QuestManager
    {
        private Dictionary<int, UnorderedList<IQuest>> questList = new Dictionary<int, UnorderedList<IQuest>>();

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

        public UnorderedList<IQuest> GetQuestList(short type)
        {
            if(questList.TryGetValue(type,out var val))
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
                    value[i].CheckQuest(data);

                    if (value[i].Clear)
                        value[i].GetReward();
                }
            }
        }

        public void RemoveQuest(short type, IQuest quest)
        {
            if (questList.TryGetValue(type, out var value))
            {
                value.Remove(quest);
            }
        }

    }
}

