namespace Module.Unity.UGUI 
{
    using Module.Unity.Utils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public abstract class UI_Base : MonoBehaviour
    {
        protected Dictionary<Type, UnityEngine.Object[]> objects = new Dictionary<Type, UnityEngine.Object[]>();
        
        public virtual void PostInit() { }
        public abstract void Init();
       

        private void Awake()
        {
            PostInit();
        }

        void Start()
        {
            Init();
        }

        protected void Bind<T>(Type type) where T : UnityEngine.Object
        {
            string[] names = Enum.GetNames(type);
            UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
            this.objects.Add(typeof(T), objects);

            for (int i = 0, range = names.Length; i < range; ++i)
            {
                if(gameObject.name == names[i])
                {
                    if (typeof(T) == typeof(GameObject))
                        objects[i] = gameObject;
                    else
                        objects[i] = GetComponent<T>();
                }
                else
                {
                    if (typeof(T) == typeof(GameObject))
                        objects[i] = ComponentUtil.FindChild(gameObject, names[i], true);
                    else
                        objects[i] = ComponentUtil.FindChild<T>(gameObject, names[i], true);
                }
              

                if (objects[i] == null)
                    Debug.LogError($"Fail To Bind({names[i]})");
            }
        }

        protected void Bind<T>(string[] names) where T : UnityEngine.Object
        {
            UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
            this.objects.Add(typeof(T), objects);

            for (int i = 0, range = names.Length; i < range; ++i)
            {
                if (typeof(T) == typeof(GameObject))
                    objects[i] = ComponentUtil.FindChild(gameObject, names[i], true);
                else
                    objects[i] = ComponentUtil.FindChild<T>(gameObject, names[i], true);

                if (objects[i] == null)
                    Debug.LogError($"Fail To Bind({names[i]})");
            }
        }

        protected void Bind<T>(UnityEngine.Object[] objs) where T : UnityEngine.Object
        {
            UnityEngine.Object[] objects = new UnityEngine.Object[objs.Length];
            this.objects.Add(typeof(T), objects);

            for (int i = 0, range = objs.Length; i < range; ++i)
            {
                if (typeof(T) == typeof(GameObject))
                    objects[i] = ComponentUtil.FindChild(gameObject, objs[i].name, true);
                else
                    objects[i] = ComponentUtil.FindChild<T>(gameObject, objs[i].name, true);

                if (objects[i] == null)
                    Debug.LogError($"Fail To Bind({objs[i].name})");
            }
        }

        protected T Get<T>(int idx) where T : UnityEngine.Object
        {
            UnityEngine.Object[] objects = null;
            if (!this.objects.TryGetValue(typeof(T), out objects))
                return null;

            return objects[idx] as T;
        }

        protected T[] Get<T>() where T : UnityEngine.Object
        {
            UnityEngine.Object[] objects = null;
            if (!this.objects.TryGetValue(typeof(T), out objects))
                return null;

            return Array.ConvertAll(objects, x=> (T)x);
        }

        protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
        protected Text GetText(int idx) { return Get<Text>(idx); }
        protected Button GetButton(int idx) { return Get<Button>(idx); }
        protected Image GetImage(int idx) { return Get<Image>(idx); }

        public static void BindEvent(GameObject go, Action<PointerEventData, System.Action<Vector2>> action, Define.UIEvent type = Define.UIEvent.Click)
        {
            UI_EventHandler handle = ComponentUtil.GetOrAddComponent<UI_EventHandler>(go);
            if(handle == null)
            {
                Debug.LogError("Null UI_EventHandler");
                return;
            }

            switch (type)
            {
                case Define.UIEvent.Click:
                    handle.OnClickHandler -= action;
                    handle.OnClickHandler += action;
                    break;
                case Define.UIEvent.Drag:
                    handle.OnDragHandler -= action;
                    handle.OnDragHandler += action;
                    break;
                case Define.UIEvent.Up:
                    handle.OnPointerUpHandler -= action;
                    handle.OnPointerUpHandler += action;
                    break;
                case Define.UIEvent.Down:
                    handle.OnPointerDownHandler -= action;
                    handle.OnPointerDownHandler += action;
                    break;
            }
        }


    }

}
