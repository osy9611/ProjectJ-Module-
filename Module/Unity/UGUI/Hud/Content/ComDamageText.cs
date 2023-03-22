namespace Module.Unity.UGUI.Hud
{
    using Module.Unity.Pivot;
    using TMPro;
    using UnityEngine;

    public class ComDamageText : ComHudAgent
    {
        [SerializeField] TextMeshProUGUI text;

        public string DamageText { set => text.text = value; }

        public override void Init(PivotInfo pivotInfo)
        {
            base.Init(pivotInfo);

            ani.Play();

            AniShowByIndex();

            Invoke("Destroy", ani.clip.length);
        }

        public override void Execute()
        {
            base.Execute();
        }

        protected override void Destroy()
        {
            transform.position = Vector3.zero;
            base.Destroy();
        }
    }
}

