namespace Module.Unity.UGUI.Hud
{
    using System;
    using Module.Unity.Pivot;
    using System.Collections.Generic;
    using UnityEngine;

    public class ComHudAgent : MonoBehaviour
    {
        protected PivotInfo pivotInfo;

        [SerializeField]
        protected Animation ani;
        public Animation Ani => ani;
        protected AnimationClip[] animationClips;

        public Action<GameObject, bool> onDestoy;


        protected virtual void Awake()
        {
            LoadAni();
        }

        public virtual void Init(PivotInfo pivotInfo)
        {
            this.pivotInfo = pivotInfo;
        }

        public virtual void Execute()
        {
            CalcTranform();
        }

        protected virtual void CalcTranform()
        {
            if (pivotInfo == null)
                return;
            transform.position = Camera.main.WorldToScreenPoint(pivotInfo.PivotTr.position);
        }

        protected virtual void Destroy()
        {
            if (onDestoy != null)
                onDestoy.Invoke(this.gameObject, false);
        }
        
        protected virtual void LoadAni()
        {
            if (ani != null)
            {
                animationClips = new AnimationClip[ani.GetClipCount()];

                int i = 0;
                foreach (AnimationState state in ani)
                {
                    animationClips[i] = state.clip;
                    i++;
                }
            }
        }

        protected virtual void AniShowByIndex()
        {
            int aniIdx = UnityEngine.Random.Range(0, animationClips.Length);
            ani.clip = animationClips[aniIdx];
            ani.Play();
        }
    }

}

