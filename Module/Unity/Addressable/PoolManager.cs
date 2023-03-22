namespace Module.Unity.Addressables
{
    using Module.Unity.Utils;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PoolManager
    {
        #region GameObjectPool
        class GameObjectPool 
        {
            public GameObject Original { get; private set; }
            public Transform Root { get; private set; }

            Stack<Poolable> poolStack = new Stack<Poolable>();

            public void Init(GameObject original, int count = 5)
            {
                Original = original;
                Root = new GameObject().transform;
                Root.name = $"{original.name}_Root";

                for (int i = 0; i < count; ++i)
                    Push(Create());
            }

            Poolable Create()
            {
                GameObject go = Object.Instantiate<GameObject>(Original);
                go.name= Original.name;
                return go.GetOrAddComponent<Poolable>();
            }

            public void Push(Poolable poolable)
            {
                if (poolable == null)
                    return;

                poolable.transform.SetParent(Root);
                poolable.gameObject.SetActive(false);
                poolable.IsUsing = false;

                poolStack.Push(poolable);
            }

            public Poolable Pop(Transform parent) 
            {
                Poolable poolable;

                if (poolStack.Count > 0)
                    poolable = poolStack.Pop();
                else
                    poolable = Create();

                poolable.gameObject.SetActive(true);
                poolable.transform.SetParent(parent);
                poolable.IsUsing = true;

                return poolable;
            }

        }
        #endregion

        Dictionary<string, GameObjectPool> pools = new Dictionary<string, GameObjectPool>();
        Transform root;

        public void Init()
        {
            if(root == null)
            {
                root = new GameObject { name = "@Pool_Root" }.transform;
                Object.DontDestroyOnLoad(root);
            }
        }

        public void CreatePool(GameObject original,int count =5)
        {
            GameObjectPool pool = new GameObjectPool();
            pool.Init(original, count);
            pool.Root.parent = root;

            pools.Add(original.name, pool);
        }

        public bool Push(Poolable poolable)
        {
            string name = poolable.gameObject.name;
            if(!pools.ContainsKey(name))
            {
                GameObject.Destroy(poolable.gameObject);
                return false;
            }

            pools[name].Push(poolable);
            return true;
        }

        public Poolable Pop(GameObject original, Transform parent = null)
        {
            if (!pools.ContainsKey(original.name))
                CreatePool(original);

            return pools[original.name].Pop(parent);
        }

        public GameObject GetOriginal(string name)
        {
            if (!pools.ContainsKey(name))
                return null;

            return pools[name].Original;
        }


        public void Clear()
        {

        }

        public void ClearAll()
        {
            foreach (Transform child in root)
                GameObject.Destroy(child.gameObject);

            pools.Clear();
        }

    }

}
