namespace Module.Unity.UGUI
{
    using Module.Core.Systems.Events;
    using Module.Unity.Addressables;
    using Module.Unity.UGUI.Notification;
    using Module.Unity.Utils;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UIManager
    {
        private int orderLayer = 10;

        private Stack<UI_Popup> popupStack = new Stack<UI_Popup>();
        private Dictionary<string, UI_Popup> popupInfos = new Dictionary<string, UI_Popup>();
        private UI_Scene sceneUI;

        private ResourceManager resourceManager;

        private Transform root;

        public void Init(ResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
            if (root == null)
            {
                root = new GameObject { name = "@UI_Root" }.transform;
                Object.DontDestroyOnLoad(root);
            }
        }

        public T GetScene<T>() where T : UI_Scene
        {
            if (sceneUI == null)
                return default(T);

            return sceneUI as T;
        }

        public T GetPopup<T>() where T : UI_Popup
        {
            if (sceneUI == null)
                return default(T);

            return sceneUI.GetPopup<T>();
        }

        public void ActivePopup<T>(System.Action<T> callback  =null) where T : UI_Popup
        {
            T result = GetPopup<T>();

            if (result != null)
                callback.Invoke(result);
            else
                Debug.LogError("Null Popup");
        }

        public T GetElem<T>() where T : UI_Element
        {
            if (sceneUI == null)
                return null;

            return sceneUI.GetElem<T>();
        }

        public void ActiveElem<T>(System.Action<T> callback = null) where T : UI_Element
        {
            T result = GetElem<T>();

            if (result != null)
                callback?.Invoke(result);
            else
                Debug.LogError("Null Element");
        }

        public void SetCanvas(GameObject go, bool sort = true)
        {
            Canvas canvas = ComponentUtil.GetOrAddComponent<Canvas>(go);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;

            if (sort)
            {
                canvas.sortingOrder = orderLayer;
                orderLayer++;
            }
            else
            {
                canvas.sortingOrder = 0;
            }
        }

        public T ShowSceneUI<T>(string path = null, bool isCreate = true) where T : UI_Scene
        {
            if (string.IsNullOrEmpty(path))
                return default(T);

            GameObject go = null;

            if(isCreate)
                go = resourceManager.LoadAndPop(path, root.transform, 1);
            else
                go = resourceManager.LoadAndPop(path, root.transform);

            if(go == null)
                return default(T);

            T sceneUI = ComponentUtil.GetOrAddComponent<T>(go);
            sceneUI.OnSetCanvasHandler += SetCanvas;
            sceneUI.OnAddPopupHandler += AddPopupUI;
            this.sceneUI = sceneUI;
            return sceneUI;
        }


        public void ShowPopupUI(UI_Popup popup)
        {
            if (popup == null)
                return;

            ShowPopupUI<UI_Popup>(popup.GetType().Name);
        }
        public void ShowPopupUI<T>(string name = null) where T : UI_Popup
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (popupInfos.TryGetValue(name, out var info))
            {
                info.gameObject.SetActive(true);
                popupStack.Push(info);
            }
        }

        public void AddPopupUI(UI_Popup[] popupInfos)
        {
            foreach (var popup in popupInfos)
            {
                if (popup == null)
                    continue;
                popup.gameObject.SetActive(false);

                popup.OnSetCanvasHandler += SetCanvas;
                popup.OnShowPopupUIHandler += ShowPopupUI;
                popup.OnClosePopupUIHandler += ClosePopupUI;
                this.popupInfos.Add(popup.GetType().Name, popup);
            }
        }

        public void ShowNotification(int index, IArgs args)
        {
            if (sceneUI == null)
                return;
            if (sceneUI.NotificationInfos == null)
                return;

            sceneUI.NotificationInfos[index].OnSetData(args);
        }

        public void CloseSceneUI()
        {
            if (this.sceneUI == null)
                return;

            resourceManager.Destory(this.sceneUI.gameObject);
        }

        public void ClosePopupUI(UI_Popup popup)
        {
            if (popupStack.Count == 0)
                return;
            if (popupStack.Peek() != popup)
            {
                Debug.LogError("Close Popup Fail!");
                return;
            }

            ClosePopupUI();
        }

        public void ClosePopupUI()
        {
            if (popupStack.Count == 0)
                return;
            UI_Popup popup = popupStack.Pop();
            popup.gameObject.SetActive(false);
            orderLayer--;
        }

        public void CloseAllPopupUI()
        {
            while (popupStack.Count > 0)
                ClosePopupUI();
        }

        public void Clear()
        {
            CloseAllPopupUI();
            sceneUI = null;
            resourceManager = null;
        }
    }

}

