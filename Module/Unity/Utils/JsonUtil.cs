namespace Module.Unity.Utils
{
    using Newtonsoft.Json.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class JsonUtil 
    {
        public static void ParseJsonArray(JObject data, string key, System.Action<JArray> callback = null)
        {
            JArray result = null;
            if (data.TryGetValue(key, out var value))
            {
                result = JArray.Parse(value.ToString());
            }
            callback?.Invoke(result);
        }
    }

}
