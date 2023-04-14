namespace Module.Unity.UGUI.Notification
{
    using Module.Core.Systems.Events;
    using TMPro;
    using UnityEngine;

    public class ComNotificationTMP : ComNotification
    {
        [SerializeField] TextMeshProUGUI text;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void OnSetData(IArgs args)
        {
            Args<string>? val = args as Args<string>?;

            if (text == null||!val.HasValue)
                return;

            text.text = val.Value.Arg1;

            Start();
        }

    }

}
