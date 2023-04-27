using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Module.Core.Systems.Collections.Generic;
using UnityEngine;

namespace Module.Core.Systems.Events
{
    public class EventEmmiter
    {
        private UnorderedList<ListenerDelegate> values;
        private int listenerCapapcity;

        public EventEmmiter(int listenrCapacity = 50)
        {
            values = new UnorderedList<ListenerDelegate>();
            this.listenerCapapcity = listenrCapacity;
        }

        public bool CheckListener(ListenerDelegate listener)
        {
            return values.Contains(listener);
        }

        public void AddListener(ListenerDelegate listener) 
        {
            values.Add(listener);
        }
        public void RemoveAllListener()
        {
            values.Clear();
        }

        public void RemoveListener(ListenerDelegate listener)
        {
            values.Remove(listener);
        }

        public void Invoke()
        {
            for(int i=0,range= values.Count;i<range;++i)
            {
                if (i >= values.Count) break;
                values[i].Invoke();
            }
        }
    }
}

