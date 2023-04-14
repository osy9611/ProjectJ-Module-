namespace Module.Unity.Utils
{
    using Module.Unity.Core;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Util
    {
        static public void Wait(MonoBehaviour mono, float time, System.Action action)
        {
            mono.StartCoroutine(WaitCorutine(time, action));
        }

        static internal IEnumerator WaitCorutine(float time, System.Action action = null)
        {
            yield return YieldCache.WaitForSeconds(time);

            if (action != null)
                action();
        }


        static public void Random(int min, int max, out int target)
        {
            target = UnityEngine.Random.Range(min, max);
        }

        static public void Random(float min, float max, out float target)
        {
            target = UnityEngine.Random.Range(min, max);
        }

        static public List<int> RandomDupilcate(int min, int max, int count)
        {
            List<int> result = new List<int>();
            System.Random random = new System.Random();
            while (result.Count < count)
            {
                int rnd = random.Next(min, max);
                if (!result.Contains(rnd)) result.Add(rnd);
            }
            return result;
        }


    }

}
