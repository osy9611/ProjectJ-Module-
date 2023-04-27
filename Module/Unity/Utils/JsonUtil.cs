namespace Module.Unity.Utils
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.ComponentModel;
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

        public static T Parse<T>(JObject data, string key)
        {
            if (data.TryGetValue(key, out var value))
            {
                if (value.ToString() is T variable)
                    return variable;

                try
                {
                    if (Nullable.GetUnderlyingType(typeof(T)) != null)
                    {
                        return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
                    }

                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    return default(T);
                }
            }

            return default(T);
        }
    }

}
