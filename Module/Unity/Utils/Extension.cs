namespace Module.Unity.Utils
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class Extension
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
        {
            return ComponentUtil.GetOrAddComponent<T>(go);
        }
    }

}

