namespace Module.Unity.UGUI
{
    using Module.Unity.UGUI.Notification;
    using System;
    using UnityEngine;
    [System.Serializable]
    public class UI_Scene : UI_Base
    {
        public System.Action<GameObject, bool> OnSetCanvasHandler;
        public System.Action<UI_Popup[]> OnAddPopupHandler;

        [SerializeField] UI_Popup[] popupInfos;
        public UI_Popup[] PopupInfo { get => popupInfos; set => popupInfos = value; }

        [SerializeField] UI_Element[] elemInfos;
        public UI_Element[] ElemInfos { get => elemInfos; set => elemInfos = value; }

        [SerializeField] ComNotification[] notificationInfos;
        public ComNotification[] NotificationInfos => notificationInfos;

        private RectTransform hudRoot;
        [HideInInspector] public RectTransform HudRoot {get => hudRoot; set => hudRoot = value; }

        public override void Init()
        {
            if (OnSetCanvasHandler != null)
                OnSetCanvasHandler.Invoke(gameObject, false);

            if (OnAddPopupHandler != null)
                OnAddPopupHandler.Invoke(popupInfos);

            for(int i=0,range=notificationInfos.Length;i<range;++i)
            {
                notificationInfos[i].gameObject.SetActive(false);
            }
        }

        public virtual T GetPopup<T>() where T : UI_Popup
        {
            return (T)Array.Find(popupInfos, element => (element as T) != null);
        }

        public virtual T GetElem<T>() where T : UI_Element
        {
            return (T)Array.Find(ElemInfos, element => (element as T) != null);
        }
    }

}