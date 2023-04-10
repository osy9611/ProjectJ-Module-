namespace Module.Unity.UGUI
{ 
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UI_Scene),true)]
    public class UI_SceneInspector : Editor
    {
        UI_Scene script;

        private void OnEnable()
        {
            script = (UI_Scene)target;
        }

        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Register All Static Popup"))
            {
                var allPopup = script.GetComponentsInChildren<UI_Popup>();

                script.PopupInfo = allPopup;
                EditorUtility.SetDirty(script);
            }

            if(GUILayout.Button("Register All Element"))
            {
                var allElem = script.GetComponentsInChildren<UI_Element>();

                script.ElemInfos = allElem;
                EditorUtility.SetDirty(script);
            }

            base.OnInspectorGUI();
        }
    }
}


