namespace Module.Unity.DayNight
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class ComDayNight : DayNight
    {
        public bool isStop;
        // Update is called once per frame
        void Update()
        {
            base.Execute();
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!isStop)
            {
                base.Execute();
            }
        }
#endif
    }
}

