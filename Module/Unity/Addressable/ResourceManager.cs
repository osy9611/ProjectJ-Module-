namespace Module.Unity.Addressables
{
    using Module.Unity.Core;
    using Module.Unity.Utils;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.UI;

    public class ResourceManager
    {
        private Dictionary<string, AsyncOperationHandle> datas = new Dictionary<string, AsyncOperationHandle>();
        private bool initCalled;
        PoolManager poolManager;

        public void Init(PoolManager pool)
        {
            if (!initCalled)
            {
                initCalled = true;
                poolManager = pool;
            }
        }

        public GameObject LoadAndInisiate(string path, Transform parent = null)
        {
            GameObject original = LoadAndGet<GameObject>(path);
            if (original == null)
            {
                Debug.Log($"Fail to load prefab : {path} ");
                return null;
            }

            GameObject go = UnityEngine.Object.Instantiate(original, parent);
            go.name = original.name;
            return go;
        }

        public T LoadAndPop<T>(string path, Transform parent = null) where T : UnityEngine.Component
        {
            GameObject original = LoadAndGet<GameObject>(path);
            if (original == null)
            {
                Debug.Log($"Fail to load prefab : {path} ");
                return null;
            }
            original.GetOrAddComponent<T>();
            return poolManager.Pop(original, parent).gameObject.GetOrAddComponent<T>();
        }

        public GameObject LoadAndPop(string path, Transform parent = null)
        {
            GameObject original = LoadAndGet<GameObject>(path);
            if (original == null)
            {
                Debug.Log($"Fail to load prefab : {path} ");
                return null;
            }

            return poolManager.Pop(original, parent).gameObject;
        }

        public GameObject LoadAndPop(string path, Transform parent = null, int poolCount = 1)
        {
            GameObject original = LoadAndGet<GameObject>(path);
            if (original == null)
            {
                Debug.Log($"Fail to load prefab : {path} ");
                return null;
            }
            poolManager.CreatePool(original, poolCount);
            return poolManager.Pop(original, parent).gameObject;
        }

        public void LoadAndPool(string path, Transform parent = null, int poolCount = 1)
        {
            GameObject original = LoadAndGet<GameObject>(path);
            if (original == null)
            {
                Debug.Log($"Fail to load prefab : {path} ");
                return;
            }
            if (poolManager.GetOriginal(original.name) != null)
                return;
            poolManager.CreatePool(original, poolCount);
        }

        public void LoadAndPool<T>(string path, Transform parent = null, int poolCount = 1) where T : UnityEngine.Component
        {
            GameObject original = LoadAndGet<GameObject>(path);
            if (original == null)
            {
                Debug.Log($"Fail to load prefab : {path} ");
                return;
            }
            if (poolManager.GetOriginal(original.name) != null)
                return;
            original.GetOrAddComponent<T>();
            poolManager.CreatePool(original, poolCount);
        }

        public void Destory(GameObject go, bool destoryPool = false)
        {
            Poolable poolable = go.GetComponent<Poolable>();
            if (poolable != null)
            {
                poolManager.Push(poolable);
                return;
            }
            else
            {
                foreach (var handle in datas.Keys)
                {
                    if (handle.Contains(go.gameObject.name))
                    {
                        if (datas[handle].IsValid())
                        {
                            Release(handle);
                            datas.Remove(handle);
                        }
                        break;
                    }
                }
            }
            UnityEngine.Object.Destroy(go);
        }

        public void LoadScene(string key, UnityEngine.SceneManagement.LoadSceneMode loadMode, System.Action<bool> resultCallback)
        {
            ComLoader.Root.StartCoroutine(CoLoadSceneAsync(key, loadMode, resultCallback));
        }

        public void LoadSceneUI(string key, UnityEngine.SceneManagement.LoadSceneMode loadMode, System.Action<bool> resultCallback, Image progressbar)
        {
            ComLoader.Root.StartCoroutine(CoLoadSceneAsync(key, loadMode, resultCallback, progressbar));
        }

        public IEnumerator CoLoadSceneAsync(string key, UnityEngine.SceneManagement.LoadSceneMode loadMode, System.Action<bool> resultCallback)
        {
            var handle = Addressables.LoadSceneAsync(key, loadMode);
            while (!handle.IsDone)
            {
                yield return null;
            }


            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
                resultCallback(true);
            else
                resultCallback(false);
        }

        public IEnumerator CoLoadSceneAsync(string key, UnityEngine.SceneManagement.LoadSceneMode loadMode, System.Action<bool> resultCallback, Image progressbar)
        {
            float timer = 0.0f;
            var handle = Addressables.LoadSceneAsync(key, loadMode);

            while (!handle.IsDone)
            {
                yield return null;
                timer += Time.deltaTime;

                if (progressbar != null)
                {
                    if (handle.GetDownloadStatus().Percent < 0.9f)
                    {
                        progressbar.fillAmount = Mathf.Lerp(progressbar.fillAmount, handle.GetDownloadStatus().Percent, timer);
                        if (progressbar.fillAmount >= handle.GetDownloadStatus().Percent)
                        {
                            timer = 0;
                        }
                    }
                    else
                    {
                        progressbar.fillAmount = Mathf.Lerp(progressbar.fillAmount, 1.0f, timer);
                        if (progressbar.fillAmount == 1.0f)
                            yield break;
                    }
                }
            }

            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
                resultCallback(true);
            else
                resultCallback(false);
        }

        public T LoadAndGet<T>(string addressable)
        {
            if (datas.ContainsKey(addressable))
                return (T)datas[addressable].Result;

            var handle = Addressables.LoadAssetAsync<T>(addressable);

            handle.WaitForCompletion();

            if (handle.Status != AsyncOperationStatus.Succeeded)
                return default(T);

            datas.Add(addressable, handle);

            return handle.Result;
        }

        public T LoadAndGet<T>(AssetReference assetRef)
        {
            string addressable = null;
            GetAddressable(assetRef, (result) =>
            {
                addressable = result;
            });

            if (string.IsNullOrEmpty(addressable))
                return default(T);

            if (datas.ContainsKey(addressable))
                return (T)datas[addressable].Result;

            var handle = assetRef.LoadAssetAsync<T>();

            handle.WaitForCompletion();
            if (handle.Status != AsyncOperationStatus.Succeeded)
                return default(T);

            datas.Add(addressable, handle);

            return handle.Result;
        }

        public void CoGetAddressable(AssetReference assetRef, Action<string> callback)
        {
            ComLoader.Root.StartCoroutine(GetAddressable(assetRef, callback));
        }

        public IEnumerator GetAddressable(AssetReference assetRef, Action<string> callback = null)
        {
            string result = null;
            var handle = Addressables.LoadResourceLocationsAsync(assetRef);
            yield return handle;

            using (AsyncOperationDisposer disposer = new AsyncOperationDisposer(handle))
            {
                if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null
                   && handle.Result.Count > 0)
                {
                    result = handle.Result[0].InternalId;
                }
                else
                {
                    Debug.Log("Fail To Load LoadResourceLocationsAsync");
                }
            }

            callback?.Invoke(result);
        }


        public void LoadAsset<T>(AssetReference assetRef, System.Action<T> callback, bool autoReleaseOnFail = true)
        {
            ComLoader.Root.StartCoroutine(CoLoadAsset<T>(assetRef, callback, autoReleaseOnFail));
        }

        public IEnumerator CoLoadAsset<T>(AssetReference assetRef, System.Action<T> callback, bool autoReleaseOnFail = true)
        {
            string addressable = null;
            yield return GetAddressable(assetRef, (result) =>
            {
                addressable = result;
            });

            if (string.IsNullOrEmpty(addressable))
                callback?.Invoke(default(T));

            AsyncOperationHandle handle;
            if (datas.ContainsKey(addressable))
            {
                handle = datas[addressable];
                callback?.Invoke((T)handle.Result);
            }
            else
            {
                handle = assetRef.LoadAssetAsync<T>();

                yield return handle;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    if (!datas.ContainsKey(addressable))
                        datas.Add(addressable, handle);
                    callback?.Invoke((T)handle.Result);
                }
                else
                {
                    if (autoReleaseOnFail)
                    {
                        assetRef.ReleaseAsset();
                    }

                    callback?.Invoke(default(T));
                }
            }
        }

        public void LoadAsset<T>(string addessable, System.Action<T> callback, bool autoRelease = true)
        {
            ComLoader.Root.StartCoroutine(CoLoadAsset<T>(addessable, callback, autoRelease));
        }

        public IEnumerator CoLoadAsset<T>(string addressable, System.Action<T> callback, bool autoRelease = true)
        {
            if (datas.ContainsKey(addressable))
            {
                var handle = datas[addressable];
                callback?.Invoke((T)handle.Result);
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<T>(addressable);

                yield return handle;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    if (!datas.ContainsKey(addressable))
                        datas.Add(addressable, handle);
                    callback?.Invoke(handle.Result);
                }
                else
                {
                    callback?.Invoke(default(T));

                    if (autoRelease)
                    {
                        if (handle.IsValid())
                            Addressables.Release(handle);
                    }
                }

            }
        }

        public void Release(AsyncOperationHandle handle)
        {
            Addressables.Release(handle);
        }

        public void Release(string addressable)
        {
            if (datas.TryGetValue(addressable, out var handle))
            {
                Addressables.Release(handle);
                datas.Remove(addressable);
            }

        }

        public void Release(AssetReference assetRef)
        {
            if (assetRef.Asset == null)
                return;
            string addressable = null;
            CoGetAddressable(assetRef, (result) =>
            {
                addressable = result;
            });

            if (string.IsNullOrEmpty(addressable))
                return;

            if (datas.TryGetValue(addressable, out var handle))
            {
                Addressables.Release(handle);
                datas.Remove(addressable);
            }
        }
        public void ReleaseAll()
        {
            foreach (var data in datas.Values)
            {
                Release(data);
            }
            datas.Clear();
        }

        public void Clear()
        {
            ReleaseAll();
            poolManager = null;
        }
    }
}
