namespace Module.Unity.UGUI
{
    using Module.Unity.Utils;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UI_Popup : UI_Base
    {
        public System.Action<GameObject, bool> OnSetCanvasHandler;
        public System.Action<UI_Popup> OnClosePopupUIHandler;

        public override void Init()
        {
            if(OnSetCanvasHandler != null)
                OnSetCanvasHandler.Invoke(gameObject,true);
        }

        public virtual void ClosePopupUI()
        {
            if (OnClosePopupUIHandler != null)
                OnClosePopupUIHandler.Invoke(this);
        }

    }
}

