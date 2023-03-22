namespace Module.Unity.UGUI.Notification
{
    using Module.Core.Systems.Events;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    abstract public class ComNotification : MonoBehaviour
    {
        private Animator animator;
        // Start is called before the first frame update
        protected virtual void Awake()
        {
            animator= GetComponent<Animator>();
        }

        public virtual void Start()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void End()
        {
            this.gameObject.SetActive(false);
        }

        abstract public void OnSetData(IEventArgs args);
    }

}