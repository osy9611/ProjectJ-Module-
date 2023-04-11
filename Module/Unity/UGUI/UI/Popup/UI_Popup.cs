namespace Module.Unity.UGUI
{
    using Module.Unity.Utils;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UI_Popup : UI_Base
    {
        public System.Action<GameObject, bool> OnSetCanvasHandler;
        public System.Action<UI_Popup> OnShowPopupUIHandler;
        public System.Action<UI_Popup> OnClosePopupUIHandler;

        public override void Init()
        {
            OnSetCanvasHandler?.Invoke(gameObject, true);               
        }

        public virtual void ShowPopupUI()
        {
            OnShowPopupUIHandler?.Invoke(this);
        }

        public virtual void ClosePopupUI()
        {
            OnClosePopupUIHandler?.Invoke(this);
        }

    }
}

