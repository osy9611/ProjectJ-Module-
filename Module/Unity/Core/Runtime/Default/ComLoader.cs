namespace Module.Unity.Core
{
    using UnityEngine;

    public class ComLoader : MonoBehaviour
    {
        static public ComLoader Root;

        private void Awake()
        {
            Root = this;
        }

        private void OnDestroy()
        {
            Root = null;
        }

        static public ComLoader Create()
        {
            if (ComLoader.Root) return ComLoader.Root;

            GameObject go = new GameObject("[Lojr]");
            DontDestroyOnLoad(go);
            return go.AddComponent<ComLoader>();
        }
    }
}