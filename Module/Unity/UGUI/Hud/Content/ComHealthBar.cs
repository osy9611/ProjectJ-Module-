namespace Module.Unity.UGUI.Hud
{
    using UnityEngine;
    using UnityEngine.UI;

    public class ComHealthBar : ComHudAgent
    {
        [SerializeField] Image HpBar;

        public override void Execute()
        {
            base.Execute();
        }

        public void SetHP(float hp)
        {
            HpBar.fillAmount = hp;
        }
    }

}
