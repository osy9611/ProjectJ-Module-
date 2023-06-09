namespace Module.Unity.UGUI.Hud
{
    using Module.Unity.Addressables;
    using Module.Unity.Pivot;
    using Module.Unity.Utils;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class HudManager
    {
        private ResourceManager resourceManager;
        private RectTransform canvas;
        public RectTransform Canvas => canvas;

        Transform rootHud;

        public void Init(ResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        public void SetHud<T>(T ui_scene) where T : UI_Scene
        {
            if (ui_scene == null)
                return;

            if (GameObject.Find("Hud") != null)
                return;

            GameObject go = new GameObject();
            go.name = "Hud";
            ui_scene.HudRoot = ComponentUtil.GetOrAddComponent<RectTransform>(go);
            ui_scene.HudRoot.transform.SetParent(ui_scene.gameObject.transform);

            canvas = ComponentUtil.GetOrAddComponent<RectTransform>(ui_scene.gameObject);
            rootHud = ui_scene.HudRoot;
        }

        public T Get<T>(string path, PivotInfo pivotInfo) where T : ComHudAgent
        {
            if (string.IsNullOrEmpty(path))
                return default(T);

            GameObject go = resourceManager.LoadAndPop(path, null);
            go.transform.SetParent(rootHud, false);
            T hud = ComponentUtil.GetOrAddComponent<T>(go);
            hud.Init(pivotInfo);
            return hud;
        }

        public T GetAndPool<T>(string path, PivotInfo pivotInfo) where T : ComHudAgent
        {
            if (string.IsNullOrEmpty(path))
                return default(T);

            GameObject go = resourceManager.LoadAndPop(path, null);
            go.transform.SetParent(rootHud, false);
            T hud = ComponentUtil.GetOrAddComponent<T>(go);
            hud.Init(pivotInfo);
            return hud;
        }

        public void Release<T>(T hudAgent) where T : ComHudAgent
        {
            if (hudAgent == null)
                return;

            resourceManager.Destory(hudAgent.gameObject);
        }

        public void Release<T>(T[] hudAgents) where T : ComHudAgent
        {
            for(int i=0,range=hudAgents.Length;i<range;++i)
            {
                resourceManager.Destory(hudAgents[i].gameObject);
            }
        }
    }
}

